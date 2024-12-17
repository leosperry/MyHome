using System;
using System.Text.Json;
using HaKafkaNet;

namespace MyHome;

public class MainBedroomRegistry : IAutomationRegistry
{
    private IStartupHelpers _helpers;
    private IHaServices _services;
    private ILogger<MainBedroomRegistry> _logger;
    private IUpdatingEntity<float?, JsonElement> _solar;

    const float _threshold = 200f;

    public MainBedroomRegistry(IStartupHelpers helpers, IHaServices services, ILogger<MainBedroomRegistry> logger)
    {
        this._helpers = helpers;
        this._services = services;
        this._logger = logger;

        this._solar = helpers.UpdatingEntityProvider.GetFloatEntity(Sensor.SolaredgeCurrentPower);
    }

    public void Register(IRegistrar reg)
    {
        reg.TryRegister(
            SoftLightOnMotion,
            TurnOffSoftLight,
            TurnOffOverhead,
            TurnOffCraftRoom
            );
    }

    IAutomationBase SoftLightOnMotion()
    {
        return _helpers.Builder.CreateSimple<OnOff>()
            .WithName("MBR Soft Lighting On")
            .WithDescription("Turns on floor lighing with motion")
            .WithTriggers(Binary_Sensor.MbrMotionGroup)
            .WithExecution(async (sc, ct) => {
                if (sc.IsOn() && _solar.State < _threshold)
                {
                    await _services.Api.TurnOn([Switch.MbrFloorLights], ct);
                }
            })
            .Build();
    }

    IAutomationBase TurnOffSoftLight()
    {
        return _helpers.Builder.CreateSchedulable<OnOff>()
            .WithName("MBR Soft Lighting Off")
            .WithTriggers(Binary_Sensor.MbrMotionGroup, Switch.MbrFloorLights)
            .While(sc => sc.EntityId switch
            {
                Switch.MbrFloorLights => sc.IsOn(),
                _ => sc.IsOff()
            })
            .ForMinutes(10)
            .MakeDurable()
            .WithExecution(async ct => 
            {
                await _services.Api.TurnOff([Switch.MbrFloorLights], ct);
            })
            .Build();
    }

    IAutomationBase TurnOffOverhead()
    {
        return _helpers.Builder.CreateSchedulable<OnOff>()
            .WithName("turn off main bedroom overhead lights")
            .WithDescription("")
            .WithTriggers(Light.MainBedroomOverhead, Binary_Sensor.MbrMotionGroup)
            .While(sc => 
                sc.EntityId switch
                {
                    Light.MainBedroomOverhead => sc.IsOn(),
                    _ => sc.IsOff()
                })
            .ForMinutes(5)
            .MakeDurable()
            .WithExecution(async ct =>
            {
                await _services.Api.TurnOff(Light.MainBedroomOverhead, ct);
            })
            .Build();
    }

    IAutomationBase TurnOffCraftRoom()
    {
        return _helpers.Builder.CreateSchedulable<OnOff>()
            .WithName("Turn Off Craft Room")
            .WithTriggers(Binary_Sensor.MbrMotionGroup, Light.CraftRoomLight)
            .While(sc => sc.EntityId switch {
                Light.CraftRoomLight => sc.IsOn(),
                _ => sc.IsOff()
            })
            .ForMinutes(10)
            .MakeDurable()
            .WithExecution(ct => _services.Api.TurnOff(Light.CraftRoomLight, ct))
            .Build();
    }
}
