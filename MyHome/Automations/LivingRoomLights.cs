using System.Text.Json;
using HaKafkaNet;

namespace MyHome;

/// <summary>
/// Uses the solar pannels on the roof as a light meter
/// When the sun is getting low or on exceptional cloudy day
///     gradually turn on the lights in the living room
///     using a parabolic curve so that it is not jarring
/// </summary>
public class LivingRoomLights : IAutomation, IAutomationMeta
{
    private readonly IHaApiProvider _api;
    private readonly IHaEntityProvider _entityProvider;
    public const string TRIGGER = "sensor.solaredge_current_power";
    const double THRESHOLD = 700;

    public LivingRoomLights(IHaApiProvider api, IHaEntityProvider entityProvider)
    {
        this._api = api;
        this._entityProvider = entityProvider;
    }

    public async Task Execute(HaEntityStateChange stateChange, CancellationToken ct)
    {
        var powerState = stateChange.ToDoubleTyped();
        var currentPower = powerState.New.State;

        if (currentPower is null)
        {
            //can't calculate; do nothing
            return;
        }

        Task<SunModel?> sunTask = null!;
        Task<HaEntityState<OnOff, JsonElement>?> overrideTask = null!;
        await Task.WhenAll(
            sunTask = _entityProvider.GetSun(),//.GetEntityState<SunAttributes>("sun.sun"),
            overrideTask = _entityProvider.GetOnOffEntity(Helpers.LivingRoomOverride));

        // only run when the sun is up and the override is off
        if (sunTask.Result?.Attributes?.Azimuth > -6 && overrideTask.Result?.State == OnOff.Off)
        {
            if (currentPower > THRESHOLD)
            {
                // turn off when we have plenty of light
                await _api.TurnOff([Lights.TvBacklight, Lights.CounchOverhead]);
            }
            else
            {
                // crazy calc time
                // get the difference
                // divide by threshold to get decimal percentage
                // raise to power of 2 for parabolic
                // set maximum for each light
                // convert to byte
                var unmodifiedValue = Math.Pow((THRESHOLD - currentPower.Value)/ THRESHOLD, 2) * 255;
                await Task.WhenAll(
                    _api.LightTurnOn(new LightTurnOnModel()
                    {
                        EntityId = [Lights.TvBacklight],
                        Brightness = (byte)Math.Round(unmodifiedValue * 0.6),
                        Kelvin = 2202,
                    },ct),
                    _api.LightTurnOn(new LightTurnOnModel()
                    {
                        EntityId = [Lights.CounchOverhead],
                        Brightness = (byte)Math.Round(unmodifiedValue * 0.25),
                        RgbColor = (255, 146, 39),
                    }, ct)
                );
            }
        }
    }

    public IEnumerable<string> TriggerEntityIds()
    {
        yield return TRIGGER;
    }

    public AutomationMetaData GetMetaData()
    {
        return new AutomationMetaData()
        {
            Name = "Living Room Lights",
            Description = "set lights based on solar power generation",
            AdditionalEntitiesToTrack = [
                Lights.Couch1, Lights.Couch2, Lights.Couch3, 
                Lights.LivingLamp1, Lights.LivingLamp2
            ]
        };
    }
}
