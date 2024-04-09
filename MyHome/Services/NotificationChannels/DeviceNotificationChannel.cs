
using HaKafkaNet;

namespace MyHome;

public class DeviceNotificationChannel : INotificationChannel
{
    readonly IHaApiProvider _api;
    readonly string[] _devices;
    public DeviceNotificationChannel(IHaApiProvider api, params string[] devicesOrGroups)
    {
        _api = api;
        _devices = devicesOrGroups;
    }
    public Task Send(NotificationId id, string message, string? title = null)
    {
        return Task.WhenAll(
            from d in _devices
            select _api.NotifyGroupOrDevice(d, message)
        );
    }
}
