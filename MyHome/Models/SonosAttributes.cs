using System;
using System.Text.Json.Serialization;
using HaKafkaNet;

namespace MyHome.Models;

public class SonosAttributes
{
    [JsonPropertyName("volume_level")]
    public float VolumeLevel { get; set; }

    [JsonPropertyName("is_volume_muted")]
    public bool IsVolumeMuted { get; set; }

    [JsonPropertyName("media_content_type")]
    public string? MediaContentType { get; set; }

    [JsonPropertyName("media_position_updated_at")]
    public DateTime? MediaPositionUpdatedAt { get; set; }

    [JsonPropertyName("shuffle")]
    public bool Shuffle { get; set; }

    // [JsonPropertyName("repeat")]
    // public Repeat Repeat { get; set; }
}
