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

    public async Task Set(byte brightness, int kelvin, CancellationToken ct)
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

                if (light.SetKelvin)
                {
                    tasks.Add(_api.LightTurnOn(new LightTurnOnModel()
                    {
                        EntityId = [light.EntityId],
                        Brightness = newBrightness,
                        Kelvin = kelvin
                    }));
                }
                else
                {
                    tasks.Add(_api.LightSetBrightness(light.EntityId, newBrightness, ct));
                }
            }
        }

        tasks.Add(Task.Run(() =>{
            set_latest(brightness);
        }));
        
        await Task.WhenAll(tasks);
    }

    public async Task TurnOff(CancellationToken cancellationToken = default)
    {
        var t =  Task.WhenAll(_lights.Select(async l => await _api.TurnOff(l.EntityId, cancellationToken)));
        set_latest(0);
        await t;
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

public record CombinedLightModel(string EntityId, short Min, short Max, bool SetKelvin = false)
{
    internal float Slope {get;set;}
}


