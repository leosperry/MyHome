using System.Collections;
using HaKafkaNet;

namespace MyHome;

public class KazulRegistry : IAutomationRegistry
{
    readonly IHaServices _services;
    readonly IAutomationFactory _factory;
    readonly IAutomationBuilder _builder;

    const EventTiming EVENT_TIMINGS = EventTiming.PostStartup | EventTiming.PreStartupSameAsLastCached | EventTiming.PreStartupPostLastCached;

    string[] KAZUL_UVB = ["switch.kazul_power_strip_3", "switch.kazul_power_strip_4"];

    public KazulRegistry(IHaServices services, IAutomationFactory factory, IAutomationBuilder builder)
    {
        _services = services;
        _factory = factory;
        _builder = builder;
    }

    public void Register(IRegistrar reg)
    {
        var kazulSunset = new SunSetAutomation(ct => 
            Task.WhenAll(
                _services.Api.LightTurnOn(KazulAlerts.CERAMIC_SWITCH, ct),
                _services.Api.LightTurnOff(KazulAlerts.HALOGEN_SWITCH, ct)
            ), timings: EVENT_TIMINGS).WithMeta(new()
            {
                Name = "Kazul sunset",
                Description = "Turn on Ceramic. Turn off Halogen"
            });
        
        var kazulSunrise = new SunRiseAutomation(ct => 
            Task.WhenAll(
                _services.Api.LightTurnOn(KazulAlerts.HALOGEN_SWITCH, ct),
                _services.Api.LightTurnOff(KazulAlerts.CERAMIC_SWITCH, ct)
            ), timings: EVENT_TIMINGS).WithMeta(new()
            {
                Name = "Kazul sunset",
                Description = "Turn off Ceramic. Turn on Halogen"
            });

        var kazulUVB = _builder.CreateSimple()
            .WithName("Kazul UVB")
            .WithDescription("On at 8am. Off at 8pm")
            .WithTriggers("binary_sensor.kazul_light_time_sensor")
            .WithTimings(EVENT_TIMINGS)
            .WithAdditionalEntitiesToTrack(KAZUL_UVB)
            .WithExecution((stateChange, ct) =>
                stateChange.New.State switch{
                    "on" => _services.Api.LightTurnOn(KAZUL_UVB, ct),
                    "off" => _services.Api.LightTurnOff(KAZUL_UVB, ct),
                    _ => _services.Api.PersistentNotification("cannot set kazul UVB")
                }
            )
            .Build();
        
        reg.RegisterMultiple([kazulSunrise, kazulSunset]);
        reg.Register(kazulUVB);
    }
}
