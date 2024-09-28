﻿using HaKafkaNet;
using MyHome.Services;

namespace MyHome;

    /// <summary>
    /// Lights in my office are dynamically adjusted based on the amount of ambient light
    /// The lights are trigger by a motion sensor which has an integrated luminocity sensor
    /// which unfortunately picks up light from the office lights themselves.
    /// This automation uses a DynamicLightAdjuster object which takes into account 
    /// the brightness from the lights themselves
    /// </summary>
public class OfficeMotion : IAutomation, IAutomationMeta
{
    public EventTiming EventTimings { get => EventTiming.PostStartup |  EventTiming.PreStartupSameAsLastCached; }
    
    public const string OFFICE_ILLUMINANCE = "sensor.lumi_lumi_sensor_motion_aq2_illuminance";
    private readonly IHaApiProvider _api;
    private readonly IHaStateCache _cache;

    private readonly IDynamicLightAdjuster _lightAdjuster;
    private readonly ILogger<OfficeMotion> _logger;

    private CombinedLight _officeLightsCombined;

    public OfficeMotion(Func<IDynamicLightAdjuster.DynamicLightModel, IDynamicLightAdjuster> lightAdjusterFactory
        , IHaApiProvider api, IHaStateCache cache, ILogger<OfficeMotion> logger)
    {
        _api = api;
        _cache = cache;
        _logger = logger;

        _officeLightsCombined = new CombinedLight(_api, 
            new CombinedLightModel(Lights.OfficeLights, Bytes.PercentToByte(1), Bytes.PercentToByte(17)),
            new CombinedLightModel(Lights.OfficeLightBars, Bytes.PercentToByte(1), Bytes._100pct));

        //old
        // _lightAdjuster = lightAdjusterFactory(new IDynamicLightAdjuster.DynamicLightModel(){
        //     //MinIllumination = 7,
        //     TargetIllumination = 60, 
        //     MinBrightness = Bytes.PercentToByte(1), //3
        //     MaxLightBrightness = Bytes.PercentToByte(16), // 40
        //     IlluminationAddedAtMin = 2,
        //     IlluminationAddedAtMax = 41 // used to be 31 ???? is now 15
        // });
        // new light bars min light 1
        // new light bars max light 21

        // combined illu at min 3
        // combined illu at max 42

        //new 
        _lightAdjuster = lightAdjusterFactory(new IDynamicLightAdjuster.DynamicLightModel(){
            //MinIllumination = 7,
            TargetIllumination = 110, 
            MinBrightness = Bytes.PercentToByte(1),
            MaxLightBrightness = Bytes._100pct, 
            IlluminationAddedAtMin = 3,
            IlluminationAddedAtMax = 42
        });
        
    }

    public IEnumerable<string> TriggerEntityIds()
    {
        yield return OFFICE_ILLUMINANCE;
        yield return Sensors.OfficeMotion;
        yield return Helpers.OfficeOverride;
    }
    
    public async Task Execute(HaEntityStateChange stateChange, CancellationToken cancellationToken)
    {
        if (stateChange.EntityId == Helpers.OfficeOverride && stateChange.ToOnOff().IsOn())
        {
            return;
        }

        var officeMotion = await _cache.GetOnOffEntity(Sensors.OfficeMotion);
        if (officeMotion?.State == OnOff.Off)
        {
            return;
        }

        var officeOverride = await _cache.GetOnOffEntity(Helpers.OfficeOverride, cancellationToken);
        if (officeOverride?.State == OnOff.On)
        {
            return;
        }

        var currentIlluminationEntity = await _cache.GetIntegerEntity(OFFICE_ILLUMINANCE);
        var currentIllumination = currentIlluminationEntity?.State;
        if (currentIllumination is not null)
        {
            await SetBrightness(currentIllumination.Value, cancellationToken);
        }
        else
        {
            _logger.LogWarning("could not get office illumination");
        }
    }

    private async Task SetBrightness(int currentIllumination, CancellationToken cancellationToken)
    {
        //var officeLights = await _cache.GetLightEntity(Lights.OfficeLights);

        var oldBrightness = _officeLightsCombined.LatestBrightness; //officeLights?.Attributes?.Brightness ?? 0;

        var newBrightness = (byte)Math.Round(_lightAdjuster.GetAppropriateBrightness(currentIllumination, oldBrightness));

        //await _api.LightSetBrightness(Lights.OfficeLights, newBrightness, cancellationToken);
        await _officeLightsCombined.Set(newBrightness);
    }

    AutomationMetaData? _meta;
    public AutomationMetaData GetMetaData() => 
        _meta ??= new AutomationMetaData()
        {
            Name = "Office lights",
            Description = "Dynamically set office light brightness based on luminance and take into account luminance from lights themselves",
        };

}
