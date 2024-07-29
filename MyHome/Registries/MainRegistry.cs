using HaKafkaNet;

namespace MyHome;

public class MainRegistry : IAutomationRegistry
{
    readonly IHaServices _services;
    readonly IAutomationFactory _factory;
    readonly IAutomationBuilder _builder;

    readonly ILivingRoomService _livingRoomService;
    readonly INotificationService _notificationService;
    readonly NotificationSender _notifyKitchenLivingRoom;
    readonly NotificationSenderNoText _notifyPressure;

    public MainRegistry(IHaServices services, IAutomationFactory factory, IAutomationBuilder builder, ILivingRoomService livingRoomService, INotificationService notificationService)
    {
        _services = services;
        _factory = factory;
        _builder = builder;
        _livingRoomService = livingRoomService;
        _notificationService = notificationService;

        var channel = notificationService.CreateAudibleChannel([MediaPlayers.Kitchen, MediaPlayers.LivingRoom]);
        _notifyKitchenLivingRoom = notificationService.CreateNotificationSender([channel]);

        _notifyPressure = _notificationService.CreateNoTextNotificationSender([_notificationService.CreateMonkeyChannel(new()
        {
            EntityId = [Lights.Monkey],
            ColorName = "darkslateblue",
            Brightness = Bytes._50pct,
        })]);
        
    }

    public void Register(IRegistrar reg)
    {        
        // lights auto off
        reg.RegisterMultiple(
            _factory.DurableAutoOff("switch.back_hall_light", TimeSpan.FromMinutes(10)).WithMeta("auto off back hall","10 min"),
            _factory.DurableAutoOff("light.upstairs_hall", TimeSpan.FromMinutes(30)).WithMeta("auto off upstairs hall","30 min"),
            _factory.DurableAutoOff("light.entry_light", TimeSpan.FromMinutes(30)).WithMeta("auto off entry light","30 min"),
            _factory.DurableAutoOffOnEntityOff([Lights.MainBedroomLight1, Lights.MainBedroomLight2, Lights.CraftRoomLights], Sensors.MainBedroom4in1Motion, TimeSpan.FromMinutes(10))
                .WithMeta("mainbedroom off on no motion","10 minutes")           
        );

        //brush lyra hair
        reg.Register(_builder.CreateSimple()
            .WithName("Lyra Brush Hair")
            .WithTriggers("binary_sensor.lyra_brush_hair")
            .WithExecution((sc, ct) => {
                _notifyKitchenLivingRoom("Time to brush Lyra's hair");
                return Task.CompletedTask;
            })
            .Build());
        
        // make sure switches are off
        reg.Register(
            _factory.SimpleAutomation([GarageService.GARAGE1_DOOR_OPENER, GarageService.GARAGE2_DOOR_OPENER],
                (sc, ct) => sc.ToOnOff().New.State == OnOff.On 
                    ? _services.Api.TurnOff(sc.EntityId)
                    : Task.CompletedTask).WithMeta("Garage Door switches auto-off")
            .WithMeta("Garage door swtiches","turn them off immediately")
        );

        reg.Register(_builder.CreateSimple()
            .WithName("Rachel Phone Battery")
            .WithDescription("Alert when her battery is low")
            .WithTriggers(Helpers.RachelPhoneBatteryHelper)
            .WithExecution(async (sc, ct) =>
            {
                var onOff = sc.ToOnOff();
                if (onOff.IsOn())
                {
                    var batteryState = await _services.EntityProvider.GetBatteryStateEntity("sensor.rachel_phone_battery_state");
                    if (batteryState?.State != BatteryState.Charging)
                    {
                        await _notifyKitchenLivingRoom("Rachel, your phone battery is low");
                    }
                }
            })
            .Build());

        reg.Register(_builder.CreateSimple(false)
            .WithName("Person arriving home")
            .WithTriggers("person.leonard", "person.rachel")
            .WithExecution(async (sc, ct) =>{
                var person = sc.ToPerson();
                if (person.CameHome())
                {
                    await _notifyKitchenLivingRoom($"{person?.New?.Attributes?.FriendlyName ?? "person"} is arriving home");
                }
            })
            .Build()
        );

        reg.Register(_builder.CreateSimple()
            .WithName("Living Room - Set lights when override disabled")
            .WithDescription("When the living room override is turned off, set the lights based on power reading")
            .WithTriggers(Helpers.LivingRoomOverride)
            .WithExecution((sc, ct) =>{
                var onOff = sc.ToOnOff();
                return sc.ToOnOff().IsOff() switch
                {
                    true => _livingRoomService.SetLightsBasedOnPower(),
                    _ => Task.CompletedTask
                };
            })
            .Build());

        reg.Register(_builder.CreateSimple()
            .WithName("Barometric Pressure Alert")
            .WithDescription("Notify when pressure drops by 0.04 over 4 hours")
            .WithTriggers("sensor.pressure_change_4_hr")
            .WithExecution(async (sc, ct) =>{
                var pressure = sc.ToFloatTyped();
                if (pressure.BecameLessThanOrEqual(-0.04f))
                {
                    await _notifyPressure();
                }
            })
            .Build());
    }


}
