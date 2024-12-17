using HaKafkaNet;
using MyHome.Models;
using MyHome.People;

namespace MyHome;

public class DiningRoomButtons : IAutomation, IAutomationMeta
{
    readonly IHaServices _services;
    private readonly AsherService _asherService;
    readonly INotificationService _notificationService;
    readonly ILogger _logger;
    private readonly IHaEntity<MediaPlayerState, SonosAttributes> _asherMediaPlayer;
    private readonly NotificationSender _diningRoomChannel;

    public DiningRoomButtons(IHaServices servcies, AsherService asherService, INotificationService notificationService, IUpdatingEntityProvider updatingEntityProvider, ILogger<DiningRoomButtons> logger)
    {
        _services = servcies;
        _asherService = asherService;
        _notificationService = notificationService;
        _logger = logger;

        _asherMediaPlayer = updatingEntityProvider.GetMediaPlayer<SonosAttributes>(Media_Player.AsherRoomSpeaker);

        this._diningRoomChannel = _notificationService.CreateNotificationSender([
            _notificationService.CreateAudibleChannel([Media_Player.DiningRoomSpeaker])]
        );        
    }

    public Task Execute(HaEntityStateChange stateChange, CancellationToken ct)
    {
        if (stateChange.EntityId == Input_Boolean.LivingRoomOverride)
        {
            return _services.Api.SetZoozSceneControllerButtonColorFromOverrideState(
                Switch.DiningRoomLights,
                stateChange.ToOnOff().New.State, 1, ct);
            //return SetHelperState((HaEntityState<OnOff, JsonElement>)stateChange.New, ct);
        }
        
        var sceneState = stateChange.ToSceneControllerEvent();

        if(!sceneState.New.StateAndLastUpdatedWithin1Second())
        {
            return Task.CompletedTask;
        }
        
        var btn = stateChange.EntityId.Last();
        var press = sceneState?.New.Attributes?.GetKeyPress();
        return (btn, press) switch
        {
            {btn: '1', press: KeyPress.KeyPressed} => _services.Api.Toggle(Input_Boolean.LivingRoomOverride, ct),
            {btn: '3', press: KeyPress.KeyPressed} => _notificationService.ClearAll(),
            {btn: '4', press: KeyPress.KeyPressed} => AsherButton(0.15f, ct),
            {btn: '4', press: KeyPress.KeyPressed2x} => AsherButton(0.20f, ct),
            {btn: '4', press: KeyPress.KeyPressed3x} => AsherButton(0.25f, ct),
            {btn: '4', press: KeyPress.KeyPressed4x} => AsherButton(0.30f, ct),
            {btn: '4', press: KeyPress.KeyPressed5x} => AsherButton(0.35f, ct),
            _ => HandleNoMatch(btn, press)
        };
    }

    Task HandleNoMatch(char  button, KeyPress? press)
    {
        _logger.LogWarning("No match found for button {button} and press {press}", button, press.ToString() ?? "null");
        return Task.CompletedTask;
    }

    AutomationMetaData _meta = new()
    {
        Name = "Dining Room Buttons",
        Description = "Living Room Override, Clear notifications, Asher button",
    };

    public AutomationMetaData GetMetaData() => _meta;

    public IEnumerable<string> TriggerEntityIds()
    {
        yield return Input_Boolean.LivingRoomOverride;
        yield return "event.dining_room_lights_scene_001";
        yield return "event.dining_room_lights_scene_002";
        yield return "event.dining_room_lights_scene_003";
        yield return "event.dining_room_lights_scene_004";
        yield return "event.dining_room_lights_scene_005";
    }

