
using HaKafkaNet;

namespace MyHome;

public class MonkeyNotificationChannel : INoTextNotificationChannel, INotificationClearingChannel
{
    IHaApiProvider _api;
    LightAlertModule _lam;
    LightTurnOnModel _effects;
    public MonkeyNotificationChannel(IHaApiProvider api, LightAlertModule lam, LightTurnOnModel effects)
    {
        _api = api;
        _lam = lam;
        _effects = effects;
    }

    public Task Clear(NotificationId id)
    {
        _lam.Clear(id);
        return Task.CompletedTask;
    }

    public Task ClearAll()
    {
        _lam.ClearAll();
        return Task.CompletedTask;
    }

    public Task Send(NotificationId id)
    {
        _lam.Add(id, _effects);
        return Task.CompletedTask;
    }
}

