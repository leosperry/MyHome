using System;
using HaKafkaNet;

namespace MyHome.Services;

public class CombinedLight
{
    readonly IHaApiProvider _api;
    readonly CombinedLightModel[] _lights;
    readonly ReaderWriterLockSlim _lock;
    readonly ILogger? _logger;

    public CombinedLight(IHaApiProvider api, ILogger? loggger = null, params CombinedLightModel[] lights)
    {
        _api = api;
        _logger = loggger;
        foreach (var light in lights)
        {
            // set slope
            light.Slope = ((float)light.Max - (float)light.Min) / (255f);
        }
        _lights = lights;
        _lock = new();
    }

    public async Task Set(byte brightness, CancellationToken ct)
    {
        List<Task> tasks = new();
        foreach (var light in _lights)
        {
            // calculate 1 to 255
            // y = m*x + b
            // y is the output
            float y = CalculateBrightness(brightness, light);

            if (y <= 0)
            {
                tasks.Add(_api.LightTurnOff(light.EntityId));
            }
            else
            {
                var newBrightness = (byte)Math.Round(y);

                // for now
                tasks.Add(_api.LightSetBrightness(light.EntityId, newBrightness, ct));

                //for later
                // if (light.SetKelvin)
                // {

                // }
                // else
                // {

                // }
            }
        }

        tasks.Add(Task.Run(() =>{
            set_latest(brightness);
        }));
        
        await Task.WhenAll(tasks);
    }

    private static float CalculateBrightness(byte brightness, CombinedLightModel light)
    {
        // y = mx + b
        var y = light.Slope * (brightness) + light.Min;

        if (y < light.Min) y = light.Min;
        if (y > light.Max) y = light.Max;
        return y;
    }

    private static float ReverseCalculation(byte observed, CombinedLightModel light)
    {
        // x = (y-b)/m
        var x = (observed + light.Min) / light.Slope;
        
        if(x < 0) x = 0;
        if(x > 255) x = 255;

        return x;
    }

    internal void RecordBrightnessFromObservation(Byte? brightness)
    {
        var primary = _lights.FirstOrDefault(l => l.primary);
        if (primary is null)
        {
            _logger?.LogWarning("cannot record outside observation for dynamic lighs. No primary is set");
            return;
        }

        bool shouldSet = false;
        _lock.EnterUpgradeableReadLock();
        try
        {
            float calculated = 0;
            if (brightness is null && _latest != 0)
            {
                shouldSet = true;
            }
            else 
            {
                calculated = CalculateBrightness(_latest, primary);
                var diff = calculated - brightness;
                if (diff is not null && Math.Abs(diff.Value)> 10)
                {
                    shouldSet = true;
                }
            }

            if (shouldSet)
            {
                byte reversed = brightness is null ? (byte)0 : (byte)Math.Round(ReverseCalculation(brightness.Value, primary));
                _logger?.LogInformation("adjusting latest brightness. Was {old_brightness} Now {new_brightness}", _latest, reversed);
                set_latest(reversed);
            }
        }
        finally
        {
            _lock.ExitUpgradeableReadLock();
        }
    }

    private void set_latest(byte? latest)
    {
        _lock.EnterWriteLock();
        try
        {
            _latest = latest ?? 0;
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    // fix: initializes to zero
    private byte _latest;
    public byte LatestBrightness 
    { 
        get
        {
            _lock.EnterReadLock();
            try
            {
                return _latest;
            }
            finally
            {
                _lock.ExitReadLock();
            }
        } 
    }
}

public record CombinedLightModel(string EntityId, short Min, short Max, bool SetKelvin = false, bool primary = false)
{
    internal float Slope {get;set;}
}


