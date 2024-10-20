namespace MyHome;

public interface IDynamicLightAdjuster
{
    double GetAppropriateBrightness(double illumination, double currentBrightness);
    //double GetIlluminationFromBrightness(double currentBrightness);
    double GetActualIllumination(double illumination, double currentBrightness);

    /// <summary>
    /// Model required for logic
    /// </summary>
    public class DynamicLightModel
    {
        /// <summary>
        /// The target which the algoritm should try to hit
        /// </summary>
        public int TargetIllumination { get; set; }
        /// <summary>
        /// When turning on the light, what is the minimum brighness needed to activate the lights
        /// </summary>
        public byte MinBrightness { get; set; }
        /// <summary>
        /// The maximum brightness that lights should be set to
        /// </summary>
        public byte MaxLightBrightness { get; set; }
        /// <summary>
        /// The amount of illumination added when lights are set to minimum 
        /// </summary>
        public int IlluminationAddedAtMin { get; set; }
        /// <summary>
        /// The amount of illumination added when lights are set to maximum 
        /// </summary>
        public int IlluminationAddedAtMax { get; set; }    

        /// <summary>
        /// The minimum value usually reported by the sensor
        /// Needs consideration
        /// </summary>
        //public int MinIllumination { get; set; } = 0;
    }
} 

/// <summary>
/// Provides a means of adjusting lights when used with a light sensor to maintain a consistent light level given variability of an outside light source (the sun).
/// </summary>
public class DynamicLightAdjuster : IDynamicLightAdjuster
{
    IDynamicLightAdjuster.DynamicLightModel _model;
    double _m, _b;
    ILogger<DynamicLightAdjuster> _logger;

    public DynamicLightAdjuster(IDynamicLightAdjuster.DynamicLightModel model, ILogger<DynamicLightAdjuster> logger)
    {
        _model = model;
        _logger = logger;

        // slope and y intercpt for use later
        _m = (double)(_model.IlluminationAddedAtMax - _model.IlluminationAddedAtMin)/(double)(_model.MaxLightBrightness - _model.MinBrightness);
        _b = _model.IlluminationAddedAtMax - _m * _model.MaxLightBrightness;
    }

    public double GetAppropriateBrightness(double illumination, double currentBrightness)
    {
        var actualIllumination = GetActualIllumination(illumination, currentBrightness);

        if (actualIllumination < 0)
        {
            _logger.LogWarning("somehow calculated impossible situation. Actual illuminatin cannot be negative. Calculated: {calculated}", actualIllumination);
            actualIllumination = 0;
        }

        //calculate what it should be and return
        return GetBrightnessFromIllumination(_model.TargetIllumination - actualIllumination);
    }

    public double GetActualIllumination(double illumination, double currentBrightness)
    {
        var illuminationAddedByLight = GetIlluminationFromBrightness(currentBrightness);
        
        // subtract it from the illumination
        var actualIllumination = illumination - illuminationAddedByLight;
        return actualIllumination;
    }

    public double GetIlluminationFromBrightness(double currentBrightness)
    {
        if (currentBrightness == 0) return 0;

        // y = mx + b
        return _m * currentBrightness + _b;
    }

    double GetBrightnessFromIllumination(double illuminationToAdd)
    {
        // x = (y-b)/m
        var result = (illuminationToAdd - _b)/_m;
        if(result < _model.MinBrightness) return 0;
        if(result > _model.MaxLightBrightness) return _model.MaxLightBrightness;
        return result;
    }
}

