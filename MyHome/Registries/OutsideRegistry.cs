using System.Net;
using System.Text.Json;
using HaKafkaNet;

namespace MyHome;

public class OutsideRegistry : IAutomationRegistry
{
    readonly IHaServices _services;
    readonly IAutomationFactory _factory;
    readonly IAutomationBuilder _builder;
    readonly ILogger _logger;
    readonly IGarageService _garage;
    private INotificationService _notificationService;
    readonly NotificationSender _notifyAboutGarage;


    public OutsideRegistry(IHaServices services, IAutomationFactory factory, IAutomationBuilder builder, ILogger<OutsideRegistry> logger, IGarageService garageService, INotificationService notificationService)
    {
        _services = services;
        _factory = factory;
        _builder = builder;
        _logger = logger;
        _garage = garageService;
        _notificationService =notificationService;

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
        reg.RegisterMultiple(
            _factory.DurableAutoOff("switch.back_flood", TimeSpan.FromMinutes(30)).WithMeta("auto off back flood","30 min"),
            _factory.DurableAutoOff("switch.back_porch_light", TimeSpan.FromMinutes(30)).WithMeta("auto off back porch","30 min"),
            _factory.DurableAutoOff("light.front_porch", TimeSpan.FromMinutes(10)).WithMeta("auto off front porch","10 min")
        );

        //door open alerts
        reg.RegisterMultiple(
            WhenDoorStaysOpen_Alert("binary_sensor.inside_garage_door_contact_opening", "Inside Garage Door"),
            WhenDoorStaysOpen_Alert("binary_sensor.front_door_contact_opening", "Front Door"),
            WhenDoorStaysOpen_Alert("binary_sensor.back_door_contact_opening", "Back Door")
        );
        
        //reg.Register(_factory.DurableAutoOn(Helpers.PorchMotionEnable, TimeSpan.FromHours(1)).WithMeta("Auto enable front porch motion","1 hour"));

        reg.Register(_builder.CreateSimple()
            .WithName("Turn on back hall light when garage door opens")
            .WithTriggers("binary_sensor.garage_1_contact_opening")
            .WithExecution(async (sc, ct) =>{
                
                if (sc.ToOnOff().TurnedOn())
                {
                    var sun = await _services.EntityProvider.GetSun();
                    if (sun?.State == SunState.Below_Horizon)
                    {
                        await _services.Api.TurnOn(Lights.BackHallLight);
                    }
                }
            })
            .Build());

        reg.Register(_builder.CreateSimple()
            .WithName("Open Grage From switch")
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
            .Build());

        reg.RegisterMultiple( 
            GarageOpenAlert("Garage Door 1",GarageService.GARAGE1_CONTACT),
            GarageOpenAlert("Garage Door 2",GarageService.GARAGE2_CONTACT));
    }

    private IConditionalAutomation WhenDoorStaysOpen_Alert(string doorId, string doorName)
    {
        int seconds = 8;
        return _builder.CreateConditional()
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
                await Task.WhenAll( 
                    _services.Api.Speak("tts.piper", "media_player.kitchen", message, cancellationToken: ct),
                    _services.Api.Speak("tts.piper", "media_player.living_room", message, cancellationToken: ct)
                );

                await Task.Delay(seconds, ct); // <-- use the cancellation token

                var doorState = await _services.EntityProvider.GetOnOffEntity(entityId, ct);
                doorOpen = doorState!.IsOn();
            } while (doorOpen && ++alertCount < 8 && !ct.IsCancellationRequested);

            if (doorOpen)
            {
                await _services.Api.NotifyGroupOrDevice("critical_notification_group", $"{friendlyName} has remained open for more than a minute", ct);
            }
        }
        catch (TaskCanceledException)
        {
            // don't do anything
            // the door was closed or
            // the application is shutting down
        }
    }  

    ISchedulableAutomation GarageOpenAlert(string name, string garageContact)
    {
        var notifcationId = new NotificationId(garageContact);
        return _builder.CreateSchedulable(enabledAtStartup: false)
            .WithName($"{name} alert")
            .WithDescription("notify when garage door stays open")
            .WithTriggers(garageContact)
            .MakeDurable()
            .ShouldExecutePastEvents()
            .GetNextScheduled(async (sc, ct) => {
                var openCloseState = sc.ToOnOff();
                if (openCloseState.New.State == OnOff.On)
                {
                    return openCloseState.New.LastUpdated.AddHours(1);
                }
                else
                {
                    await _notificationService.Clear(notifcationId);
                }
                return default;
            })
            .WithExecution(async ct => { 
                await _notifyAboutGarage($"{name} has been open for an hour", "Garage Alert", notifcationId);
            })
            .Build();
    }
}
