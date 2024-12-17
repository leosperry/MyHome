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

        _kitchenLights = helpers.UpdatingEntityProvider.GetLightEntity(Light.KitchenLights);
        _solarMeter = helpers.UpdatingEntityProvider.GetFloatEntity(Sensor.SolaredgeCurrentPower);
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
            .WithTriggers(Input_Number.AudibleAlertToPlay)
            .WithName("Replay notification")
            .WithExecution(async (sc, ct) => {
                if (sc.BecameGreaterThan(0))
                {
                    var msgToPlay = (await _services.EntityProvider.GetEntity($"input_text.audible_alert_{sc.New.State}"))?.State;
                    if (msgToPlay is null)
                    {
                        await _services.Api.SpeakPiper(Media_Player.DiningRoomSpeaker, "could not retrieve message", true);
                    }
                    else
                    {
                        await _services.Api.SpeakPiper(Media_Player.DiningRoomSpeaker, msgToPlay);
                    }
                    await _services.Api.InputNumberSet(Input_Number.AudibleAlertToPlay, 0);
                }
            })
            .Build();
    }

    IAutomationBase Zone2Enter_TurnOnALittle()
    {
        return _builder.CreateSimple<int>()
            .WithName("Zone2Enter - Turn on a little")
            .WithTriggers(Sensor.EsphomekitchenmotionZone2AllTargetCount) //sensor.esphomekitchenmotion_zone_2_all_target_count
            .WithExecution(async (sc,ct) => {
                if (sc.BecameGreaterThan(0) && _solarMeter.State < 1100)
                {
                    // sometimes zone 1 turns on light, but _kitchenLights is not updated yet. 
                    // so go manually get the latest state
                    var kitchResponse = await _services.Api.GetEntity<HaEntityState<OnOff, JsonElement>>(Light.KitchenLights);
                    kitchResponse.response.EnsureSuccessStatusCode();

                    if (kitchResponse.entityState.IsOff())
                    {
                        await _services.Api.LightSetBrightness(Light.KitchenLights, Bytes._5pct);
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
            .WithTriggers(Sensor.EsphomekitchenmotionZone1AllTargetCount)
            .WithExecution(async (sc, ct) => {
                if (sc.BecameGreaterThan(0) && _solarMeter.State < 1100)
                {
                    var kitchResponse = await _services.Api.GetEntity<HaEntityState<OnOff, LightModel>>(Light.KitchenLights);
                    kitchResponse.response.EnsureSuccessStatusCode();
                    
                    if (kitchResponse.entityState.IsOff() || kitchResponse.entityState?.Attributes?.Brightness < Bytes.PercentToByte(20))
                    {
                        await _services.Api.LightSetBrightness(Light.KitchenLights, Bytes._20pct);
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
            .WithTriggers( Sensor.Livingroomandkitchenpresencecount) //livingroomandkitchenpresencecount
            .While(sc => sc.IsOff() && _kitchenLights.IsOn())
            .For(TimeSpan.FromMinutes(minutesToLeaveOn))
            .WithExecution(ct => {
                return _services.Api.TurnOff(Light.KitchenLights);
            })
            .Build();
    }
}
