﻿using HaKafkaNet;
namespace MyHome.Dev;

[ExcludeFromDiscovery]
public class MotionBehaviorTuttorial : IAutomation, IAutomationMeta
{
    readonly string _motion, _light;
    readonly IHaServices _services;
    public MotionBehaviorTuttorial(string motion, string light, IHaServices services)
    {
        _motion = motion;
        _light = light;
        _services = services;    
    }

    public IEnumerable<string> TriggerEntityIds() => [_motion];

    public async Task Execute(HaEntityStateChange stateChange, CancellationToken cancellationToken)
    {
        var homeState = await _services.EntityProvider.GetEntityState("input_boolean.am_home", cancellationToken);
        var isHome = homeState?.State == "on";

        if (isHome)
            await _services.Api.TurnOn(_light);
        else
            await _services.Api.NotifyGroupOrDevice(
                "device_tracker.my_phone", $"Motion was detected by {_motion}", cancellationToken);
    }

    public AutomationMetaData GetMetaData() =>
        new()
        {
            Name = $"Motion Behavior{_motion}",
            Description = $"Turn on {_light} if we're home, otherwise notify",
            AdditionalEntitiesToTrack = [_light]
        };
}

static class FactoryExtensions
{
    public static IAutomation CreateMotionBehavior(this IAutomationFactory factory, string motion, string light)
        => new MotionBehaviorTuttorial(motion, light, factory.Services);
}