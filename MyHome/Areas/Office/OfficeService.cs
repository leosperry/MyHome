using System;
using System.Text.Json;
using HaKafkaNet;
using MyHome.Services;

namespace MyHome.Areas.Office;

public class OfficeService
{
    public const string OFFICE_ILLUMINANCE = "sensor.lumi_lumi_sensor_motion_aq2_illuminance";
    private readonly IHaApiProvider _api;

    private readonly IDynamicLightAdjuster _lightAdjuster;
    private readonly ILogger<OfficeService> _logger;
    private readonly IEntityStateProvider _entityProvider;
    private CombinedLight _officeLightsCombined;

    // IHaEntity<OnOff, LightModel> _officeLights;
    // IHaEntity<OnOff, ColorLightModel> _officeLightBars;
    IHaEntity<int?, JsonElement> _officeIlluminance;
    IHaEntity<OnOff, JsonElement> _officeMotion;
    IHaEntity<float?, JsonElement> _threshold;
    IHaEntity<OnOff, JsonElement> _override;
    IHaEntity<SunState, SunAttributes> _sun;
    private IDynamicLightAdjuster.DynamicLightModel _dynamicModel;

    private byte _previousBrightness = 127; // set it to something in the middle, init will set it otherwhise.

    public OfficeService(Func<IDynamicLightAdjuster.DynamicLightModel, IDynamicLightAdjuster> lightAdjusterFactory
        , IHaApiProvider api, IUpdatingEntityProvider updatingEntityProvider, IHaEntityProvider entityProvider, ILogger<OfficeService> logger)
    {
        _api = api;
        _logger = logger;
        _entityProvider = entityProvider;

        _dynamicModel = new IDynamicLightAdjuster.DynamicLightModel(){
            //MinIllumination = 7,
            TargetIllumination = 50, 
            MinBrightness = Bytes.PercentToByte(1),
            MaxLightBrightness = Bytes._100pct, 
            IlluminationAddedAtMin = 4,
            IlluminationAddedAtMax = 50
        };

        _lightAdjuster = lightAdjusterFactory(_dynamicModel);

        _officeIlluminance = updatingEntityProvider.GetIntegerEntity(Sensors.OfficeIlluminance);
        _threshold = updatingEntityProvider.GetFloatEntity(Helpers.OfficeIlluminanceThreshold);
        _override = updatingEntityProvider.GetOnOffEntity(Helpers.OfficeOverride);
        _officeMotion = updatingEntityProvider.GetOnOffEntity(Sensors.OfficeMotion);
        _sun = updatingEntityProvider.GetSun();

        _officeLightsCombined = new CombinedLight(_api, _logger,
            new CombinedLightModel(Lights.OfficeLights, -40, Bytes.PercentToByte(20), MinOn: 22),
            new CombinedLightModel(Lights.OfficeLightBars, Bytes.PercentToByte(20), Bytes._100pct, true));
    }

    internal void Init(byte previousBrightness)
    {
        _previousBrightness = previousBrightness;
    }

    internal async Task TurnOff(CancellationToken ct = default)
    {
        await _officeLightsCombined.TurnOff(ct);
        await _api.TurnOff([Devices.OfficeFan, Lights.OfficeLeds], ct);
        await _api.TurnOffByLabel(Labels.OfficeDevices, ct);
    }

    /// <summary>
    /// called at top and bottom of hour
    /// </summary>
    /// <returns></returns>
    internal async Task PeriodicTasks()
    {
        // adjust target threshold if needed
        await adjustTargetThresholdBasedOnSun();
        await checkForOutsideAdjustments();
    }

    public async Task SetLights(bool triggeredByIlluminance, CancellationToken ct = default)
    {
        if (triggeredByIlluminance)
        {
            _previousBrightness = _officeLightsCombined.LatestBrightness;
        }
        _dynamicModel.TargetIllumination = (int)(_threshold.State ?? 60f);
        if (_override.State == OnOff.On)
        {
            return;
        }

        if (_officeMotion.Bad() || _officeIlluminance.Bad())
        {
            _logger.LogWarning("could not get office state. Motion:{motion_state} Illuminance:{office_illuminance}", _officeMotion, _officeIlluminance);
            return;
        }

        if (_officeMotion.State == OnOff.On || DateTime.Now - _officeMotion.LastChanged < TimeSpan.FromMinutes(10))
        {
            await SetBrightness(triggeredByIlluminance, _officeIlluminance.State!.Value, ct);
            // we'll turn them off in another automation
        }
    }

