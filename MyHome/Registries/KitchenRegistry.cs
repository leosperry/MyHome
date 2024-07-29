using HaKafkaNet;

namespace MyHome;


public class KitchenRegistry : IAutomationRegistry
{
    readonly IHaServices _services;
    readonly IAutomationBuilder _builder;
    readonly IAutomationFactory _factory;

    public KitchenRegistry(IHaServices services, IAutomationBuilder builder, IAutomationFactory factory)
    {
        _services = services;
        _builder = builder;
        _factory = factory;
    }

    public void Register(IRegistrar reg)
    {
        var minutesToLeaveOn = 5;
        var turnOffAfterNoOccupancy = _builder.CreateSchedulable()
            .WithName("Turn Off Kitchen Lights")
            .WithDescription($"Turn off the kitchen lights when unoccupied for {minutesToLeaveOn} minutes")
            .MakeDurable()
            .WithTriggers(Sensors.KitchenPresence)
            .GetNextScheduled(async (sc, ct) => {
                DateTime? time = default;
                if (sc.ToOnOff().New.State == OnOff.Off)
                {
                    var lightStatus = await _services.EntityProvider.GetLightEntity(Lights.KitchenLights);
                    if (lightStatus?.State == OnOff.On)
                    {
                        time = DateTime.Now.AddMinutes(minutesToLeaveOn);
                    }
                }
                return time;
            })
            .WithExecution(ct => {
                return _services.Api.TurnOff(Lights.KitchenLights);
            })
            .Build();

        var turnOn = _builder.CreateSimple()
            .WithName("Turn On Kithen Lights")
            .WithDescription("If ambient light is low, turn on kitchen lights")
            .WithTriggers(Sensors.KitchenZone1AllCount)
            .WithExecution(async (sc, ct) => {
                var solaredge_current_power = await _services.EntityProvider.GetFloatEntity(Devices.SolarPower);
                if (solaredge_current_power?.State < 1000)
                {
                    await _services.Api.LightSetBrightness(Lights.KitchenLights, Bytes._20pct);
                }
            })
            .Build();

        var doubleTap = _builder.CreateSimple()
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
            .Build();
        
        reg.RegisterMultiple(turnOn, doubleTap);
        reg.RegisterMultiple(turnOffAfterNoOccupancy);
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
