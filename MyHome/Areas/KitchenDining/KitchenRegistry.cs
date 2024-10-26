using System.Text.Json;
using HaKafkaNet;

namespace MyHome;


public class KitchenRegistry : IAutomationRegistry
{
    readonly IHaServices _services;
    readonly IAutomationBuilder _builder;
    private readonly IHaEntity<OnOff, LightModel> _kitchenLights;
    private readonly IHaEntity<float?, JsonElement> _solarMeter;

    public KitchenRegistry(IHaServices services, IStartupHelpers helpers)
    {
        _services = services;
        _builder = helpers.Builder;

        _kitchenLights = helpers.UpdatingEntityProvider.GetLightEntity(Lights.KitchenLights);
        _solarMeter = helpers.UpdatingEntityProvider.GetFloatEntity(Devices.SolarPower);
    }

    public void Register(IRegistrar reg)
    {
        // reg.Register(
        //     Zone1Enter_TurnOn(), 
        //     Zone2Enter_TurnOnALittle()
        // );

        // reg.RegisterDelayed(
        //     NoOccupancy_for5min_TurnOff()
        // );

        var exceptions = reg.TryRegister(
            Zone1Enter_TurnOn,
            Zone2Enter_TurnOnALittle,
            NoOccupancy_for5min_TurnOff
        );
    }

    IAutomation Zone2Enter_TurnOnALittle()
    {
        return _builder.CreateSimple()
            .WithName("Zone2Enter - Turn on a little")
            .WithTriggers(Sensors.KitchenZone2AllCount)
            .WithExecution(async (sc,ct) => {
                if (_solarMeter.State < 1100)
                {
                    if (_kitchenLights.IsOff())
                    {
                        await _services.Api.LightSetBrightness(Lights.KitchenLights, Bytes._5pct);
                    }                
                }
            })
            .Build();
    }

    IAutomation Zone1Enter_TurnOn()
    {
        return _builder.CreateSimple()
            .WithName("Turn On Kithen Lights")
            .WithDescription("If ambient light is low, turn on kitchen lights")
            .WithTriggers(Sensors.KitchenZone1AllCount)
            .WithExecution(async (sc, ct) => {
                if (_solarMeter.State < 1100)
                {
                    var lightStatus = await _services.EntityProvider.GetLightEntity(Lights.KitchenLights);
                    if (lightStatus.IsOff() || lightStatus!.Attributes!.Brightness < Bytes.PercentToByte(20))
                    {
                        await _services.Api.LightSetBrightness(Lights.KitchenLights, Bytes._20pct);
                    }
                }
            })
            .Build();
    }

    ISchedulableAutomation NoOccupancy_for5min_TurnOff()
    {
        var minutesToLeaveOn = 5;

        return _builder.CreateSchedulable()
            .WithName("Turn Off Kitchen Lights")
            .WithDescription($"Turn off the kitchen lights when unoccupied for {minutesToLeaveOn} minutes")
            .MakeDurable()
            .WithTriggers(Sensors.KitchenPresence)
            .While(sc => sc.ToOnOff().IsOff() && _kitchenLights.IsOn())
            .For(TimeSpan.FromMinutes(minutesToLeaveOn))
            .WithExecution(ct => {
                return _services.Api.TurnOff(Lights.KitchenLights);
            })
            .Build();
    }
}
