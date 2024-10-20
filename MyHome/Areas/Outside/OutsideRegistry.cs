using System.Net;
using System.Text.Json;
using HaKafkaNet;

namespace MyHome;

public class OutsideRegistry : IAutomationRegistry
{
    readonly IHaServices _services;
    readonly IStartupHelpers _helpers;
    readonly IGarageService _garage;
    private INotificationService _notificationService;
    private readonly IHaEntity<OnOff, JsonElement> _maintenanceMode;
    readonly NotificationSender _notifyAboutGarage;


    public OutsideRegistry(IHaServices services, IStartupHelpers helpers, IGarageService garageService, INotificationService notificationService)
    {
        _services = services;
        _helpers = helpers;

        _garage = garageService;
        _notificationService =notificationService;

        _maintenanceMode = _helpers.UpdatingEntityProvider.GetOnOffEntity(Helpers.MaintenanceMode);

        var garageAlertChannel = notificationService.CreateMonkeyChannel(new LightTurnOnModel()
        {
            EntityId = [Lights.Monkey],
            ColorName = "saddlebrown",
            Brightness = Bytes.Max
        });
        var phoneChannel = notificationService.CreateGroupOrDeviceChannel([Phones.LeonardPhone]);
        _notifyAboutGarage = notificationService.CreateNotificationSender([phoneChannel],[garageAlertChannel]);    }    

    public void Register(IRegistrar reg)
    {
        reg.RegisterDelayed(
            _helpers.Factory.DurableAutoOff("switch.back_flood", TimeSpan.FromMinutes(30)).WithMeta("auto off back flood","30 min"),
            _helpers.Factory.DurableAutoOff("switch.back_porch_light", TimeSpan.FromMinutes(30)).WithMeta("auto off back porch","30 min"),
            _helpers.Factory.DurableAutoOff("light.front_porch", TimeSpan.FromMinutes(10)).WithMeta("auto off front porch","10 min")
        );

        //door open alerts
        reg.RegisterDelayed(
            WhenDoorStaysOpen_Alert("binary_sensor.inside_garage_door_contact_opening", "Inside Garage Door"),
            WhenDoorStaysOpen_Alert("binary_sensor.front_door_contact_opening", "Front Door"),
            WhenDoorStaysOpen_Alert("binary_sensor.back_door_contact_opening", "Back Door")
        );

        reg.Register(
            MakeSureGarageSwitchesAreOff(),
            OpenGarageFromSWitch(), 
            GarageOpens_TurnOnBackHall());

        reg.RegisterDelayed( 
            GarageOpenAlert("Garage Door 1",GarageService.GARAGE1_CONTACT),
            GarageOpenAlert("Garage Door 2",GarageService.GARAGE2_CONTACT));

    }

    IAutomation MakeSureGarageSwitchesAreOff()
    {
        return _helpers.Factory.SimpleAutomation([GarageService.GARAGE1_DOOR_OPENER, GarageService.GARAGE2_DOOR_OPENER],
                (sc, ct) => sc.ToOnOff().New.State == OnOff.On 
                    ? _services.Api.TurnOff(sc.EntityId)
                    : Task.CompletedTask).WithMeta("Garage Door switches auto-off")
            .WithMeta("Garage door swtiches","turn them off immediately");
    }

    IAutomation GarageOpens_TurnOnBackHall()
    {
        return _helpers.Builder.CreateSimple()
            .WithName("Turn on back hall light when garage door opens")
            .WithTriggers("binary_sensor.garage_1_contact_opening")
            .WithExecution(async (sc, ct) => {
                
                if (sc.ToOnOff().TurnedOn())
                {
                    var sun = await _services.EntityProvider.GetSun();
                    var officeDoor = await _services.EntityProvider.GetOnOffEntity(Sensors.OfficeDoor);
                    if (officeDoor.IsOff() || sun?.State == SunState.Below_Horizon)
                    {
                        await _services.Api.TurnOn(Lights.BackHallLight);
                    }
                }
            })
            .Build();
    }

