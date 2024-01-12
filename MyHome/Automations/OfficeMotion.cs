using HaKafkaNet;

namespace MyHome;

public class OfficeMotion : IAutomation
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
        var officeMotion = await _cache.Get(OFFICE_MOTION);
        if (officeMotion?.State == "off")
        {
            //turn off the lights
            // need to pick a pattern to wait some more time
            //await _api.LightTurnOff(OFFICE_LIGHTS);
            return;
        }

        var officeOverride = await _cache.Get(OFFICE_OVERRIDE, cancellationToken);
        if (officeOverride?.State == "on")
        {
            return;
        }

        var currentIlluminationEntity = await _cache.Get(OFFICE_ILLUMINANCE);
        int currentIllumination;
        if (currentIlluminationEntity is not null && int.TryParse(currentIlluminationEntity.State, out currentIllumination))
        {
            await SetBrightness(currentIllumination, cancellationToken);
        }
        else
        {
            _logger.LogWarning("could not get office illumination");
        }
    }

    private async Task SetBrightness(int currentIllumination, CancellationToken cancellationToken)
    {
        _logger.LogInformation("setting office brightness");
        var officeLights = await _cache.Get<SimpleLightModel>(OFFICE_LIGHTS);

        var oldBrightness = officeLights!.Attributes!.Brightness ?? 0;

        var newBrightness = (byte)Math.Round(_lightAdjuster.GetAppropriateBrightness(currentIllumination, oldBrightness));

        await _api.LightSetBrightness(OFFICE_LIGHTS, newBrightness, cancellationToken);
    }
}
