using System;
using HaKafkaNet;

namespace MyHome.Registries;

public class RachelRegistry : IAutomationRegistry
{
    IAutomationBuilder _builder;
    IHaServices _services;
    readonly INotificationService _notificationService;
    readonly NotificationSender _notifyDiningAndMainBedroom;
    readonly NotificationSenderNoText _notifyPressure;
    
    public RachelRegistry(IAutomationBuilder builder, IHaServices services, INotificationService notificationService)
    {
        _builder = builder;
        _services = services;
        _notificationService = notificationService;

        var channel = notificationService.CreateAudibleChannel([MediaPlayers.DiningRoom]);
        //var channel = notificationService.CreateAudibleChannel([MediaPlayers.DiningRoom, MediaPlayers.MainBedroom]);
        _notifyDiningAndMainBedroom = notificationService.CreateNotificationSender([channel]);

        _notifyPressure = _notificationService.CreateNoTextNotificationSender([_notificationService.CreateMonkeyChannel(new()
        {
            EntityId = [Lights.Monkey],
            ColorName = "darkslateblue",
            Brightness = Bytes._50pct,
        })]);
    }

    public void Register(IRegistrar reg)
    {
        reg.Register(
            BarometricPressureAlert(),
            BrushLyraHair(),
            RachelPhoneBattery()
        );
    }

    IAutomation BarometricPressureAlert()
    {
        return _builder.CreateSimple()
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
            .Build();
    }

    IAutomation BrushLyraHair()
    {
        return _builder.CreateSimple()
            .WithName("Lyra Brush Hair")
            .WithTriggers("binary_sensor.lyra_brush_hair")
            .WithExecution((sc, ct) => {
                _notifyDiningAndMainBedroom("Time to brush Lyra's hair");
                return Task.CompletedTask;
            })
            .Build();
    }

    IAutomation RachelPhoneBattery()
    {
        return _builder.CreateSimple()
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
                        await _notifyDiningAndMainBedroom("Rachel, your phone battery is low");
                    }
                }
            })
            .Build();
    }


}
