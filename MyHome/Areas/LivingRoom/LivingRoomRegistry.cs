using HaKafkaNet;

namespace MyHome;

public class LivingRoomRegistry : IAutomationRegistry
{
    readonly IAutomationFactory _factory;
    readonly IAutomationBuilder _builder;
    readonly IHaServices _services;
    readonly LightAlertModule _lam;
    readonly ILivingRoomService _livingRoomService;
    readonly ILogger<LivingRoomRegistry> _logger;
    static readonly TimeSpan four_hours = TimeSpan.FromHours(4);

    public LivingRoomRegistry(IAutomationFactory factory, IAutomationBuilder builder, IHaServices services, LightAlertModule lam, ILivingRoomService livingRoomService, ILogger<LivingRoomRegistry> logger)
    {
        _factory = factory;
        _builder = builder;
        _services = services;
        _lam = lam;
        _livingRoomService = livingRoomService;
        _logger = logger;
    }

    public void Register(IRegistrar reg)
    {
        reg.RegisterMultiple(
            Sunrise(),
            SunDusk(),
            Sunset()
        );

        reg.RegisterMultiple(
            SomeoneInLivingRoom(),
            OverrideTurnedOff_SetLights()
        );

        reg.RegisterMultiple(
            NoOneDownstairs_for30min_PauseRoku(), 
            RokuPaused_for15min_TurnItOff(),
            NoOneInLivingRoom_for5min_TurnOffLights()
        );
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
            .WithExecution((sc, ct) =>{
                var onOff = sc.ToOnOff();
                return sc.ToOnOff().IsOff() switch
                {
                    true => _livingRoomService.SetLightsBasedOnPower(),
                    _ => Task.CompletedTask
                };
            })
            .Build();
    }

    ISchedulableAutomation NoOneInLivingRoom_for5min_TurnOffLights()
    {
        var downstairsLightDelayMinutes = 5;

        return _builder.CreateSchedulable()
            .MakeDurable()
            .WithName($"Living Room and Kitchen not occupied for {downstairsLightDelayMinutes} minutes")
            .WithDescription("Turn Off living room lights")
            .WithTriggers(Sensors.LivingRoomPresence) // helper entitiy combining kitchen and living room
            .GetNextScheduled((sc, ct) => {
                DateTime? time = default;
                if (sc.ToOnOff().New.State == OnOff.Off)
                {
                    time = DateTime.Now.AddMinutes(downstairsLightDelayMinutes);
                }
                return Task.FromResult(time);
            })
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

    ISchedulableAutomation NoOneDownstairs_for30min_PauseRoku()
    {
        return _builder.CreateSchedulable()
            .MakeDurable()
            .WithName("Living Room All zones empty for 30 min")
            .WithDescription("pause the roku and turn off dining room")
            .WithTriggers(Sensors.LivingRoomAndKitchenPresenceCount)
            .GetNextScheduled((sc, ct) => {
                var zoneCount = sc.ToOnOff();
                if (zoneCount.New.IsOff())
                {
                    return Task.FromResult<DateTime?>(DateTime.Now.AddMinutes(30));
                }
                return Task.FromResult<DateTime?>(null);
            })
            .WithExecution(async ct => {
                var roku = await _services.EntityProvider.GetEntity(MediaPlayers.Roku, ct);

                if(roku!.State == "playing")
                {
                    await RokuCommand(RokuCommands.play);
                }
                await _services.Api.TurnOff(Lights.DiningRoomLights); 
            })
            .Build();
    }
    
    ISchedulableAutomation RokuPaused_for15min_TurnItOff()
    {
        var statesToLeaveAlone = new HashSet<string>(["playing","standby","idle"]);
        return _builder.CreateSchedulable()
            .MakeDurable()
            .WithName("Roku Paused")
            .WithTriggers(MediaPlayers.Roku)
            .WithDescription("If Roku is not playing for 15 minutes, turn it off")
            .GetNextScheduled((sc, ct) => {
                if (!statesToLeaveAlone.Contains(sc.New.State))
                {
                    return Task.FromResult<DateTime?>(DateTime.Now.AddMinutes(15));
                }
                return Task.FromResult<DateTime?>(null);
            })
            .WithExecution(ct => {
                return RokuCommand(RokuCommands.power);
            })
            .Build();
    }

    IAutomation SomeoneInLivingRoom()
    {
        return _builder.CreateSimple()
            .WithName("Living Room Zone 1")
            .WithDescription("sets the Monkey light standby brightness")
            .WithTriggers(Sensors.LivingRoomZone1Count)
            .WithExecution(async (sc, ct) => {
                var zone = sc.ToFloatTyped();
                if (zone.BecameGreaterThan(0))
                {
                    await _livingRoomService.SetLightsBasedOnPower();

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
