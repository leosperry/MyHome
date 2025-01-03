using System;
using HaKafkaNet;
using HaKafkaNet.Implementations.AutomationBuilder;

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

        var channel = notificationService.CreateAudibleChannel([Media_Player.DiningRoomSpeaker]);
        //var channel = notificationService.CreateAudibleChannel([MediaPlayers.DiningRoom, MediaPlayers.MainBedroom]);
        _notifyDiningAndMainBedroom = notificationService.CreateNotificationSender([channel]);

        _notifyPressure = _notificationService.CreateNoTextNotificationSender([_notificationService.CreateMonkeyChannel(new()
        {
            EntityId = [Light.MonkeyLight],
            ColorName = "darkslateblue",
            Brightness = Bytes._50pct,
        })]);
    }

    public void Register(IRegistrar reg)
    {
        reg.TryRegister(
            BarometricPressureAlert,
            BrushLyraHair,
            LyraMedicine,
            RachelPhoneBattery
        );
    }

    IAutomation BarometricPressureAlert()
    {
        return _builder.CreateSimple()
            .WithName("Barometric Pressure Alert")
            .WithDescription("Notify when pressure drops by 0.04 over 4 hours")
            .WithTriggers(Sensor.PressureChange4Hr)
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
            .WithTriggers(Binary_Sensor.LyraBrushHair)
            .WithExecution((sc, ct) => {
                _notifyDiningAndMainBedroom("Time to brush Lyra's hair");
                return Task.CompletedTask;
            })
            .Build();
    }

    IAutomationBase LyraMedicine()
    {
        return _builder.CreateDailyFromTimeHelper(Input_Datetime.LyraMedicine)
            .WithName("Lyra Medicine")
            .WithExecution(async ct => 
                await _services.Api.SpeakPiper([
                    Media_Player.DiningRoomSpeaker
                ], "Time for Lyra medicine", true, Voices.Butler))
            .Build();
    }

    IAutomation RachelPhoneBattery()
    {
        return _builder.CreateSimple()
            .WithName("Rachel Phone Battery")
            .WithDescription("Alert when her battery is low")
            .WithTriggers(Binary_Sensor.Rachelphonebattlowhelper)
            .WithExecution(async (sc, ct) =>
            {
                var onOff = sc.ToOnOff();
                if (onOff.IsOn())
                {
                    var batteryState = await _services.EntityProvider.GetBatteryStateEntity(Sensor.RachelPhoneBatteryState);
                    if (batteryState?.State != BatteryState.Charging)
                    {
                        await _notifyDiningAndMainBedroom("Rachel, your phone battery is low");
                    }
                }
            })
            .Build();
    }
}
