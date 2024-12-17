using System.Text.Json;
using HaKafkaNet;

namespace MyHome;

public class MainRegistry : IAutomationRegistry
{
    readonly IHaServices _services;
    readonly IAutomationFactory _factory;
    readonly IAutomationBuilder _builder;
    private readonly INotificationService _notificationService;
    private readonly INoTextNotificationChannel _maintenanceChannel;

    public MainRegistry(IHaServices services, IStartupHelpers helpers, INotificationService notificationService)
    {
        _services = services;
        _factory = helpers.Factory;
        _builder = helpers.Builder;
        this._notificationService = notificationService;

        var maintenanceModeLightSettings = new LightTurnOnModel()
        {
            EntityId = [Light.MonkeyLight],
            RgbColor = (255, 100, 0),
            Brightness = Bytes._75pct
        };

        this._maintenanceChannel = _notificationService.CreateMonkeyChannel(maintenanceModeLightSettings);
    }

    public void Register(IRegistrar reg)
    {
        var exceptions = reg.TryRegister(
            DiningRoomVolumeAdjust,
            ClearNotifications,
            ReportMainenanceMode
        );

        // lights auto off
        reg.TryRegister(
            _factory.DurableAutoOff(Input_Boolean.MaintenanceMode, TimeSpan.FromHours(1)).WithMeta("auto off maintenance mode", "1 hour"),
            _factory.DurableAutoOff(Light.UpstairsHall, TimeSpan.FromMinutes(30)).WithMeta("auto off upstairs hall","30 min"),
            _factory.DurableAutoOff(Light.EntryLight, TimeSpan.FromMinutes(30)).WithMeta("auto off entry light","30 min")
        );
    }

    IAutomationBase ReportMainenanceMode()
    {
        const string maintenance_notification_id = "maintenance_mode";
        return _builder.CreateSimple<OnOff>()
            .WithName(nameof(ReportMainenanceMode))
            .WithTriggers(Input_Boolean.MaintenanceMode)
            .WithExecution(async (sc, ct) => {
                if (sc.IsOn())
                {
                    await _maintenanceChannel.Send(new(maintenance_notification_id));
                }
                else
                {
                    await _notificationService.Clear(new(maintenance_notification_id));
                }
            })
            .Build();
    }

    private IAutomationBase ClearNotifications()
    {
        return _builder.CreateSimple<DateTime?>()
            .WithName("clear notifications")
            .WithTriggers(Input_Button.ClearNotifications)
            .WithExecution(async (sc, ct) => {
                if (sc.New.StateAndLastUpdatedWithin1Second())
                {
                    await _notificationService.ClearAll();
                }
            })
            .Build();
    }

    IAutomation<OnOff,JsonElement> DiningRoomVolumeAdjust()
    {
        return _builder.CreateSimple<OnOff>()
            .WithName("Adjust Dining room notification volume")
            .WithDescription("using binary_sensor.house_active_times_of_day adjust the volume of dining room speaker")
            .WithTriggers("binary_sensor.house_active_times_of_day")
            .WithExecution((sc, ct) => {
                if (sc.IsOn())
                {
                    return _services.Api.MediaPlayerSetVolume(Media_Player.DiningRoomSpeaker, MediaPlayer.DiningRoomActiveVolume);
                }
                else
                {
                    return _services.Api.MediaPlayerSetVolume(Media_Player.DiningRoomSpeaker, MediaPlayer.DiningRoomInActiveVolume);
                }
            })
            .Build();
    }
}
