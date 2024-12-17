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

    public LivingRoomRegistry(IStartupHelpers helpers, IHaServices services, LightAlertModule lam, LivingRoomService livingRoomService, ILogger<LivingRoomRegistry> logger)
    {
        _factory = helpers.Factory;
        _builder = helpers.Builder;
        _services = services;
        _lam = lam;
        _livingRoomService = livingRoomService;
        _logger = logger;

        _rokuMediaPlayer = helpers.UpdatingEntityProvider.GetMediaPlayer(Media_Player.RokuUltra);
    }

    public void Register(IRegistrar reg)
    {
        reg.TryRegister(
            Sunrise,
            SunDusk,
            Sunset,
            SolarPowerChange,
            OverrideTurnedOff_SetLights,
            SomeoneInLivingRoom,
            NoOneDownstairs_for20min_PauseRoku,
            NoOneInLivingRoom_forXmin_TurnOffLights,
            FindRokuFromDashboard
        );
    }

    private IAutomationBase FindRokuFromDashboard()
    {
        return _builder.CreateSimple<DateTime?>()
            .WithName("Find Roku Remote from dashboard")
            .WithTriggers(Input_Button.FindRokuRemote)
            .WithExecution(async (sc, ct) =>
            {
                if (sc.New.StateAndLastUpdatedWithin1Second())
                {
                    await _services.Api.RemoteSendCommand(Remote.RokuUltra, RokuCommands.find_remote.ToString());
                }
            })
            .Build();
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
        return _services.Api.RemoteSendCommand(Remote.RokuUltra, command.ToString());
    }

    IAutomation OverrideTurnedOff_SetLights()
    {
        return _builder.CreateSimple()
            .WithName("Living Room - Set lights when override disabled")
            .WithDescription("When the living room override is turned off, set the lights based on power reading")
            .WithTriggers(Input_Boolean.LivingRoomOverride)
            .WithExecution((sc, ct) => _livingRoomService.SetLights(ct))
            .Build();
    }

    IDelayableAutomation<OnOff, JsonElement> NoOneInLivingRoom_forXmin_TurnOffLights()
    {
        var downstairsLightDelayMinutes = 15;

        return _builder.CreateSchedulable<OnOff>()
            .MakeDurable()
            .WithName("Living Room and Kitchen not occupied")
            .WithDescription($"After {downstairsLightDelayMinutes} min, Turn Off living room lights")
            .WithTriggers(Binary_Sensor.EsphomeLivingRoomPresence) //esphome_living_room_presence
            .While(sc => sc.New.IsOff())
            .ForMinutes(downstairsLightDelayMinutes)
            .WithExecution(ct => {
                _lam.ConfigureStandByBrightness(0);
                return Task.WhenAll(
                    _services.Api.TurnOff([
                        Light.TvBacklight, Light.CouchOverhead, Switch.PeacockLamp
                        // leave on Couch3 for nightlight
                        ])
                );
            })
            .Build();
    }

    IDelayableAutomation<float, JsonElement> NoOneDownstairs_for20min_PauseRoku()
    {
        return _builder.CreateSchedulable<float>()
            .MakeDurable()
            .WithName("Living Room All zones empty")
            .WithDescription("after 30 min, pause the roku and turn off dining room")
            .WithTriggers(Sensor.Livingroomandkitchenpresencecount)
            .While(sc => sc.New.State == 0)
            .ForMinutes(20)
            .WithExecution(async ct => {
                if(_rokuMediaPlayer.State == MediaPlayerState.Playing)
                {
                    await RokuCommand(RokuCommands.play);
                }
                await _services.Api.TurnOff(Switch.DiningRoomLights); 
            })
            .Build();
    }

    IAutomation<float, JsonElement> SomeoneInLivingRoom()
    {
        return _builder.CreateSimple<float>()
            .WithName("Enter Living Room Zone 1")
            .WithDescription("sets the Monkey light standby brightness")
            .WithTriggers(Sensor.EsphomeLivingRoomZone1AllTargetCount)
            .WithExecution(async (zone, ct) => {
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
            await _services.Api.TurnOff([Light.CouchOverhead], ct);
            _lam.ConfigureStandByBrightness(Bytes.PercentToByte(9));
            })
        .WithMeta("Sunrise", "turn off couch 1");
    }

    ISchedulableAutomation Sunset()
    {
        return _factory.SunSetAutomation(async ct => {
            var zone1 = await _services.EntityProvider.GetIntegerEntity(Sensor.EsphomeLivingRoomZone1AllTargetCount);
            if (zone1!.State > 0)
            {
                _lam.ConfigureStandByBrightness(Bytes.PercentToByte(6));
            }
        }).WithMeta("Sunset", "dim monkey standby");
    }

    ISchedulableAutomation SunDusk()
    {
        return _factory.SunDuskAutomation(async ct => {
            var zone1 = await _services.EntityProvider.GetIntegerEntity(Sensor.EsphomeLivingRoomZone1AllTargetCount);
            if (zone1!.State > 0)
            {
                _lam.ConfigureStandByBrightness(Bytes.PercentToByte(3));
            }            
        }).WithMeta("Dusk", "dim monkey standby");
    }

}