    private async Task SetBrightness(bool triggeredByIlluminance, int currentIllumination, CancellationToken cancellationToken)
    {
        byte oldBrightness = triggeredByIlluminance ? _officeLightsCombined.LatestBrightness : _previousBrightness;

        var newBrightness = (byte)Math.Round(_lightAdjuster.GetAppropriateBrightness(currentIllumination, oldBrightness));

        await _officeLightsCombined.Set(newBrightness, GetKelvin(), cancellationToken);
        _logger.LogInformation("Set Brightness {values}", new SetBrightnessLog(triggeredByIlluminance, currentIllumination, oldBrightness, newBrightness));
        await _api.InputNumberSet("input_number.office_brightness_tracker", newBrightness);
    }

    private record SetBrightnessLog(bool trigger, int currentIllum, int oldBrightness, int newBrightness);

    private async Task adjustTargetThresholdBasedOnSun()
    {
        const float smallestThreshold = 15f;
        const float maxThreshold = 120f;

        var sunState = _sun.State;
        var threshold = _threshold.State ?? 60;

        if (sunState == SunState.Below_Horizon && _threshold.State > smallestThreshold)
        {
            var timeSinceMotion = _officeMotion.State == OnOff.On
                ? TimeSpan.Zero
                : DateTime.Now - _officeMotion.LastChanged;
            if (timeSinceMotion > TimeSpan.FromHours(4))
            {
                await _api.InputNumberSet(Helpers.OfficeIlluminanceThreshold, smallestThreshold);
            }
        }
        else if (sunState == SunState.Above_Horizon && threshold < maxThreshold && _override.State == OnOff.Off)
        {
            var currentBrightness = _officeLightsCombined.LatestBrightness;

            if (_officeIlluminance.Bad())
            {
                _logger.LogWarning("cannot calculate actual illumination. Sensor in bad state");
                return;
            }

            var sunIllumination = _lightAdjuster.GetActualIllumination(_officeIlluminance.State!.Value ,currentBrightness);

            _logger.LogInformation("attempbting adjusting for sun: {currentBrightness},{sunIllumination}", currentBrightness, sunIllumination);

            if(sunIllumination >= threshold)
            {
                _logger.LogInformation("adjusting office lights target threshold. Was:{was}, Now:{now}", threshold, sunIllumination);
                await _api.InputNumberSet(Helpers.OfficeIlluminanceThreshold, Math.Min(maxThreshold, (int)sunIllumination));
            }
        }
    }

    private async Task checkForOutsideAdjustments()
    {
        var lightBarStat = await _entityProvider.GetColorLightEntity(Lights.OfficeLightBars);
        if (lightBarStat.Bad()|| lightBarStat.Attributes is null)
        {
            _logger.LogWarning("something is wrong with the light bars");
            return;
        }

        if (lightBarStat.IsOff())
        {
            await _officeLightsCombined.TurnOff();
        }
    }

    private int GetKelvin()
    {
        //  warmest  2000
        //  coolest 9000

        var now = DateTime.Now.TimeOfDay.TotalSeconds;

        const int coolest = 8000;
        const int warmest = 3000;
        const float coolWarmDiff = coolest - warmest;

        const int secondsInHour = 3600;
        const int fourAM = 4 * secondsInHour;
        const int tenAM = 10 * secondsInHour;
        const int noon = 12 * secondsInHour;
        const int sevenPM = 19 * secondsInHour;

        const float coolingRate = coolWarmDiff / (tenAM - fourAM);
        const float warmingRate = coolWarmDiff / (7 /*hours*/ * secondsInHour);

        if(now < fourAM)
        {
            return warmest;
            // still warming
            // how long before 4 am?
        }
        else if(now < tenAM)
        {
            // start cooling
            // how long after 4 am?
            var diff = now - fourAM;
            return  warmest + (int)(diff * coolingRate);
        }
        else if (now < noon)
        {
            return coolest;   
        }
        else if(now < sevenPM)
        {
            // start warming
            // how long after noon?
            var diff = now - noon;
            return coolest - (int)(diff * warmingRate);
        }
        else
        {
            return warmest;
        }
    }
}
