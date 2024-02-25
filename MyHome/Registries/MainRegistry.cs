using HaKafkaNet;

namespace MyHome;

public class MainRegistry : IAutomationRegistry
{
    readonly IHaServices _services;
    readonly IAutomationFactory _factory;
    readonly IAutomationBuilder _builder;

    public MainRegistry(IHaServices services, IAutomationFactory factory, IAutomationBuilder builder)
    {
        _services = services;
        _factory = factory;
        _builder = builder;
    }

    public void Register(IRegistrar reg)
    {
        SetupKitcheLights(reg);

        reg.Register(_factory.LightOffOnNoMotion(
            ["binary_sensor.lumi_lumi_sensor_motion_aq2_motion"],
            ["light.office_led_light", "light.office_lights"], TimeSpan.FromMinutes(10)).WithMeta("Office Light Off","10 minutes"));
        
        // lights auto off
        reg.RegisterMultiple(
            _factory.DurableAutoOff("switch.back_hall_light", TimeSpan.FromMinutes(10)).WithMeta("auto off back hall","10 min"),
            _factory.DurableAutoOff("light.upstairs_hall", TimeSpan.FromMinutes(30)).WithMeta("auto off upstairs hall","30 min"),
            _factory.DurableAutoOff("light.entry_light", TimeSpan.FromMinutes(30)).WithMeta("auto off entry light","30 min")
        );

        //brush lyra hair
        reg.Register(_builder.CreateSimple()
            .WithName("Lyra Brush Hair")
            .WithTriggers("binary_sensor.lyra_brush_hair")
            .WithExecution((sc, ct) => _services.Api.NotifyAlexaMedia("Time to brush Lyra's hair", [Alexa.LivingRoom, Alexa.Kitchen], ct))
            .Build());
        
        // make sure switches are off
        reg.Register(
            _factory.SimpleAutomation([GarageService.GARAGE1_DOOR_OPENER, GarageService.GARAGE2_DOOR_OPENER],
                (sc, ct) => sc.ToOnOff().New.State == OnOff.On 
                    ? _services.Api.TurnOff(sc.EntityId)
                    : Task.CompletedTask).WithMeta("Garage Door switches auto-off")
            .WithMeta("Garage door swtiches","turn them off immediately")
        );
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