    IAutomation OpenGarageFromSWitch()
    {
        return _helpers.Builder.CreateSimple()
            .WithName("Open Garage From switch")
            .WithTriggers("event.back_hall_light_scene_001", "event.back_hall_light_scene_002")
            .WithExecution((sc, ct) =>{
                var scene = sc.ToSceneControllerEvent();
                if(scene.New.StateAndLastUpdatedWithin1Second())
                {
                    var btn = scene.EntityId.Last();
                    var press = scene?.New.Attributes?.GetKeyPress();
                    return (btn, press) switch
                    {
                        {btn: '1' , press: KeyPress.KeyHeldDown} => _garage.OpenCloseGarage1(true),
                        {btn: '1' , press: KeyPress.KeyPressed2x} => _garage.OpenCloseGarage1(true),
                        {btn: '2', press: KeyPress.KeyHeldDown} => _garage.OpenCloseGarage1(false),
                        {btn: '2', press: KeyPress.KeyPressed2x} => _garage.OpenCloseGarage1(false),
                        _ => Task.CompletedTask
                    };
                }
                return Task.CompletedTask;
            })
            .Build();
    }

    private IConditionalAutomation WhenDoorStaysOpen_Alert(string doorId, string doorName)
    {
        int seconds = 8;
        return _helpers.Builder.CreateConditional()
            .WithName($"{doorName} Alert")
            .WithDescription($"Notify when {doorName} stays open for {seconds} seconds")
            .WithTriggers(doorId)
            .When((stateChange) => stateChange.New.GetStateEnum<OnOff>() == OnOff.On)
            .ForSeconds(seconds)
            .Then(ct => NotifyDoorOpen(doorId, doorName, TimeSpan.FromSeconds(seconds), ct))
            .Build();
    }

    private async Task NotifyDoorOpen(string entityId, string friendlyName, TimeSpan seconds, CancellationToken ct)
    {
        // if we get here, the door has been open for 10 seconds
        string message = $"{friendlyName} is open";
        bool doorOpen = true;
        int alertCount = 0;
        try
        {
            do
            {
                await _services.Api.SpeakPiper(MediaPlayers.DiningRoom, message);

                //await _services.Api.NotifyAlexaMedia(message, ["Kitchen", "Living Room"]);
                
                await Task.Delay(seconds, ct); // <-- use the cancellation token

                var doorState = await _services.EntityProvider.GetOnOffEntity(entityId, ct);
                doorOpen = doorState.IsOn();
            } while (doorOpen && ++alertCount < 8 && !ct.IsCancellationRequested);

            if (doorOpen)
            {
                await _services.Api.NotifyGroupOrDevice("critical_notification_group", $"{friendlyName} has remained open for more than a minute", "Door Open", ct);
            }
        }
        catch (Exception ex) when (ex is TaskCanceledException || ex is OperationCanceledException)
        {
            // don't do anything
            // the door was closed or
            // the application is shutting down
        }
    }  

    ISchedulableAutomation GarageOpenAlert(string name, string garageContact)
    {
        var notifcationId = new NotificationId(garageContact);
        return _helpers.Builder.CreateSchedulable(enabledAtStartup: true)
            .WithName($"{name} alert")
            .WithDescription("notify when garage door stays open")
            .WithTriggers(garageContact)
            .MakeDurable()
            .ShouldExecutePastEvents()
            .While(sc => 
            {
                var openCloseState = sc.ToOnOff();
                if(openCloseState.New.State == OnOff.Off)
                {
                    _ = Task.Run(() => _notificationService.Clear(notifcationId));
                    return false;
                }
                else
                {
                    return _maintenanceMode.IsOn();
                }
            })
            .For(TimeSpan.FromHours(1))
            .WithExecution(async ct => { 
                await _notifyAboutGarage($"{name} has been open for an hour", "Garage Alert", notifcationId);
            })
            .Build();
    }
}
