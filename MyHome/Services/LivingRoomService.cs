using System.Text.Json;
using HaKafkaNet;

namespace MyHome;

public interface ILivingRoomService
{
    Task SetLightsBasedOnPower(float? currentPower = null, CancellationToken ct = default);
}

public class LivingRoomService : ILivingRoomService
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

    public async Task SetLightsBasedOnPower(float? currentPower = null, CancellationToken ct = default)
    {
        if (currentPower is null)
        {
            currentPower = (await _entityProvider.GetFloatEntity(Devices.SolarPower))?.State;    
        }
        if (currentPower is null)
        {
            _logger.LogWarning("could not get solar power state");
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
