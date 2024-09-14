using HaKafkaNet;

namespace MyHome;


public class KitchenRegistry : IAutomationRegistry
{
    readonly IHaServices _services;
    readonly IAutomationBuilder _builder;

    public KitchenRegistry(IHaServices services, IAutomationBuilder builder)
    {
        _services = services;
        _builder = builder;
    }

    public void Register(IRegistrar reg)
    {
        reg.RegisterMultiple(
            Zone1Enter_TurnOn(), 
            Zone2Enter_TurnOnALittle()
        );

        reg.RegisterMultiple(
            NoOccupancy_for5min_TurnOff()
        );
    }

    IAutomation Zone2Enter_TurnOnALittle()
    {
        return _builder.CreateSimple()
            .WithName("Zone2Enter - Turn on a little")
            .WithTriggers(Sensors.KitchenZone2AllCount)
            .WithExecution(async (sc,ct) => {
                var solaredge_current_power = await _services.EntityProvider.GetFloatEntity(Devices.SolarPower);

                if (solaredge_current_power?.State < 1100)
                {
                    var kitchenLightStatus = await _services.EntityProvider.GetOnOffEntity(Lights.KitchenLights);

                    if (kitchenLightStatus.IsOff())
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
                var solaredge_current_power = await _services.EntityProvider.GetFloatEntity(Devices.SolarPower);

                if (solaredge_current_power?.State < 1100)
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
    }
}
