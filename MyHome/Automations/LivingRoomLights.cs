using HaKafkaNet;

namespace MyHome;

/// <summary>
/// Uses the solar pannels on the roof as a light meter
/// When the sun is getting low or on exceptional cloudy day
///     gradually turn on the lights in the living room
///     using a parabolic curve so that it is not jarring
/// </summary>
public class LivingRoomLights : IAutomation
{
    private readonly IHaApiProvider _api;
    private readonly IHaEntityProvider _entityProvider;
    public const string TRIGGER = "sensor.solaredge_current_power";
    public const string OVERRIDE = "input_boolean.living_room_override";
    public const string COUCH_OVERHEAD = "light.couch_overhead";
    const string TV_BACKLIGHT = "light.tv_backlight";
    const double THRESHOLD = 700;

    public LivingRoomLights(IHaApiProvider api, IHaEntityProvider entityProvider)
    {
        this._api = api;
        this._entityProvider = entityProvider;
    }

    public async Task Execute(HaEntityStateChange stateChange, CancellationToken cancellationToken)
    {
        Task<HaEntityState<SunAttributes>?> sunTask = null!;
        Task<HaEntityState?> overrideTask = null!;
        await Task.WhenAll(
            sunTask = _entityProvider.GetEntityState<SunAttributes>("sun.sun"),
            overrideTask = _entityProvider.GetEntityState(OVERRIDE));

        // only run when the sun is up and the override is off
        if (sunTask.Result?.Attributes?.Azimuth > -6 && overrideTask.Result?.State == "off"
            && float.TryParse(stateChange.New.State, out var currentPower))
        {
            if (currentPower > THRESHOLD)
            {
                // turn off when we have plenty of light
                await _api.LightTurnOff([TV_BACKLIGHT, COUCH_OVERHEAD]);
            }
            else
            {
                // crazy calc time
                // get the difference
                // divide by threshold to get decimal percentage
                // raise to power of 2 for parabolic
                // set maximum for each light
                // convert to byte
                var unmodifiedValue = Math.Pow((THRESHOLD - currentPower)/ THRESHOLD, 2) * 255;
                await Task.WhenAll(
                    _api.LightSetBrightness(TV_BACKLIGHT, (byte)Math.Round(unmodifiedValue * 0.6)),
                    _api.LightSetBrightness(COUCH_OVERHEAD, (byte)Math.Round(unmodifiedValue * 0.25))
                );
            }
        }
    }

    public IEnumerable<string> TriggerEntityIds()
    {
        yield return TRIGGER;
    }
}
