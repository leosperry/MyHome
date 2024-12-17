
using HaKafkaNet;
using MyHome.Services;

namespace MyHome;

public class AudibleNotificationChannel : INotificationChannel
{
    readonly IHaApiProvider _api;
    readonly HashSet<string> _targets;
    private readonly INotificationObserver _notificationObserver;
    private readonly ILogger _logger;
    readonly PiperSettings? _voiceSettings;

    public AudibleNotificationChannel(IHaApiProvider api, IEnumerable<string> mediaPlayerTargets, ILogger logger, INotificationObserver notificationObserver, PiperSettings? piperSettings = null)
    {
        _api = api;
        _voiceSettings = piperSettings;
        _targets = mediaPlayerTargets.ToHashSet();
        this._notificationObserver = notificationObserver;
        this._logger = logger;
    }

    public async Task Send(NotificationId id, string message, string? title = null)
    {
        var response = await _api.SpeakPiper(_targets, title ?? message, true, _voiceSettings);
        if(!response.IsSuccessStatusCode)
        {
            _logger.LogError("piper did not succeed - status:{status_code}, reason:{reason}", response.StatusCode, response.ReasonPhrase);
        }
        if (_targets.Contains(Media_Player.DiningRoomSpeaker))
        {
            _notificationObserver.OnNotificationSent(message);
        }
    }
}
