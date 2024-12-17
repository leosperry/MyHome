using HaKafkaNet;
using MyHome.Services;

namespace MyHome;



public interface INotificationService
{
    NotificationSender CreateNotificationSender(IEnumerable<INotificationChannel> channels, IEnumerable<INoTextNotificationChannel>? noTextChannels = null);
    NotificationSenderNoText CreateNoTextNotificationSender(IEnumerable<INoTextNotificationChannel> noTextChannels);
    
    INotificationChannel CreateGroupOrDeviceChannel(params string[] device_targets);
    INotificationChannel CreateAudibleChannel(IEnumerable<string> media_player_targets, PiperSettings? voiceSettings = null);
    INoTextNotificationChannel CreateMonkeyChannel(LightTurnOnModel effects);
    NotificationSender CreateInformationalSender();
    INotificationChannel Persistent { get; }

    Task Clear(NotificationId id);
    Task ClearAll();
}


public interface INotificationChannel 
{ 
    Task Send(NotificationId id, string message, string? title = null);
}

public interface INoTextNotificationChannel 
{
    Task Send(NotificationId id);
}

public interface INotificationClearingChannel
{
    Task Clear(NotificationId id);
    Task ClearAll();
}


public readonly record struct NotificationId(string Value)
{
    public static implicit operator string(NotificationId id) => id.Value;
}

public class NotificationService : INotificationService
{
    readonly IHaServices _services;
    readonly LightAlertModule _lam;
    private readonly INotificationObserver _notificationObserver;
    private readonly ILogger<NotificationService> _logger;
    readonly INotificationChannel _persistent;

    readonly List<INotificationClearingChannel> _clearers = new();

    public NotificationService(IHaServices services, LightAlertModule lam, INotificationObserver notificationObserver, ILogger<NotificationService> logger)
    {
        _services = services;
        _lam = lam;
        this._notificationObserver = notificationObserver;
        this._logger = logger;
        _persistent = new PersistentNotificationChannel(services.Api);
    }

    public INotificationChannel Persistent => _persistent;

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

    public INotificationChannel CreateAudibleChannel(IEnumerable<string> media_player_targets, PiperSettings? voiceSettings = null)
    {
        return new AudibleNotificationChannel(_services.Api, media_player_targets, _logger, _notificationObserver, voiceSettings);
    }

    public INotificationChannel CreateGroupOrDeviceChannel(params string[] device_targets)
    {
        return new DeviceNotificationChannel(_services.Api, device_targets);
    }

    public INoTextNotificationChannel CreateMonkeyChannel(LightTurnOnModel effect)
    {
        var monkey = new MonkeyNotificationChannel(_services.Api, _lam, effect);
        _clearers.Add(monkey);
        return monkey;
    }

    public NotificationSender CreateInformationalSender()
    {
        return CreateNotificationSender([Persistent], [CreateMonkeyChannel(new LightTurnOnModel{
            EntityId = [Light.MonkeyLight],
            ColorName = "lightpink",
            Brightness = Bytes._30pct
        })]);
    }

    public NotificationSenderNoText CreateNoTextNotificationSender(IEnumerable<INoTextNotificationChannel> noTextChannels)
    {
        INoTextNotificationChannel[] channels = noTextChannels.ToArray();
        return async id => {
            if (id is null)
            {
                id = new NotificationId(Guid.NewGuid().ToString());
            }
            await Task.WhenAll(
                channels.Select(c => c.Send(id.Value)).ToArray()
            );
            return id.Value;
        };
    }

    public NotificationSender CreateNotificationSender(IEnumerable<INotificationChannel> channels, IEnumerable<INoTextNotificationChannel>? noTextChannels = null)
    {
        return async (message, title, id) => {
            if (id is null)
            {
                id = new NotificationId(Guid.NewGuid().ToString());
            }

            var textTasks = channels.Select(c => c.Send(id.Value, message, title));
            if (noTextChannels is not null)
            {
                await Task.WhenAll(noTextChannels.Select(ntc => ntc.Send(id.Value)));
            }

            await Task.WhenAll(textTasks);

            return id.Value;
        };
    }  
}

public delegate Task<NotificationId> NotificationSender(string message, string? title = null, NotificationId? id = null);
public delegate Task<NotificationId> NotificationSenderNoText(NotificationId? id = null);



