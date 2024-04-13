
using HaKafkaNet;

namespace MyHome;

public class AudibleNotificationChannel : INotificationChannel
{
    readonly IHaApiProvider _api;
    readonly string[] _targets;
    readonly PiperSettings? _voiceSettings;

    public AudibleNotificationChannel(IHaApiProvider api, IEnumerable<string> mediaPlayerTargets, PiperSettings? piperSettings = null)
    {
        _api = api;
        _voiceSettings = piperSettings;
        _targets = mediaPlayerTargets.ToArray();
    }

    public Task Send(NotificationId id, string message, string? title = null)
    {
        return _api.SpeakPiper(_targets, message, true, _voiceSettings);
    }
}
