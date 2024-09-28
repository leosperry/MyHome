using System;
using HaKafkaNet;

namespace MyHome.Services;

public class CombinedLight
{
    readonly IHaApiProvider _api;
    readonly CombinedLightModel[] _lights;

    public CombinedLight(IHaApiProvider api, params CombinedLightModel[] lights )
    {
        _api = api;
        foreach (var light in lights)
        {
            // set slope
            light.Slope = ((float)light.Max - (float)light.Min) / (255f);
        }
        _lights = lights;
    }

    public async Task Set(byte brightness)
    {
        if (brightness == 0)
        {
            await _api.TurnOff(_lights.Select(l => l.EntityId));
            return;
        }

        List<Task> tasks = new();
        foreach (var light in _lights)
        {
            // calculate 1 to 255

            // y = m*x + b
            // y is the output

            var y = light.Slope * (brightness) + light.Min;

            if (y < light.Min) y = light.Min;
            if (y > light.Max) y = light.Max;

            var newBrightness = (byte)Math.Round(y);

            tasks.Add(_api.LightSetBrightness(light.EntityId, newBrightness));
        }
        LatestBrightness = brightness;
        await Task.WhenAll(tasks);
    }

    // fix: initializes to zero
    public byte LatestBrightness { get; private set; }
}

public record CombinedLightModel(string EntityId, byte Min, byte Max)
{
    internal float Slope {get;set;}
}
