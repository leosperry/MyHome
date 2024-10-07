using System.Text.Json;
using HaKafkaNet;

namespace MyHome;

public class LivingRoomRegistry : IAutomationRegistry
{
    readonly IAutomationFactory _factory;
    readonly IAutomationBuilder _builder;
    readonly IHaServices _services;
    readonly LightAlertModule _lam;
    readonly LivingRoomService _livingRoomService;
    readonly ILogger<LivingRoomRegistry> _logger;
    private readonly IHaEntity<MediaPlayerState, JsonElement> _rokuMediaPlayer;
    static readonly TimeSpan four_hours = TimeSpan.FromHours(4);

    public LivingRoomRegistry(IAutomationFactory factory, IAutomationBuilder builder, IHaServices services, LightAlertModule lam, LivingRoomService livingRoomService, IUpdatingEntityProvider updatingEntityProvider, ILogger<LivingRoomRegistry> logger)
    {
        _factory = factory;
        _builder = builder;
        _services = services;
        _lam = lam;
        _livingRoomService = livingRoomService;
        _logger = logger;

        _rokuMediaPlayer = updatingEntityProvider.GetMediaPlayer(MediaPlayers.Roku);
    }

    public void Register(IRegistrar reg)
    {
        reg.RegisterMultiple(
            Sunrise(),
            SunDusk(),
            Sunset()
        );

        reg.RegisterMultiple(
            SolarPowerChange(),
            SomeoneInLivingRoom(),
            OverrideTurnedOff_SetLights()
        );

        reg.RegisterMultiple(
            NoOneDownstairs_for20min_PauseRoku(), 
            NoOneInLivingRoom_for5min_TurnOffLights()
        );
    }

    private IAutomation SolarPowerChange()
    {
        return _builder.CreateSimple()
            .WithTriggers("sensor.solaredge_current_power")
            .WithName("Living Room lights")
            .WithDescription("Set living room lights based on solar power")
            .WithExecution((sc, ct) => _livingRoomService.SetLights(ct))
            .Build();
    }

    Task RokuCommand(RokuCommands command)
    {
        _logger.LogWarning("{command} Remote command sent to roku", command);
        return _services.Api.RemoteSendCommand(Devices.Roku, command.ToString());
    }

    IAutomation OverrideTurnedOff_SetLights()
    {
        return _builder.CreateSimple()
            .WithName("Living Room - Set lights when override disabled")
            .WithDescription("When the living room override is turned off, set the lights based on power reading")
            .WithTriggers(Helpers.LivingRoomOverride)
            .WithExecution((sc, ct) => _livingRoomService.SetLights(ct))
            .Build();
    }

    ISchedulableAutomation NoOneInLivingRoom_for5min_TurnOffLights()
    {
        var downstairsLightDelayMinutes = 10;

        return _builder.CreateSchedulable()
            .MakeDurable()
            .WithName("Living Room and Kitchen not occupied")
            .WithDescription($"After {downstairsLightDelayMinutes} min, Turn Off living room lights")
            .WithTriggers(Sensors.LivingRoomPresence)
            .While(sc => sc.ToOnOff().New.State == OnOff.Off)
            .For(TimeSpan.FromMinutes(downstairsLightDelayMinutes))
            .WithExecution(ct => {
                _lam.ConfigureStandByBrightness(0);
                return Task.WhenAll(
                    _services.Api.TurnOff([
                        Lights.TvBacklight, Lights.Couch1, Lights.Couch2, Lights.PeacockLamp
                        // leave on Couch3 for nightlight
                        ])
                );
            })
            .Build();
    }

    ISchedulableAutomation NoOneDownstairs_for20min_PauseRoku()
    {
        return _builder.CreateSchedulable()
            .MakeDurable()
            .WithName("Living Room All zones empty")
            .WithDescription("after 30 min, pause the roku and turn off dining room")
            .WithTriggers(Sensors.LivingRoomAndKitchenPresenceCount)
            .While(sc => sc.ToFloatTyped().New.State == 0)
            .For(TimeSpan.FromMinutes(20))
            .WithExecution(async ct => {
                if(_rokuMediaPlayer.State == MediaPlayerState.Playing)
                {
                    await RokuCommand(RokuCommands.play);
                }
                await _services.Api.TurnOff(Lights.DiningRoomLights); 
            })
            .Build();
    }

    IAutomation SomeoneInLivingRoom()
    {
        return _builder.CreateSimple()
            .WithName("Enter Living Room Zone 1")
            .WithDescription("sets the Monkey light standby brightness")
            .WithTriggers(Sensors.LivingRoomZone1Count)
            .WithExecution(async (sc, ct) => {
                var zone = sc.ToFloatTyped();

                bool occupied = zone.BecameGreaterThan(0);

                if (occupied)
                {
                    await _livingRoomService.SetLights(ct);
                    // this code is to set the monkeylight
                    var sun = await _services.EntityProvider.GetSun();
                    // figure out what it should be 
                    // at sunrise   9%
                    // at sunset    6%
                    // at dusk      3%
                    if (sun!.State == SunState.Above_Horizon)
                    {
                        _lam.ConfigureStandByBrightness(Bytes.PercentToByte(9));
                    }
                    else if (sun!.Attributes!.NextDusk - DateTime.Now > four_hours)
                    {
                        // sun has set
                        // if dusk has passed, next dusk will be big
                        _lam.ConfigureStandByBrightness(Bytes.PercentToByte(3));
                    }
                    else
                    {
                        _lam.ConfigureStandByBrightness(Bytes.PercentToByte(6));
                    }
                }
            })
            .Build();
    }

    ISchedulableAutomation Sunrise()
    {
        return _factory.SunRiseAutomation(async ct => { 
            // turn off night light.
            await _services.Api.TurnOff([Lights.Couch1], ct);
            _lam.ConfigureStandByBrightness(Bytes.PercentToByte(9));
            })
        .WithMeta("Sunrise", "turn off couch 1");
    }

    ISchedulableAutomation Sunset()
    {
        return _factory.SunSetAutomation(async ct => {
            var zone1 = await _services.EntityProvider.GetIntegerEntity(Sensors.LivingRoomZone1Count);
            if (zone1!.State > 0)
            {
                _lam.ConfigureStandByBrightness(Bytes.PercentToByte(6));
            }
        }).WithMeta("Sunset", "dim monkey standby");
    }

    ISchedulableAutomation SunDusk()
    {
        return _factory.SunDuskAutomation(async ct => {
            var zone1 = await _services.EntityProvider.GetIntegerEntity(Sensors.LivingRoomZone1Count);
            if (zone1!.State > 0)
            {
                _lam.ConfigureStandByBrightness(Bytes.PercentToByte(3));
            }            
        }).WithMeta("Dusk", "dim monkey standby");
    }

}
