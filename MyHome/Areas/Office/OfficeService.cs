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
    private readonly ILogger<OfficeMotion> _logger;
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

    private byte _previous = 127; // set it to something in the middle

    public OfficeService(Func<IDynamicLightAdjuster.DynamicLightModel, IDynamicLightAdjuster> lightAdjusterFactory
        , IHaApiProvider api, IUpdatingEntityProvider updatingEntityProvider, IHaEntityProvider entityProvider, ILogger<OfficeMotion> logger)
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
            new CombinedLightModel(Lights.OfficeLights, -40, Bytes.PercentToByte(17)),
            new CombinedLightModel(Lights.OfficeLightBars, Bytes.PercentToByte(20), Bytes._100pct, primary:true));
    }

    internal async Task TurnOff(CancellationToken ct)
    {
        
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
            _previous = _officeLightsCombined.LatestBrightness;
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
        byte oldBrightness = triggeredByIlluminance ? _officeLightsCombined.LatestBrightness : _previous;

        var newBrightness = (byte)Math.Round(_lightAdjuster.GetAppropriateBrightness(currentIllumination, oldBrightness));

        await _officeLightsCombined.Set(newBrightness, cancellationToken);
        _logger.LogInformation("Set Brightness {values}", new SetBrightnessLog(triggeredByIlluminance, currentIllumination, oldBrightness, newBrightness));
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
            var sunIllumination = _lightAdjuster.GetIlluminationFromBrightness(currentBrightness);
            if(sunIllumination > threshold)
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

        _officeLightsCombined.RecordBrightnessFromObservation(lightBarStat.Attributes.Brightness);
    }

    private int GetKelvin()
    {
        //between 2000 and 9000
        // warm to cool
        return 6000; // not too cool

        //want 
        //  4 am warmest  2000
        //  10 am coolest 9000
    }
}
