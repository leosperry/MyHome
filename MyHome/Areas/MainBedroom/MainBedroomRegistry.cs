using System;
using System.Text.Json;
using HaKafkaNet;

namespace MyHome.Areas.MainBedroom;

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

        this._solar = helpers.UpdatingEntityProvider.GetFloatEntity(Devices.SolarPower);
    }

    public void Register(IRegistrar reg)
    {

        // reg.TryRegister(
        //     () => _helpers.Factory.DurableAutoOffOnEntityOff([Lights.MainBedroomLight1, Lights.MainBedroomLight2, Lights.CraftRoomLights], Sensors.MainBedroom4in1Motion, TimeSpan.FromMinutes(10))
        //         .WithMeta("mainbedroom off on no motion","10 minutes")
        // );
        
        reg.TryRegister(
            SoftLightOnMotion,
            TurnOffSoftLight,
            TurnOffOverhead
            );
    }

    IAutomationBase SoftLightOnMotion()
    {
        return _helpers.Builder.CreateSimple<OnOff>()
            .WithName("MBR Soft Lighting On")
            .WithDescription("Turns on floor lighing with motion")
            .WithTriggers(Sensors.MainBedroomMotion1, Sensors.MainBedroomMotion2)
            .WithExecution(async (sc, ct) => {
                if (sc.IsOn() && _solar.State < _threshold)
                {
                    await _services.Api.TurnOn([Lights.MainBedroomDadSideSwitch, Lights.MainBedroomDresserSwitch], ct);
                }
            })
            .Build();
    }

    IAutomationBase TurnOffSoftLight()
    {
        return _helpers.Builder.CreateSchedulable<OnOff>()
            .WithName("MBR Soft Lighting Off")
            .WithTriggers(Sensors.MainBedroomMotion1, Sensors.MainBedroomMotion2, Lights.MainBedroomDresserSwitch)
            .While(sc => sc.EntityId switch
            {
                Lights.MainBedroomDresserSwitch => sc.IsOn(),
                _ => sc.IsOff()
            })
            .ForMinutes(10)
            .MakeDurable()
            .WithExecution(async ct => 
            {
                await _services.Api.TurnOff([Lights.MainBedroomDadSideSwitch, Lights.MainBedroomDresserSwitch], ct);
            })
            .Build();
    }

    IAutomationBase TurnOffOverhead()
    {
        return _helpers.Builder.CreateSchedulable<OnOff>()
            .WithName("turn off main bedroom overhead lights")
            .WithDescription("")
            .WithTriggers(Lights.MainBedroomOverhead, Sensors.MainBedroomMotion1, Sensors.MainBedroomMotion2)
            .While(sc => 
                sc.EntityId switch
                {
                    Lights.MainBedroomOverhead => sc.IsOn(),
                    _ => sc.IsOff()
                })
            .ForMinutes(5)
            .MakeDurable()
            .WithExecution(async ct =>
            {
                await _services.Api.TurnOff(Lights.MainBedroomOverhead);
            })
            .Build();
    }
}
