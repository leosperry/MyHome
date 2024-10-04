using System.Text.Json;
using HaKafkaNet;

namespace MyHome;


public class LivingRoomService
{
    readonly IHaApiProvider _api;
    readonly IHaEntityProvider _entityProvider; 
    readonly ILogger _logger;

    const double THRESHOLD = 700;

    public LivingRoomService(IHaApiProvider api, IHaEntityProvider entityProvider, ILogger<LivingRoomService> logger)
    {
        _api = api;
        _entityProvider = entityProvider;
        _logger = logger;
    }

    public async Task SetLights(DateTime? lastOccupied = null, bool? overrideOn = null, float? currentPower = null, CancellationToken ct = default)
    {
        // if override is on , do nothing
        if ((overrideOn is null && (await _entityProvider.GetOnOffEntity(Helpers.LivingRoomOverride)).IsOn()) || overrideOn == true)
        {
            return;
        }

        if (await HasBeenUnoccupiedForXminutes(10, lastOccupied) == true)
        {
            // turn off
            await _api.TurnOff([Lights.TvBacklight, Lights.CounchOverhead]);
            return;
        }

        if (currentPower is null)
        {
            currentPower = (await _entityProvider.GetFloatEntity(Devices.SolarPower))?.State;
        }
        if (currentPower is null) // if it's still null we have an issue
        {
            _logger.LogWarning("could not fetch current solar power");
        }
        else
        {
            await SetLightsBasedOnPower(currentPower.Value, ct);
        }
    }

    private async Task<bool?> HasBeenUnoccupiedForXminutes(int minutes, DateTime? lastOccupied)
    {
        var occupiedAt = lastOccupied ?? await FetchLastOccupied();

        if (occupiedAt is null)
        {
            _logger.LogWarning("could not get living room occupied state");
            return null;
        }
        
        return (DateTime.Now - occupiedAt.Value).TotalMinutes > minutes;
    }

    private async Task<DateTime?> FetchLastOccupied()
    {
        var presenceState = await _entityProvider.GetOnOffEntity(Sensors.LivingRoomPresence);
        if (presenceState is null)
        {
            return null;
        }
        if (presenceState.IsOn())
        {
            return DateTime.Now;
        }
        return presenceState.LastChanged;
    }

    private async Task SetLightsBasedOnPower(float currentPower, CancellationToken ct)
    {
        if (currentPower > THRESHOLD)
        {
            // turn off when we have plenty of light
            await _api.TurnOff([Lights.TvBacklight, Lights.CounchOverhead]);
        }
        else
        {
            // crazy calc time
            // get the difference
            // divide by threshold to get decimal percentage
            // raise to power of 2 for parabolic
            // set maximum for each light
            // convert to byte
            var unmodifiedValue = Math.Pow((THRESHOLD - currentPower)/ THRESHOLD, 2) * 255;
            await Task.WhenAll(
                _api.LightTurnOn(new LightTurnOnModel()
                {
                    EntityId = [Lights.TvBacklight],
                    Brightness = (byte)Math.Round(unmodifiedValue * 0.6),
                    Kelvin = 2202,
                },ct),
                _api.LightTurnOn(new LightTurnOnModel()
                {
                    EntityId = [Lights.CounchOverhead],
                    Brightness = (byte)Math.Round(unmodifiedValue * 0.25),
                    RgbColor = (255, 146, 39),
                }, ct)
            );
        } 
    }
}

