﻿using System.Text.Json;
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
        Task<SunModel?> sunTask = null!;
        Task<HaEntityState<OnOff, JsonElement>?> overrideTask = null!;
        await Task.WhenAll(
            sunTask = _entityProvider.GetSun(),//.GetEntityState<SunAttributes>("sun.sun"),
            overrideTask = _entityProvider.GetOnOffEntity(OVERRIDE));

        // only run when the sun is up and the override is off
        if (sunTask.Result?.Attributes?.Azimuth > -6 && overrideTask.Result?.State == OnOff.Off
            && float.TryParse(stateChange.New.State, out var currentPower))
        {
            if (currentPower > THRESHOLD)
            {
                // turn off when we have plenty of light
                await _api.TurnOff([TV_BACKLIGHT, COUCH_OVERHEAD]);
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
                    _api.LightTurnOn(new LightTurnOnModel()
                    {
                        EntityId = [TV_BACKLIGHT],
                        Brightness = (byte)Math.Round(unmodifiedValue * 0.6),
                        Kelvin = 2202,
                    },cancellationToken),
                    _api.LightTurnOn(new LightTurnOnModel()
                    {
                        EntityId = [COUCH_OVERHEAD],
                        Brightness = (byte)Math.Round(unmodifiedValue * 0.25),
                        RgbColor = (255, 146, 39),
                    }, cancellationToken)
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
                "light.wiz_rgbw_tunable_79a59c", "light.wiz_rgbw_tunable_79aab4", "light.wiz_rgbw_tunable_79aab4", "light.living_lamp_1", "light.living_lamp_2"
            ]
        };
    }
}
