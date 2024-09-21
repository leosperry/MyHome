using System.Diagnostics;
using System.Text.Json;
using HaKafkaNet;
using MyHome.Models;

namespace MyHome;

public class DiningRoomButtons : IAutomation, IAutomationMeta
{
    readonly IHaServices _services;
    readonly INotificationService _notificationService;
    readonly ILogger _logger;
    public DiningRoomButtons(IHaServices servcies, INotificationService notificationService, ILogger<DiningRoomButtons> logger)
    {
        _services = servcies;
        _notificationService = notificationService;
        _logger = logger;
    }

    public Task Execute(HaEntityStateChange stateChange, CancellationToken ct)
    {
        if (stateChange.EntityId == Helpers.LivingRoomOverride)
        {
            return SetHelperState((HaEntityState<OnOff, JsonElement>)stateChange.New, ct);
        }
        
        var sceneState = stateChange.ToSceneControllerEvent();

        if(!sceneState.New.StateAndLastUpdatedWithin1Second())
        {
            _logger.LogWarning("Exiting. Not within 1 second");
            return Task.CompletedTask;
        }
        
        var btn = stateChange.EntityId.Last();
        var press = sceneState?.New.Attributes?.GetKeyPress();
        return (btn, press) switch
        {
            {btn: '1', press: KeyPress.KeyPressed} => _services.Api.Toggle(Helpers.LivingRoomOverride, ct),
            {btn: '3', press: KeyPress.KeyPressed} => _notificationService.ClearAll(),
            {btn: '4', press: KeyPress.KeyPressed} => AsherButton(0.15f, ct),
            {btn: '4', press: KeyPress.KeyPressed2x} => AsherButton(0.20f, ct),
            {btn: '4', press: KeyPress.KeyPressed3x} => AsherButton(0.25f, ct),
            {btn: '4', press: KeyPress.KeyPressed4x} => AsherButton(0.30f, ct),
            {btn: '4', press: KeyPress.KeyPressed5x} => AsherButton(0.35f, ct),
            _ => Task.CompletedTask
        };
    }

    AutomationMetaData _meta = new()
    {
        Name = "Dining Room Buttons",
        Description = "Living Room Override, Clear notifications, Asher button",
    };

    public AutomationMetaData GetMetaData() => _meta;

    public IEnumerable<string> TriggerEntityIds()
    {
        yield return Helpers.LivingRoomOverride;
        yield return "event.dining_room_scene_001";
        //yield return "event.dining_room_scene_002";
        yield return "event.dining_room_scene_003";
        yield return "event.dining_room_scene_004";
        //yield return "event.dining_room_scene_005";
    }

    async Task AsherButton(float targetVolume ,CancellationToken ct)
    {
        if (!shouldPlay())
        {
            return;
        }
        /* 
        Step 1
            Play random
            Set 1 light on and 1 off
        Step 2
            repeat 4 times
            toggle lights
        Step 3
            set lights to 40%*/
        _ = PlayRandom(targetVolume, ct);
        await Task.WhenAll(
            _services.Api.TurnOn(Lights.Basement1, ct),
            _services.Api.TurnOff(Lights.Basement2, ct)
        );

        int count = 0;
        while(++count <= 4)
        {
            await Task.WhenAll(
                Task.Delay(3000, ct),
                _services.Api.Toggle([Lights.Basement1, Lights.Basement2], ct)
            );
        }
        await _services.Api.LightSetBrightness([Lights.Basement1, Lights.Basement2], Bytes._40pct, ct);
    }

    static readonly string[] _messages = [ 
            "My lord. Your presence is requested in the main chamber",
            "Prince Asher, the king and queen have summoned you",
            "Hey you. Yes, you. Please come upstairs",
            "The parental units have requested of the carbon based lifeform known as Asher to vacate his domicile and return upstairs",
            "The crown has requested a status report from the dungeons forthwith",
            "Your assignment is as follows, collect dishes and return to base. This message will self destruct",
            "Brave knight. You have been given a valiant quest to return to the overworld",
            "Random message to see if you're paying attention" ];
    
    static readonly PiperSettings[] _voices = [Voices.Buttler, Voices.Female, Voices.Mundane];
    static readonly Random _random = new();
    async Task PlayRandom(float targetVolume, CancellationToken ct)
    {
        // get a message
        var message = _messages[_random.Next(0, _messages.Length)];
        var voice = _voices[_random.Next(0, _voices.Length)];

        // get the volume
        var playerState = await _services.EntityProvider.GetMediaPlayer<SonosAttributes>(MediaPlayers.Asher);

        if (!playerState.Bad())
        {
            float? previousVolume = playerState.Attributes?.VolumeLevel;
            if (previousVolume < targetVolume)
            {
                await _services.Api.MediaPlayerSetVolume(MediaPlayers.Asher, targetVolume);
            }

            await _services.Api.SpeakPiper(MediaPlayers.Asher, message, false, voice);

            if (previousVolume is not null && previousVolume != targetVolume)
            {
                await _services.Api.MediaPlayerSetVolume(MediaPlayers.Asher, previousVolume.Value);
            }
        }
        else
        {
            _logger.LogWarning("Asher speaker state is {AsherState}", playerState?.State.ToString() ?? "null" );
            await _services.Api.SpeakPiper(MediaPlayers.DiningRoom, "Something is wrong with Asher's speaker");
        }
    }

    static DateTime _lastPressed = DateTime.Now;
    static object _presslock = new {};
    static readonly TimeSpan _lockDelay = TimeSpan.FromSeconds(4);
    private bool shouldPlay()
    {
        var now = DateTime.Now;
        lock (_presslock)
        {
            var diff = now - _lastPressed;
            _lastPressed = now;

            return diff > _lockDelay;
        }
    }

    Task SetHelperState(HaEntityState<OnOff, JsonElement>? helperState, CancellationToken ct)
    {
        (ZoozColor color, int parameter) settings = helperState switch
        {
            {State : OnOff.On, EntityId : Helpers.LivingRoomOverride} => (ZoozColor.Yellow, 7),
            {State : OnOff.Off, EntityId : Helpers.LivingRoomOverride} => (ZoozColor.Cyan, 7),
            _ => throw new Exception("unknown setup for dining room LED status indicators")
        };

        return _services.Api.ZwaveJs_SetConfigParameter(new{
            entity_id = Lights.DiningRoomLights,
            endpoint = 0,
            settings.parameter,
            value = (int)settings.color
        }, ct);
    }

    enum ZoozColor
    {
        White = 0,
        Blue = 1,
        Green = 2,
        Red = 3, 
        Magenta = 4,
        Yellow = 5,
        Cyan = 6
    }
}
