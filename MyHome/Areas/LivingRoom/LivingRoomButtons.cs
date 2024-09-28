
using HaKafkaNet;

namespace MyHome;

public class LivingRoomButtons : IAutomation, IAutomationMeta
{
    readonly IHaServices _services;
    readonly INotificationService _notificationService;
    readonly ILogger _logger;
    
    LightTurnOnModel _tvBacklightPresets = new LightTurnOnModel()
    {
        EntityId = [Lights.TvBacklight],
        Brightness = 153,
        Kelvin = 2202
    };

    LightTurnOnModel _overheadPresets = new LightTurnOnModel()
    {
        EntityId = [Lights.CounchOverhead],
        Brightness = 64,
        RgbColor = (255, 146, 39)
    };

    public LivingRoomButtons(IHaServices services, INotificationService notificationService, ILogger<LivingRoomButtons> logger)
    {
        _services = services;
        _notificationService = notificationService;
        _logger = logger;
    }

    public Task Execute(HaEntityStateChange stateChange, CancellationToken ct)
    {
        var sceneEvent = stateChange.ToSceneControllerEvent();
        
        if(!sceneEvent.New.StateAndLastUpdatedWithin1Second()) return Task.CompletedTask;

        var button = sceneEvent.EntityId.Last();
        var keypress = sceneEvent.New.Attributes?.GetKeyPress();
        return (button,keypress) switch 
        {
            {button: '1', keypress: KeyPress.KeyPressed}    => ToggleLight(Lights.TvBacklight, ct),
            {button: '1', keypress: KeyPress.KeyHeldDown}   => IncreaseBrightness(Lights.TvBacklight, ct),
            {button: '1', keypress: KeyPress.KeyPressed2x}  => ReduceBrightness(Lights.TvBacklight, ct),
            {button: '2', keypress: KeyPress.KeyPressed}    => ToggleLight(Lights.CounchOverhead, ct),
            {button: '2', keypress: KeyPress.KeyHeldDown}   => IncreaseBrightness(Lights.CounchOverhead, ct),
            {button: '2', keypress: KeyPress.KeyPressed2x}  => ReduceBrightness(Lights.CounchOverhead, ct),
            {button: '3', keypress: KeyPress.KeyPressed}    => _services.Api.TurnOff([Lights.KitchenLights, Lights.DiningRoomLights, Lights.FrontRoomLight, Lights.BackHallLight],ct),
            {button: '4', keypress: KeyPress.KeyPressed2x}  => RokuCommand(RokuCommands.find_remote),
            {button: '4', keypress: KeyPress.KeyPressed}    => RokuCommand(RokuCommands.play),
            {button: '5', keypress: KeyPress.KeyPressed}    => _services.Api.Toggle(Lights.PeacockLamp, ct),
            {button: '6', keypress: KeyPress.KeyPressed}    => ClearAllNotifications(),
            {button: '8', keypress: KeyPress.KeyPressed}    => RokuCommand(RokuCommands.select),
            _ => Task.CompletedTask //unassigned 
        };    
    }

    Task RokuCommand(RokuCommands command)
    {
        _logger.LogWarning("{command} Remote command sent to roku", command);
        return _services.Api.RemoteSendCommand(Devices.Roku, command.ToString());
    }

    Task ClearAllNotifications()
    {
        _notificationService.ClearAll();
        return Task.CompletedTask;
    }

    private async Task ToggleLight(string lightId, CancellationToken ct)
    {
        var light = await _services.EntityProvider.GetColorLightEntity(lightId, ct);
        if (light.Bad())
        {
            await _services.Api.PersistentNotification($"{lightId} is in a bad state", ct);
        }

        if (light?.State == OnOff.On)
        {
            await _services.Api.TurnOff(lightId);
        }
        else
        {
            switch (lightId)
            {
                case Lights.TvBacklight:
                    await _services.Api.LightTurnOn(_tvBacklightPresets, ct);
                    break;
                case Lights.CounchOverhead:
                    await _services.Api.LightTurnOn(_overheadPresets, ct);
                    break;
                default:
                    await _services.Api.TurnOn(lightId);
                    break;
            }
        }
    }

    private Task IncreaseBrightness(string lightId, CancellationToken ct)
    {
        LightTurnOnModel settings = new LightTurnOnModel()
        {
            EntityId = [lightId],
            BrightnessStepPct = 15
        };
        return _services.Api.LightTurnOn(settings);
    }

    private Task ReduceBrightness(string lightId, CancellationToken ct)
    {
        LightTurnOnModel settings = new LightTurnOnModel()
        {
            EntityId = [lightId],
            BrightnessStepPct = -25
        };
        return _services.Api.LightTurnOn(settings);
    }

    public AutomationMetaData GetMetaData()
    {
        return new()
        {
            Name = "Living Room Buttons"
        };
    }

    public IEnumerable<string> TriggerEntityIds()
    {
        yield return "event.living_room_buttons_scene_001";
        yield return "event.living_room_buttons_scene_002";
        yield return "event.living_room_buttons_scene_002";
        yield return "event.living_room_buttons_scene_003";
        yield return "event.living_room_buttons_scene_004";
        yield return "event.living_room_buttons_scene_005";
        yield return "event.living_room_buttons_scene_006";
        //yield return "event.living_room_buttons_scene_007";
        yield return "event.living_room_buttons_scene_008";
    }
}
