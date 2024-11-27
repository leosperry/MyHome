using System.Text.Json;
using HaKafkaNet;

namespace MyHome;


public class KitchenRegistry : IAutomationRegistry
{
    readonly IHaServices _services;
    readonly IAutomationBuilder _builder;
    private readonly ILogger<KitchenRegistry> _logger;
    private readonly IHaEntity<OnOff, LightModel> _kitchenLights;
    private readonly IHaEntity<float?, JsonElement> _solarMeter;

    public KitchenRegistry(IHaServices services, IStartupHelpers helpers, ILogger<KitchenRegistry> logger)
    {
        _services = services;
        _builder = helpers.Builder;
        this._logger = logger;

        _kitchenLights = helpers.UpdatingEntityProvider.GetLightEntity(Lights.KitchenLights);
        _solarMeter = helpers.UpdatingEntityProvider.GetFloatEntity(Devices.SolarPower);
    }

    public void Register(IRegistrar reg)
    {
        var exceptions = reg.TryRegister(
            Zone1Enter_TurnOn,
            Zone2Enter_TurnOnALittle,
            NoOccupancy_for5min_TurnOff,
            ReplayNotification
        );
    }

    IAutomationBase ReplayNotification()
    {
        return _builder.CreateSimple<float>()
            .WithTriggers(Helpers.AudibleAlertToPlay)
            .WithName("Replay notification")
            .WithExecution(async (sc, ct) => {
                if (sc.BecameGreaterThan(0))
                {
                    var msgToPlay = (await _services.EntityProvider.GetEntity($"input_text.audible_alert_{sc.New.State}"))?.State;
                    if (msgToPlay is null)
                    {
                        await _services.Api.SpeakPiper(MediaPlayers.DiningRoom, "could not retrieve message", true);
                    }
                    else
                    {
                        await _services.Api.SpeakPiper(MediaPlayers.DiningRoom, msgToPlay);
                    }
                    await _services.Api.InputNumberSet(Helpers.AudibleAlertToPlay, 0);
                }
            })
            .Build();
    }

    IAutomationBase Zone2Enter_TurnOnALittle()
    {
        return _builder.CreateSimple<int>()
            .WithName("Zone2Enter - Turn on a little")
            .WithTriggers(Sensors.KitchenZone2AllCount)
            .WithExecution(async (sc,ct) => {
                if (sc.BecameGreaterThan(0) && _solarMeter.State < 1100)
                {
                    // sometimes zone 1 turns on light, but _kitchenLights is not updated yet. 
                    // so go manually get the latest state
                    var kitchResponse = await _services.Api.GetEntity<HaEntityState<OnOff, JsonElement>>(Lights.KitchenLights);
                    kitchResponse.response.EnsureSuccessStatusCode();

                    if (kitchResponse.entityState.IsOff())
                    {
                        await _services.Api.LightSetBrightness(Lights.KitchenLights, Bytes._5pct);
                    }                
                }
            })
            .Build();
    }

    IAutomationBase Zone1Enter_TurnOn()
    {
        return _builder.CreateSimple<int>()
            .WithName("Turn On Kithen Lights")
            .WithDescription("If ambient light is low, turn on kitchen lights")
            .WithTriggers(Sensors.KitchenZone1AllCount)
            .WithExecution(async (sc, ct) => {
                if (sc.BecameGreaterThan(0) && _solarMeter.State < 1100)
                {
                    if (_kitchenLights.IsOff() || _kitchenLights.Attributes?.Brightness < Bytes.PercentToByte(20))
                    {
                        await _services.Api.LightSetBrightness(Lights.KitchenLights, Bytes._20pct);
                    }
                }
            })
            .Build();
    }

    IAutomationBase NoOccupancy_for5min_TurnOff()
    {
        var minutesToLeaveOn = 5;

        return _builder.CreateSchedulable<OnOff>()
            .WithName("Turn Off Kitchen Lights")
            .WithDescription($"Turn off the kitchen lights when unoccupied for {minutesToLeaveOn} minutes")
            .MakeDurable()
            .WithTriggers(Sensors.KitchenPresence)
            .While(sc => sc.IsOff() && _kitchenLights.IsOn())
            .For(TimeSpan.FromMinutes(minutesToLeaveOn))
            .WithExecution(ct => {
                return _services.Api.TurnOff(Lights.KitchenLights);
            })
            .Build();
    }
}