    async Task AsherButton(float targetVolume ,CancellationToken ct)
    {
        if (!shouldPlay())
        {
            return;
        }

        await Task.WhenAll(
            _asherService.PlayRandom(targetVolume, ct),
            _asherService.Toggle3Times(ct)
        );
        
        await Task.WhenAll(
            _services.Api.TurnOn(Light.BasementLight1, ct),
            _services.Api.TurnOff(Light.BasementLight2, ct)
        );

        int count = 0;
        while(++count <= 4)
        {
            await Task.WhenAll(
                Task.Delay(3000, ct),
                _services.Api.Toggle([Light.BasementLight1, Light.BasementLight2], ct)
            );
        }
        await _services.Api.LightSetBrightness([Light.BasementLight1, Light.BasementLight2], Bytes._40pct, ct);
    }

    // static readonly string[] _messages = [ 
    //         "My lord. Your presence is requested in the main chamber",
    //         "Prince Asher, the king and queen have summoned you",
    //         "Hey you. Yes, you. Please come upstairs",
    //         "The parental units have requested of the carbon based lifeform known as Asher to vacate his domicile and return upstairs",
    //         "The crown has requested a status report from the dungeons forthwith",
    //         "Your assignment is as follows, collect dishes and return to base. This message will self destruct",
    //         "Brave knight. You have been given a valiant quest to return to the overworld",
    //         "Random message to see if you're paying attention" ];
    
    //static readonly PiperSettings[] _voices = [Voices.Buttler, Voices.Female, Voices.Mundane];

    //static readonly Random _random = new();
    // async Task PlayRandom(float targetVolume, CancellationToken ct)
    // {
    //     // get a message
    //     var message = _messages[_random.Next(0, _messages.Length)];
    //     var voice = _voices[_random.Next(0, _voices.Length)];

    //     // get the volume

    //     if (!_asherMediaPlayer.Bad())
    //     {
    //         float? previousVolume = _asherMediaPlayer.Attributes?.VolumeLevel;
    //         if (previousVolume < targetVolume)
    //         {
    //             await _services.Api.MediaPlayerSetVolume(MediaPlayers.Asher, targetVolume);
    //             await Task.Delay(TimeSpan.FromSeconds(2));
    //         }

    //         await _services.Api.SpeakPiper(MediaPlayers.Asher, message, true, voice);
    //         await Task.Delay(TimeSpan.FromSeconds(10));

    //         if (previousVolume is not null && previousVolume != targetVolume)
    //         {
    //             await _services.Api.MediaPlayerSetVolume(MediaPlayers.Asher, previousVolume.Value);
    //         }
    //     }
    //     else
    //     {
    //         _logger.LogWarning("Asher speaker state is {AsherState}", _asherMediaPlayer?.State.ToString() ?? "null" );
    //         await _diningRoomChannel("Something is wrong with Asher's speaker");
    //     }
    // }

    static DateTime _lastPressed = SystemMonitor.StartTime; // hack
    static object _presslock = new {};
    static readonly TimeSpan _lockDelay = TimeSpan.FromSeconds(4);
    private bool shouldPlay()
    {
        var now = DateTime.Now;
        lock (_presslock)
        {
            var diff = now - _lastPressed;
            _lastPressed = now;

            var retVal = diff > _lockDelay;

            if (!retVal)
            {
                _logger.LogWarning("Press Lock engaged - Now:{now} Last Pressed:{last_pressed} Diff: {diff}", now, _lastPressed, diff);
            }

            return retVal;
        }
    }

    // Task SetHelperState(HaEntityState<OnOff, JsonElement> helperState, CancellationToken ct)
    // {
    //     // var color = helperState switch
    //     // {
    //     //     {State : OnOff.On, EntityId : Helpers.LivingRoomOverride} => ZoozColor.Yellow,
    //     //     {State : OnOff.Off, EntityId : Helpers.LivingRoomOverride} => ZoozColor.Cyan,
    //     //     _ => ZoozColor.Red
    //     // };

    //     return _services.Api.SetZoozSceneControllerButtonColorFromToggleState(
    //         helperState, 1, ct);

    //     // return _services.Api.ZwaveJs_SetConfigParameter(new{
    //     //     entity_id = Lights.DiningRoomLights,
    //     //     endpoint = 0,
    //     //     settings.parameter,
    //     //     value = (int)settings.color
    //     // }, ct);
    // }

    
}
