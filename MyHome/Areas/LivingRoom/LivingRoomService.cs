using System.Text.Json;
using HaKafkaNet;

namespace MyHome;


public class LivingRoomService
{
    readonly IHaApiProvider _api;
    readonly IHaEntityProvider _entityProvider; 
    readonly ILogger _logger;
    private readonly IHaEntity<float?, JsonElement> _powerSensor;
    private readonly IHaEntity<OnOff, JsonElement> _override;
    private readonly IHaEntity<OnOff, JsonElement> _presence;
    const double THRESHOLD = 700;

    public LivingRoomService(IHaApiProvider api, IHaEntityProvider entityProvider, IUpdatingEntityProvider updatingEntityProvider, ILogger<LivingRoomService> logger)
    {
        _api = api;
        _entityProvider = entityProvider;
        _logger = logger;

        _powerSensor = updatingEntityProvider.GetFloatEntity(Devices.SolarPower);
        _override = updatingEntityProvider.GetOnOffEntity(Helpers.LivingRoomOverride);
        _presence = updatingEntityProvider.GetOnOffEntity(Sensors.LivingRoomPresence);
    }

    public async Task SetLights(CancellationToken ct = default)
    {
        // if override is on , do nothing
        if (_override.IsOff())
        {
            if (HasBeenUnoccupiedForXminutes(10) == true)
            {
                // turn off
                await _api.TurnOff([Lights.TvBacklight, Lights.CounchOverhead]);
                return;
            }

            await SetLightsBasedOnPower(ct);
        }
    }

    private bool? HasBeenUnoccupiedForXminutes(int minutes)
    {
        var occupiedAt = FetchLastOccupied();

        if (occupiedAt is null)
        {
            return null;
        }
        
        return (DateTime.Now - occupiedAt.Value).TotalMinutes > minutes;
    }

    private DateTime? FetchLastOccupied()
    {
        if (_presence.Bad())
        {
            _logger.LogWarning(Sensors.LivingRoomPresence + " is in a bad state: {state}", _presence.State);
            return null;
        }
        if (_presence.IsOn())
        {
            return DateTime.Now;
        }
        return _presence.LastChanged;
    }

    private async Task SetLightsBasedOnPower(CancellationToken ct)
    {
        var currentPower = _powerSensor.State;
        if (currentPower is null)
        {
            _logger.LogWarning("could not read solar power");
            return;
        }
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
            var unmodifiedValue = Math.Pow((THRESHOLD - currentPower.Value)/ THRESHOLD, 2) * 255;
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

