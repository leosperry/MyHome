using HaKafkaNet;

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
    public const string OFFICE_MOTION = "binary_sensor.lumi_lumi_sensor_motion_aq2_motion";
    public const string OFFICE_ILLUMINANCE = "sensor.lumi_lumi_sensor_motion_aq2_illuminance";
    public const string OFFICE_OVERRIDE = "input_boolean.office_override";
    public const string OFFICE_LIGHTS = "light.office_lights";
    private readonly IHaApiProvider _api;
    private readonly IHaStateCache _cache;

    private readonly IDynamicLightAdjuster _lightAdjuster;
    private readonly ILogger<OfficeMotion> _logger;

    public OfficeMotion(Func<IDynamicLightAdjuster.DynamicLightModel, IDynamicLightAdjuster> lightAdjusterFactory
        , IHaApiProvider api, IHaStateCache cache, ILogger<OfficeMotion> logger)
    {
        _api = api;
        _cache = cache;
        _logger = logger;

        _lightAdjuster = lightAdjusterFactory(new IDynamicLightAdjuster.DynamicLightModel(){
            //MinIllumination = 7,
            TargetIllumination = 100,
            MinBrightness = 3,
            MaxLightBrightness = 40,
            IlluminationAddedAtMin = 2,
            IlluminationAddedAtMax = 31
        });
    }

    public IEnumerable<string> TriggerEntityIds()
    {
        yield return OFFICE_ILLUMINANCE;
        yield return OFFICE_MOTION;
    }

    
    public async Task Execute(HaEntityStateChange stateChange, CancellationToken cancellationToken)
    {
        var officeMotion = await _cache.GetEntity(OFFICE_MOTION);
        if (officeMotion?.State == "off")
        {
            return;
        }

        var officeOverride = await _cache.GetEntity(OFFICE_OVERRIDE, cancellationToken);
        if (officeOverride?.State == "on")
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
        var officeLights = await _cache.GetOnOffEntity<SimpleLightModel>(OFFICE_LIGHTS);

        var oldBrightness = officeLights?.Attributes?.Brightness ?? 0;

        var newBrightness = (byte)Math.Round(_lightAdjuster.GetAppropriateBrightness(currentIllumination, oldBrightness));

        await _api.LightSetBrightness(OFFICE_LIGHTS, newBrightness, cancellationToken);
    }

    AutomationMetaData? _meta;
    public AutomationMetaData GetMetaData() => 
        _meta ??= new AutomationMetaData()
        {
            Name = "Office lights",
            Description = "Dynamically set office light brightness based on luminance and take into account luminance from lights themselves",
        };

}
