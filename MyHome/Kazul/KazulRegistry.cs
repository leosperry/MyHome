﻿using System.Collections;
using HaKafkaNet;

namespace MyHome;

public class KazulRegistry : IAutomationRegistry
{
    readonly IHaServices _services;
    readonly IAutomationFactory _factory;
    readonly IAutomationBuilder _builder;

    const EventTiming EVENT_TIMINGS = EventTiming.PostStartup | EventTiming.PreStartupSameAsLastCached | EventTiming.PreStartupPostLastCached | EventTiming.PreStartupNotCached;

    string[] KAZUL_UVB = ["switch.kazul_power_strip_3", "switch.kazul_power_strip_4"];

    NotificationSender _notifyCritical;

    public KazulRegistry(IHaServices services, IAutomationFactory factory, IAutomationBuilder builder, INotificationService notificationService)
    {
        _services = services;
        _factory = factory;
        _builder = builder;
        _notifyCritical = notificationService.GetCritical();
    }

    public void Register(IRegistrar reg)
    {
        var kazulSunset = _factory.SunSetAutomation(async ct => {
            await _services.Api.TurnOn(KazulAlerts.CERAMIC_SWITCH, ct);
            await _services.Api.TurnOff(KazulAlerts.HALOGEN_SWITCH, ct);
        }).WithMeta("Kazul sunset","Turn on Ceramic. Turn off Halogen");

        var kazulSunrise = _factory.SunRiseAutomation(async ct => {
            await _services.Api.TurnOn(KazulAlerts.HALOGEN_SWITCH, ct);
            await _services.Api.TurnOff(KazulAlerts.CERAMIC_SWITCH, ct);
        }).WithMeta("Kazul sunrise", "Turn off Ceramic. Turn on Halogen");

        var kazulUVB =_factory.EntityOnOffWithAnother("binary_sensor.kazul_light_time_sensor", KAZUL_UVB)
            .WithMeta("Kazul UVB", "On at 8am. Off at 8pm");

        var ensureOneIsOn = _builder.CreateSimple()
            .WithName("Kazul - ensure 1 is on")
            .WithDescription("when either the ceramic or halogen change state, ensure at least 1 is on")
            .WithTriggers(KazulAlerts.CERAMIC_SWITCH, KazulAlerts.HALOGEN_SWITCH)
            .WithExecution(async (sc, ct) =>{
                try
                {
                    var swithcStates = await Task.WhenAll(
                        _services.EntityProvider.GetOnOffEntity(KazulAlerts.CERAMIC_SWITCH),
                        _services.EntityProvider.GetOnOffEntity(KazulAlerts.HALOGEN_SWITCH)
                    );
                    bool noneOn = 
                        swithcStates[0]?.State != OnOff.On && 
                        swithcStates[1]?.State != OnOff.On;
                    if (noneOn)
                    {
                        await _notifyCritical("Kazul - Neither the ceramic nor halogen are on");
                    }
                }
                catch (System.Exception)
                {
                    await _notifyCritical("Could not verify Kazul power swites");
                    throw;
                }
            })
            .Build();
        
        reg.RegisterMultiple(kazulSunrise, kazulSunset);
        reg.RegisterMultiple(kazulUVB, ensureOneIsOn);
    }
}
