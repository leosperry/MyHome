using HaKafkaNet;

namespace MyHome;

public class MainRegistry : IAutomationRegistry
{
    readonly IHaServices _services;
    readonly IAutomationFactory _factory;
    readonly IAutomationBuilder _builder;


    public MainRegistry(IHaServices services, IAutomationFactory factory, IAutomationBuilder builder)
    {
        _services = services;
        _factory = factory;
        _builder = builder;
    }

    public void Register(IRegistrar reg)
    {
        var DiningRoomVolumeAdjust = _builder.CreateSimple()
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

        reg.RegisterMultiple(DiningRoomVolumeAdjust);

        // lights auto off
        reg.RegisterMultiple(
            _factory.DurableAutoOff("switch.back_hall_light", TimeSpan.FromMinutes(10)).WithMeta("auto off back hall","10 min"),
            _factory.DurableAutoOff("light.upstairs_hall", TimeSpan.FromMinutes(30)).WithMeta("auto off upstairs hall","30 min"),
            _factory.DurableAutoOff("light.entry_light", TimeSpan.FromMinutes(30)).WithMeta("auto off entry light","30 min"),
            _factory.DurableAutoOffOnEntityOff([Lights.MainBedroomLight1, Lights.MainBedroomLight2, Lights.CraftRoomLights], Sensors.MainBedroom4in1Motion, TimeSpan.FromMinutes(10))
                .WithMeta("mainbedroom off on no motion","10 minutes")           
        );
    }
}
