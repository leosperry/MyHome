using HaKafkaNet;

namespace MyHome;

public class MainRegistry : IAutomationRegistry
{
    readonly IHaServices _services;
    readonly IAutomationFactory _factory;
    readonly IAutomationBuilder _builder;

    readonly ILivingRoomService _livingRoomService;
    readonly INotificationService _notificationService;
    readonly NotificationSender _notifyKitchenLivingRoom;
    readonly NotificationSenderNoText _notifyPressure;

    public MainRegistry(IHaServices services, IAutomationFactory factory, IAutomationBuilder builder, ILivingRoomService livingRoomService, INotificationService notificationService)
    {
        _services = services;
        _factory = factory;
        _builder = builder;
        _livingRoomService = livingRoomService;
        _notificationService = notificationService;

        var channel = notificationService.CreateAudibleChannel([MediaPlayers.Kitchen, MediaPlayers.LivingRoom]);
        _notifyKitchenLivingRoom = notificationService.CreateNotificationSender([channel]);

        _notifyPressure = _notificationService.CreateNoTextNotificationSender([_notificationService.CreateMonkeyChannel(new()
        {
            EntityId = [Lights.Monkey],
            ColorName = "darkslateblue",
            Brightness = Bytes._50pct,
        })]);
        
    }

    public void Register(IRegistrar reg)
    {
        SetupKitcheLights(reg);
        
        // lights auto off
        reg.RegisterMultiple(
            _factory.DurableAutoOff("switch.back_hall_light", TimeSpan.FromMinutes(10)).WithMeta("auto off back hall","10 min"),
            _factory.DurableAutoOff("light.upstairs_hall", TimeSpan.FromMinutes(30)).WithMeta("auto off upstairs hall","30 min"),
            _factory.DurableAutoOff("light.entry_light", TimeSpan.FromMinutes(30)).WithMeta("auto off entry light","30 min"),
            _factory.DurableAutoOffOnEntityOff([Lights.MainBedroomLight1, Lights.MainBedroomLight2, Lights.CraftRoomLights], Sensors.MainBedroom4in1Motion, TimeSpan.FromMinutes(10))
                .WithMeta("mainbedroom off on no motion","10 minutes")           
        );

        //brush lyra hair
        reg.Register(_builder.CreateSimple()
            .WithName("Lyra Brush Hair")
            .WithTriggers("binary_sensor.lyra_brush_hair")
            .WithExecution((sc, ct) => {
                _notifyKitchenLivingRoom("Time to brush Lyra's hair");
                return Task.CompletedTask;
            })
            .Build());
        
        // make sure switches are off
        reg.Register(
            _factory.SimpleAutomation([GarageService.GARAGE1_DOOR_OPENER, GarageService.GARAGE2_DOOR_OPENER],
                (sc, ct) => sc.ToOnOff().New.State == OnOff.On 
                    ? _services.Api.TurnOff(sc.EntityId)
                    : Task.CompletedTask).WithMeta("Garage Door switches auto-off")
            .WithMeta("Garage door swtiches","turn them off immediately")
        );

        reg.Register(_builder.CreateSimple()
            .WithName("Rachel Phone Battery")
            .WithDescription("Alert when her battery is low")
            .WithTriggers(Helpers.RachelPhoneBatteryHelper)
            .WithExecution((sc, ct) => sc.ToOnOff().New.State == OnOff.On ? _notifyKitchenLivingRoom("Rachel, your phone battery is low") : Task.CompletedTask)
            .Build());

        reg.Register(_builder.CreateSimple()
            .WithName("Living Room - Set lights when override disabled")
            .WithDescription("When the living room override is turned off, set the lights based on power reading")
            .WithTriggers(Helpers.LivingRoomOverride)
            .WithExecution((sc, ct) =>{
                var onOff = sc.ToOnOff();
                return sc.ToOnOff().IsOff() switch
                {
                    true => _livingRoomService.SetLightsBasedOnPower(),
                    _ => Task.CompletedTask
                };
            })
            .Build());

        reg.Register(_builder.CreateSimple()
            .WithName("Barometric Pressure Alert")
            .WithDescription("Notify when pressure drops by 0.04 over 4 hours")
            .WithTriggers("sensor.pressure_change_4_hr")
            .WithExecution(async (sc, ct) =>{
                var pressure = sc.ToFloatTyped();
                if (pressure.BecameLessThanOrEqual(-0.04f))
                {
                    await _notifyPressure();
                }
            })
            .Build());
    }

    private void SetupKitcheLights(IRegistrar reg)
    {
        reg.Register(_builder.CreateSimple()
            .WithName("Kitchen double tap")
            .WithDescription("increase brightness")
            .WithTriggers("event.kitchen_lights_scene_001", "event.kitchen_lights_scene_002")
            .WithExecution((sc, ct) => {
                var sceneController = sc.ToSceneControllerEvent();
                var press = sceneController?.New.Attributes?.GetKeyPress();
                var scene = sc.EntityId.Last();
                
                return (scene, press) switch
                {
                    {scene: '1', press: KeyPress.KeyPressed} => UpKitchenLights(ct),
                    {scene: '1', press: KeyPress.KeyPressed2x } => _services.Api.LightTurnOn(new LightTurnOnModel()
                    {
                        EntityId = [Lights.KitchenLights],
                        BrightnessStepPct = 10
                    }),
                    {scene: '2', press: KeyPress.KeyPressed} => TurnOffKitchen(ct),
                    _ => Task.CompletedTask
                };
            })
            .Build());
    }

    private async Task TurnOffKitchen(CancellationToken ct)
    {
        var kitchenLights = await _services.EntityProvider.GetLightEntity(Lights.KitchenLights);
        if (kitchenLights?.Attributes?.Brightness > 26)//10%
        {
            //make sure that when we turn back on, we don't blind everyone.
            await _services.Api.LightSetBrightness(Lights.KitchenLights, Bytes._10pct, ct);
            await Task.Delay(500);
            await _services.Api.TurnOff(Lights.KitchenLights);
        }
    }

    private async Task UpKitchenLights(CancellationToken ct)
    {
        var kitchenLights = await _services.EntityProvider.GetLightEntity(Lights.KitchenLights);
        if (kitchenLights?.State == OnOff.On)
        {
            await _services.Api.LightTurnOn(new LightTurnOnModel()
            {
                EntityId = [Lights.KitchenLights],
                BrightnessStepPct = 10
            });
        }
    }

}
