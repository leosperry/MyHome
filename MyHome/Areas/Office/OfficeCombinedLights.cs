using System;
using HaKafkaNet;

namespace MyHome;


public class OfficeCombinedLights : IAutomation_ColorLight, IAutomationMeta
{
    private readonly IHaServices _services;

    public EventTiming EventTimings { get => EventTiming.Durable; }

    private readonly CombinedLightModel2 _caseLightModel = new()
    {
        TurnOnAt = 127,
        Min = 25,
        Max = Bytes.PercentToByte(20)
    };
    private readonly CombinedLightModel2 _LightBarsModel = new()
    {
        TurnOnAt = 0,
        Min = Bytes.PercentToByte(20),
        Max = Bytes._100pct
    };

    public OfficeCombinedLights(IHaServices services)
    {
        this._services = services;
    }

    public async Task Execute(HaEntityStateChange<HaEntityState<OnOff, ColorLightModel>> stateChange, CancellationToken ct)
    {
        if(stateChange.New.Bad() || stateChange.New.Attributes is null) return;
        

        if(stateChange.New.Attributes.Brightness is null)
        {
            await TurnOff();
        }
        else
        {
            await TurnOn(stateChange.New.Attributes);
        }
    }

    static readonly string[] _colorLights = [Light.OfficeLedLight, Light.OfficeLightBars];

    private async Task TurnOn(ColorLightModel attributes)
    {
        var newModelFromInput = GetFromState(attributes);
        newModelFromInput.EntityId = _colorLights;

        var inputBrightness = attributes.Brightness!.Value;

        var lightBarBrightness = _LightBarsModel.GetBrightness(inputBrightness);
        newModelFromInput.Brightness = lightBarBrightness;

        var caseBrightness = _caseLightModel.GetBrightness(inputBrightness);

        await Task.WhenAll(
            _services.Api.LightSetBrightness(Light.OfficeDisplayLights, caseBrightness),
            _services.Api.LightTurnOn(newModelFromInput)
        );
    }

    private async Task TurnOff()
    {
        await _services.Api.TurnOff(_colorLights.Append(Light.OfficeDisplayLights));
    }

    public IEnumerable<string> TriggerEntityIds()
    {
        yield return Light.OfficeCombinedLight;
    }

    private LightTurnOnModel GetFromState(ColorLightModel lightModel)
    {
        if(lightModel.ColorMode == "color_temp")
        {
            return new()
            {
                Brightness = lightModel.Brightness,
                Kelvin = lightModel.TempKelvin
            };
        }
        else if(lightModel.ColorMode == "rgb")
        {
            return new()
            {
                Brightness = lightModel.Brightness,
                RgbColor = lightModel.RGB,
            };
        }
        
        return new LightTurnOnModel()
        {
            Brightness = lightModel.Brightness
        };
    }

    public AutomationMetaData GetMetaData()
    {
        return new AutomationMetaData()
        {
            Name = "Office Lights Combined",
            Mode = AutomationMode.Smart
        };
    }
}

class CombinedLightModel2
{
    public byte TurnOnAt { get; set; }
    public byte Min { get; set; }
    public byte Max { get; set; }

    public byte GetBrightness(byte inputBrightness)
    {
        if (inputBrightness < TurnOnAt)
        {
            return 0;
        }

        float range = Max - Min;
        var brightnessPerInput = range / byte.MaxValue;

        var brightnessAboveMin = inputBrightness - TurnOnAt;
        var output = brightnessAboveMin * brightnessPerInput + Min;

        return (byte)output;
    }
}
