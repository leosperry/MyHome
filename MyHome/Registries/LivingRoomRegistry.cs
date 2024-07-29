using HaKafkaNet;

namespace MyHome;

public class LivingRoomRegistry : IAutomationRegistry
{
    readonly IAutomationFactory _factory;
    readonly IAutomationBuilder _builder;
    readonly IHaServices _services;
    readonly LightAlertModule _lam;

    static readonly TimeSpan four_hours = TimeSpan.FromHours(4);

    public LivingRoomRegistry(IAutomationFactory factory, IAutomationBuilder builder, IHaServices services, LightAlertModule lam)
    {
        _factory = factory;
        _builder = builder;
        _services = services;
        _lam = lam;
    }

    public void Register(IRegistrar reg)
    {
        var downstairsLightDelayMinutes = 5;
        var livingAndKitchenNoPresence = _builder.CreateSchedulable()
            .MakeDurable()
            .WithName($"Living Room and Kitchen not occupied for {downstairsLightDelayMinutes} minutes")
            .WithDescription("Turn Off living room lights and turn on living room override")
            .WithTriggers(Sensors.LivingRoomAndKitchenPresenceCount) // helper entitiy combining kitchen and living room
            .GetNextScheduled((sc, ct) => {
                DateTime? time = default;
                if (sc.ToFloatTyped().New.State == 0)
                {
                    time = DateTime.Now.AddMinutes(downstairsLightDelayMinutes);
                }
                return Task.FromResult(time);
            })
            .WithExecution(ct => {
                return Task.WhenAll(
                    _services.Api.TurnOn(Helpers.LivingRoomOverride),
                    _services.Api.TurnOff([Lights.TvBacklight, Lights.Couch1, Lights.Couch2, 
                    Lights.DiningRoomLights, Lights.KitchenLights])
                );
            })
            .Build();
        
        var peopleDetectedTurnOffLivingRoomOverride = _builder.CreateSimple()
            .WithName("People entered Downstairs")
            .WithDescription("Turn off the living room override. This will trigger auto setting of lights")
            .WithTriggers(Sensors.LivingRoomAndKitchenPresenceCount)
            .WithExecution(async (sc, ct) => {
                var count = sc.ToFloatTyped();
                if (count.BecameGreaterThan(0))
                {
                    await _services.Api.TurnOff(Helpers.LivingRoomOverride);
                }
            })
            .Build();

        var livingroomAllZoneExit = _builder.CreateSchedulable()
            .MakeDurable()
            .WithName("Living Room All zones empty")
            .WithDescription("pause the roku")
            .WithTriggers(Sensors.LivingRoomPresence)
            .GetNextScheduled((sc, ct) => {
                var zoneCount = sc.ToOnOff();
                if (zoneCount.New.IsOff())
                {
                    return Task.FromResult<DateTime?>(DateTime.Now.AddMinutes(30));
                }
                return Task.FromResult<DateTime?>(null);
            })
            .WithExecution(async ct => {
                var roku = await _services.EntityProvider.GetEntity(MediaPlayers.Roku,ct);
                if (roku.Bad())
                {
                    await _services.Api.NotifyGroupOrDevice(Phones.LeonardPhone, "roku offline again");
                }
                else if(roku!.State == "playing")
                {
                    await _services.Api.RemoteSendCommand(Devices.Roku, RokuCommands.play.ToString());
                }                
            })
            .Build();

        var statesToLeaveAlone = new HashSet<string>(["playing","standby","idle"]);
        var rokuPaused = _builder.CreateSchedulable()
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
                return _services.Api.RemoteSendCommand(Devices.Roku, RokuCommands.power.ToString());
            })
            .Build();


        var livingRoomZone1Exit =_builder.CreateSchedulable()
            .MakeDurable()
            .WithName("Living Room Zone 1 Exit")
            .WithDescription("Turn off TV when unoccupied")
            .WithTriggers(Sensors.LivingRoomZone1AllCount)
            .WithAdditionalEntitiesToTrack(Devices.Roku)
            .MakeDurable()
            .GetNextScheduled((sc, ct) => {
                var zoneCount = sc.ToIntTyped();
                if (zoneCount.New.State == 0)
                {
                    return Task.FromResult<DateTime?>(DateTime.Now.AddMinutes(20));
                }
                return Task.FromResult<DateTime?>(null);
            })
            .WithExecution(cd => {
                //await _services.Api.TurnOff(Devices.Roku);
                _lam.ConfigureStandByBrightness(0);
                return Task.CompletedTask;
            })
            .Build();
        
        var livingRoomZone1Enter = _builder.CreateSimple()
            .WithName("Living Room Zone 1")
            .WithTriggers(Sensors.LivingRoomZone1AllCount)
            .WithExecution(async (sc, ct) => {
                var zone = sc.ToIntTyped();
                if (zone.BecameGreaterThan(0))
                {
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

        reg.RegisterMultiple(
            _factory.SunRiseAutomation(this.Sunrise).WithMeta("Sunrise", "turn off couch 1"),
            _factory.SunRiseAutomation(
                async ct => {
                    //await _services.Api.TurnOff(Helpers.LivingRoomOverride, ct);
                    
                    var zone1 = await _services.EntityProvider.GetIntegerEntity(Sensors.LivingRoomZone1AllCount);
                    if (zone1!.State > 0)
                    {
                        _lam.ConfigureStandByBrightness(Bytes.PercentToByte(9));
                    }
                }, 
                TimeSpan.FromHours(1))
                .WithMeta("re-enable living room lights","1 hour after sunrise, monkey standby brighter")
        );

        reg.Register(_factory.SunSetAutomation(async ct => {
            var zone1 = await _services.EntityProvider.GetIntegerEntity(Sensors.LivingRoomZone1AllCount);
            if (zone1!.State > 0)
            {
                _lam.ConfigureStandByBrightness(Bytes.PercentToByte(6));
            }
        }).WithMeta("Sunset", "dim monkey standby"));

        reg.Register(_factory.SunDuskAutomation(async ct => {
            var zone1 = await _services.EntityProvider.GetIntegerEntity(Sensors.LivingRoomZone1AllCount);
            if (zone1!.State > 0)
            {
                _lam.ConfigureStandByBrightness(Bytes.PercentToByte(3));
            }            
        }).WithMeta("Dusk", "dim monkey standby"));


        reg.RegisterMultiple(livingRoomZone1Enter, peopleDetectedTurnOffLivingRoomOverride);
        reg.RegisterMultiple(livingRoomZone1Exit, livingroomAllZoneExit, rokuPaused, livingAndKitchenNoPresence);        
    }

    Task Sunrise(CancellationToken ct) => _services.Api.TurnOff([Lights.Couch1], ct);
}
