using HaKafkaNet;

namespace MyHome;

public class FrontPorchMotion : IAutomation, IAutomationMeta
{
    readonly IHaApiProvider _api;
    readonly IHaEntityProvider _provider;

    public FrontPorchMotion(IHaApiProvider api, IHaEntityProvider provider)
    {
        _api = api;
        _provider = provider;
    }

    public Task Execute(HaEntityStateChange stateChange, CancellationToken ct)
    {
        var motionState = stateChange.ToOnOff();
        if (motionState.New.State == OnOff.On)
        {
            return Task.WhenAll(
                HandleNight(ct),
                HandleCamera(ct)
            );        
        }
        return Task.CompletedTask;
    }

    private async Task HandleCamera(CancellationToken ct)
    {
        var enableState = await _provider.GetOnOffEntity(Helpers.PorchMotionEnable);
        if (enableState?.State == OnOff.On)
        {
            // tell echo to play camera
            // wait 10 seconds
            // turn it off
            await _api.CallService("media_player", "play_media", new{
                entity_id = "media_player.kitchen",
                media_content_type = "custom",
                media_content_id = "Show me the doorbell camera on living room"
            }, ct);

            await Task.Delay(TimeSpan.FromSeconds(15));

            await _api.CallService("media_player", "play_media", new{
                entity_id = "media_player.living_room",
                media_content_type = "custom",
                media_content_id = "stop"
            }, ct);
        }
    }

    private async Task HandleNight(CancellationToken ct)
    {
        var sun = await _provider.GetSun();
        if (sun!.State == SunState.Below_Horizon)
        {
            await _api.TurnOn(Lights.FrontPorchLight, ct);
        }
    }

    public AutomationMetaData GetMetaData()
    {
        return new()
        {
            Name = "Front porch motion",
            Description = "Turn on light after sunset and echo show if enabled"
        };
    }

    public IEnumerable<string> TriggerEntityIds()
    {
        yield return Sensors.FrontPorchMotion;
    }
}
