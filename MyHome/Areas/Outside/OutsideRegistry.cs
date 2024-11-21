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
    private readonly NotificationSender _notifyDiningRoom;
    private readonly IHaEntity<SunState, SunAttributes> _sun;
    private readonly IHaEntity<OnOff, JsonElement> _officeDoor;
    private readonly IHaEntity<OnOff, JsonElement> _backHallLight;

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
        _notifyAboutGarage = notificationService.CreateNotificationSender([phoneChannel],[garageAlertChannel]); 
        
        this._notifyDiningRoom = _notificationService.CreateNotificationSender([
            _notificationService.CreateAudibleChannel([MediaPlayers.DiningRoom])]
        );

        this._sun = helpers.UpdatingEntityProvider.GetSun();   
        this._officeDoor = helpers.UpdatingEntityProvider.GetOnOffEntity(Sensors.OfficeDoor);
        this._backHallLight = helpers.UpdatingEntityProvider.GetOnOffEntity(Lights.BackHallLight);
    }    

    public void Register(IRegistrar reg)
    {
        reg.TryRegister(
            _helpers.Factory.DurableAutoOff(Lights.BackFlood, TimeSpan.FromMinutes(30)).WithMeta("auto off back flood","30 min"),
            _helpers.Factory.DurableAutoOff(Lights.BackPorch, TimeSpan.FromMinutes(30)).WithMeta("auto off back porch","30 min"),
            _helpers.Factory.DurableAutoOff(Lights.FrontPorchLight, TimeSpan.FromMinutes(10)).WithMeta("auto off front porch","10 min"),
            WhenDoorStaysOpen_Alert("binary_sensor.inside_garage_door_contact_opening", "Inside Garage Door"),
            WhenDoorStaysOpen_Alert("binary_sensor.front_door_contact_opening", "Front Door"),
            WhenDoorStaysOpen_Alert("binary_sensor.back_door_contact_opening", "Back Door"),
            GarageOpenAlert("Garage Door 1",GarageService.GARAGE1_CONTACT),
            GarageOpenAlert("Garage Door 2",GarageService.GARAGE2_CONTACT)
        );

        reg.TryRegister(
            OpenGarageFromSWitch,
            MakeSureGarageSwitchesAreOff,
            GarageOpens_TurnOnBackHall
        );
    }

    IAutomationBase MakeSureGarageSwitchesAreOff()
    {
        return _helpers.Factory.CreateSimple<OnOff, JsonElement>([GarageService.GARAGE1_DOOR_OPENER, GarageService.GARAGE2_DOOR_OPENER],
                (sc, ct) => sc.New.State == OnOff.On 
                    ? _services.Api.TurnOff(sc.EntityId)
                    : Task.CompletedTask).WithMeta("Garage Door switches auto-off")
            .WithMeta("Garage door swtiches","turn them off immediately");
    }

    IAutomationBase GarageOpens_TurnOnBackHall()
    {
        return _helpers.Builder.CreateSimple<OnOff>()
            .WithName("Turn on back hall light when garage door opens")
            .WithTriggers("binary_sensor.garage_1_contact_opening")
            .WithExecution(async (sc, ct) => {
                if (sc.TurnedOn())
                {
                    await TurnOnBackHallIfDark();
                }
            })
            .Build();
    }

    private async Task TurnOnBackHallIfDark()
    {
        if (_backHallLight.IsOff() && (_officeDoor.IsOff() || _sun.State == SunState.Below_Horizon))
        {
            await _services.Api.TurnOn(Lights.BackHallLight);
        }
    }

    IAutomationBase OpenGarageFromSWitch()
    {
        return _helpers.Builder.CreateSceneController()
            .WithName("Open Garage From switch")
            .WithTriggers("event.back_hall_light_scene_001", "event.back_hall_light_scene_002")
            .WithExecution((scene, ct) =>{
                if(scene.New.StateAndLastUpdatedWithin1Second())
                {
                    var btn = scene.EntityId.Last();
                    var press = scene?.New.Attributes?.GetKeyPress();
                    return (btn, press) switch
                    {
                        {btn: '1' , press: KeyPress.KeyHeldDown} => Task.WhenAll(_garage.OpenCloseGarage1(true), TurnOnBackHallIfDark()),
                        {btn: '1' , press: KeyPress.KeyPressed2x} => Task.WhenAll(_garage.OpenCloseGarage1(true), TurnOnBackHallIfDark()),
                        {btn: '2', press: KeyPress.KeyHeldDown} => _garage.OpenCloseGarage1(false),
                        {btn: '2', press: KeyPress.KeyPressed2x} => _garage.OpenCloseGarage1(false),
                        _ => Task.CompletedTask
                    };
                }
                return Task.CompletedTask;
            })
            .Build();
    }

    private IAutomationBase WhenDoorStaysOpen_Alert(string doorId, string doorName)
    {
        int seconds = 8;
        return _helpers.Builder.CreateConditional<OnOff>()
            .WithName($"{doorName} Alert")
            .WithDescription($"Notify when {doorName} stays open for {seconds} seconds")
            .WithTriggers(doorId)
            .When((stateChange) => stateChange.IsOn())
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
                await _notifyDiningRoom(message);
                                
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

    IAutomationBase GarageOpenAlert(string name, string garageContact)
    {
        var notifcationId = new NotificationId(garageContact);
        return _helpers.Builder.CreateSchedulable<OnOff>(enabledAtStartup: true)
            .WithName($"{name} alert")
            .WithDescription("notify when garage door stays open")
            .WithTriggers(garageContact)
            .MakeDurable()
            .ShouldExecutePastEvents()
            .While(sc => 
            {
                if(sc.New.State == OnOff.Off)
                {
                    _ = Task.Run(() => _notificationService.Clear(notifcationId));
                    return false;
                }
                else
                {
                    return true;
                }
            })
            .For(TimeSpan.FromHours(1))
            .WithExecution(async ct => { 
                await _notifyAboutGarage($"{name} has been open for an hour", "Garage Alert", notifcationId);
            })
            .Build();
    }
}
