using System.Text.Json.Serialization;

namespace MyHome;

public class SimpleLightModel
{
        [JsonPropertyName("brightness")]
        public byte? Brightness { get; set; }
}
