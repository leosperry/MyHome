
using HaKafkaNet;

namespace MyHome;

public class PersistentNotificationChannel : INotificationChannel
{
    readonly IHaApiProvider _api;
    public PersistentNotificationChannel(IHaApiProvider api)
    {
        _api = api;
    }

    public Task Send(NotificationId id, string message, string? title = null)
    {
        return _api.PersistentNotificationDetail(message, title, id);
    }
}
