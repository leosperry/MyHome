using System.Text.Json.Serialization;
using HaKafkaNet;

namespace MyHome.Dev;

public class TestLightTriggered : IAutomation
{
    public Task Execute(HaEntityStateChange stateChange, CancellationToken cancellationToken)
    {
        
        var lightState = stateChange.ToOnOff<LightModel>();

        //var rgb = stateChange.New.FromAttributes<RgbTuple>("rgb_color");
        var rgb = lightState.New.Attributes?.Rgb;
        
        Console.WriteLine($"the light is now: {lightState.New.State}");
        Console.WriteLine($"RGB is: {rgb}");

        return Task.CompletedTask;

    }

    public IEnumerable<string> TriggerEntityIds()
    {
        yield return "light.office_led_light";
    }
}

public class LightModel
{
    [JsonPropertyName("friendly_name")]
    public string FriendlyName{ get; set; } = string.Empty;
    
    [JsonPropertyName("supported_features")]
    public int Supported_Features { get; set; }

    [JsonPropertyName("rgb_color")]
    public RgbTuple? Rgb { get; set; }
}
