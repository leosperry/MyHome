
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
        return _lam.Clear(id);
    }

    public Task ClearAll()
    {
        return _lam.ClearAll();
    }

    public Task Send(NotificationId id)
    {
        return _lam.Add(id, _effects);
    }
}

