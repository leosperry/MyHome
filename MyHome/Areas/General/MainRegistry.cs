using HaKafkaNet;

namespace MyHome;

public class MainRegistry : IAutomationRegistry
{
    readonly IHaServices _services;
    readonly IAutomationFactory _factory;
    readonly IAutomationBuilder _builder;
    private readonly INotificationService _notificationService;

    public MainRegistry(IHaServices services, IStartupHelpers helpers, INotificationService notificationService)
    {
        _services = services;
        _factory = helpers.Factory;
        _builder = helpers.Builder;
        this._notificationService = notificationService;
    }

    public void Register(IRegistrar reg)
    {
        reg.RegisterMultiple(
            DiningRoomVolumeAdjust(),
            ClearNotifications()
            );

        // lights auto off
        reg.RegisterMultiple(
            _factory.DurableAutoOff(Helpers.MaintenanceMode, TimeSpan.FromHours(1)).WithMeta("auto off maintenance mode", "1 hour"),
            _factory.DurableAutoOff("switch.back_hall_light", TimeSpan.FromMinutes(10)).WithMeta("auto off back hall","10 min"),
            _factory.DurableAutoOff("light.upstairs_hall", TimeSpan.FromMinutes(30)).WithMeta("auto off upstairs hall","30 min"),
            _factory.DurableAutoOff("light.entry_light", TimeSpan.FromMinutes(30)).WithMeta("auto off entry light","30 min"),
            _factory.DurableAutoOffOnEntityOff([Lights.MainBedroomLight1, Lights.MainBedroomLight2, Lights.CraftRoomLights], Sensors.MainBedroom4in1Motion, TimeSpan.FromMinutes(10))
                .WithMeta("mainbedroom off on no motion","10 minutes")           
        );
    }

    private IAutomation ClearNotifications()
    {
        return _builder.CreateSimple()
            .WithName("clear notifications")
            .WithTriggers(Helpers.ClearNotificationButton)
            .WithExecution((sc, ct) => _notificationService.ClearAll())
            .Build();
    }

    IAutomation DiningRoomVolumeAdjust()
    {
        return _builder.CreateSimple()
            .WithName("Adjust Dining room notification volume")
            .WithDescription("using binary_sensor.house_active_times_of_day adjust the volume of dining room speaker")
            .WithTriggers("binary_sensor.house_active_times_of_day")
            .WithExecution((sc, ct) => {
                var isActive = sc.ToOnOff();
                if (isActive.New.State == OnOff.On)
                {
                    return _services.Api.MediaPlayerSetVolume(MediaPlayers.DiningRoom, MediaPlayers.DiningRoomActiveVolume);
                }
                else
                {
                    return _services.Api.MediaPlayerSetVolume(MediaPlayers.DiningRoom, MediaPlayers.DiningRoomInActiveVolume);
                }
            })
            .Build();
    }
}
