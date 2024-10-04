using HaKafkaNet;
using MyHome.Areas.Office;
using MyHome.Services;

namespace MyHome;

    /// <summary>
    /// Lights in my office are dynamically adjusted based on the amount of ambient light
    /// The lights are trigger by a motion sensor which has an integrated luminocity sensor
    /// which unfortunately picks up light from the office lights themselves.
    /// This automation uses a DynamicLightAdjuster object which takes into account 
    /// the brightness from the lights themselves
    /// </summary>
[ExcludeFromDiscovery]
public class OfficeMotion : IAutomation, IAutomationMeta
{
    //public EventTiming EventTimings { get => EventTiming.PostStartup; }
    
    public const string OFFICE_ILLUMINANCE = "sensor.lumi_lumi_sensor_motion_aq2_illuminance";
    private readonly IHaApiProvider _api;
    private readonly IHaStateCache _cache;

    //private readonly IDynamicLightAdjuster _lightAdjuster;
    private readonly OfficeService _officeService;
    private readonly ILogger<OfficeMotion> _logger;


    //private CombinedLight _officeLightsCombined;

    public OfficeMotion(Func<IDynamicLightAdjuster.DynamicLightModel, IDynamicLightAdjuster> lightAdjusterFactory
        , IHaApiProvider api, IHaStateCache cache, OfficeService officeService, ILogger<OfficeMotion> logger)
    {
        _api = api;
        _cache = cache;
        _officeService = officeService;
        _logger = logger;

        // _officeLightsCombined = new CombinedLight(_api, 
        //     new CombinedLightModel(Lights.OfficeLights, Bytes.PercentToByte(1), Bytes.PercentToByte(17)),
        //     new CombinedLightModel(Lights.OfficeLightBars, Bytes.PercentToByte(1), Bytes._100pct));

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
        // _lightAdjuster = lightAdjusterFactory(new IDynamicLightAdjuster.DynamicLightModel(){
        //     //MinIllumination = 7,
        //     TargetIllumination = 110, 
        //     MinBrightness = Bytes.PercentToByte(1),
        //     MaxLightBrightness = Bytes._100pct, 
        //     IlluminationAddedAtMin = 3,
        //     IlluminationAddedAtMax = 42
        // });
        
    }

    public IEnumerable<string> TriggerEntityIds()
    {
        yield return OFFICE_ILLUMINANCE;
        yield return Sensors.OfficeMotion;
        yield return Helpers.OfficeOverride;
        yield return Helpers.OfficeIlluminanceThreshold;
    }
    
    public async Task Execute(HaEntityStateChange stateChange, CancellationToken cancellationToken)
    {

        await _officeService.SetLights(stateChange.EntityId == Sensors.OfficeIlluminance, cancellationToken);
    }


    AutomationMetaData? _meta;
    public AutomationMetaData GetMetaData() => 
        _meta ??= new AutomationMetaData()
        {
            Name = "Office lights",
            Description = "Dynamically set office light brightness based on luminance and take into account luminance from lights themselves",
        };

}
