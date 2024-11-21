using System.Text.Json;
using HaKafkaNet;

namespace MyHome;

public class FrontPorchMotion : IAutomation<OnOff>, IAutomationMeta
{
    readonly IHaApiProvider _api;
    readonly IHaEntityProvider _provider;
    readonly INotificationService _notifications;

    readonly NotificationSenderNoText _alert;
    readonly NotificationId _porchMotionID = new NotificationId("frontportchmotion");

    public FrontPorchMotion(IHaApiProvider api, IHaEntityProvider provider, INotificationService notifications)
    {
        _api = api;
        _provider = provider;
        _notifications = notifications;

        var monkey = notifications.CreateMonkeyChannel(new LightTurnOnModel()
        {
            EntityId = [Lights.Monkey],
            ColorName = "darkgreen",
            Brightness = Bytes._10pct
        });
        _alert = notifications.CreateNoTextNotificationSender([monkey]);
    }

    public IEnumerable<string> TriggerEntityIds()
    {
        yield return Sensors.FrontPorchMotion;
    }

    public Task Execute(HaEntityStateChange<HaEntityState<OnOff, JsonElement>> motionState, CancellationToken ct)
    {
        if (motionState.New.State == OnOff.On)
        {
            _alert(_porchMotionID);
            
            return Task.WhenAll(
                HandleNight(ct)
                //HandleCamera(ct)
            );        
        }
        else
        {
            _notifications.Clear(_porchMotionID);
        }
        return Task.CompletedTask;
    }

    // private async Task HandleCamera(CancellationToken ct)
    // {
    //     // this code has been disabled
    //     // stupid Alexa can't do this without making an audible noise
    //     // so the dog goes ape shit.
    //     // If there's an amazon developer reading this,
    //     // understand that this is just one more reason why I 
    //     // will be abandoning your services entirely.
    //     // Alexa is becoming more obtrusive and obnoxious by the day 
    //     var enableState = await _provider.GetOnOffEntity(Helpers.PorchMotionEnable);
    //     if (enableState?.State == OnOff.On)
    //     {
    //         // tell echo to play camera
    //         // wait 10 seconds
    //         // turn it off
    //         await _api.CallService("media_player", "play_media", new{
    //             entity_id = "media_player.kitchen",
    //             media_content_type = "custom",
    //             media_content_id = "Show me the doorbell camera on living room"
    //         }, ct);

    //         await Task.Delay(TimeSpan.FromSeconds(15));

    //         await _api.CallService("media_player", "play_media", new{
    //             entity_id = "media_player.living_room",
    //             media_content_type = "custom",
    //             media_content_id = "stop"
    //         }, ct);
    //     }
    // }

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
            Description = "Turn on light after sunset"
        };
    }
}
