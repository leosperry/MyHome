
using System.Text.Json;
using HaKafkaNet;

namespace MyHome;

public class LivingRoomButtons : IAutomation_SceneController, IAutomationMeta, IFallbackExecution
{
    readonly IHaServices _services;
    readonly INotificationService _notificationService;
    readonly ILogger _logger;
    private readonly IHaEntity<OnOff, JsonElement> _maintenanceMode;
    private readonly NotificationSender _diningRoomChannel;
    LightTurnOnModel _tvBacklightPresets = new LightTurnOnModel()
    {
        EntityId = [Light.TvBacklight],
        Brightness = 153,
        Kelvin = 2202
    };

    LightTurnOnModel _overheadPresets = new LightTurnOnModel()
    {
        EntityId = [Light.CouchOverhead],
        Brightness = 64,
        RgbColor = (255, 146, 39)
    };

    public LivingRoomButtons(IHaServices services, IStartupHelpers helpers, INotificationService notificationService, ILogger<LivingRoomButtons> logger)
    {
        _services = services;
        _notificationService = notificationService;
        _logger = logger;

        this._maintenanceMode = helpers.UpdatingEntityProvider.GetOnOffEntity(Input_Boolean.MaintenanceMode); 

        this._diningRoomChannel = _notificationService.CreateNotificationSender([
            _notificationService.CreateAudibleChannel([Media_Player.DiningRoomSpeaker])]
        );
    }

    public Task Execute(HaEntityStateChange<HaEntityState<DateTime?, SceneControllerEvent>> sceneEvent, CancellationToken ct)
    {
        if(!sceneEvent.New.StateAndLastUpdatedWithin1Second()) return Task.CompletedTask;

        var button = sceneEvent.EntityId.Last();
        var keypress = sceneEvent.New.Attributes?.GetKeyPress();
        return (button,keypress) switch 
        {
            {button: '1', keypress: KeyPress.KeyPressed}    => ToggleLight(Light.TvBacklight, ct),
            {button: '1', keypress: KeyPress.KeyHeldDown}   => IncreaseBrightness(Light.TvBacklight, ct),
            {button: '1', keypress: KeyPress.KeyPressed2x}  => ReduceBrightness(Light.TvBacklight, ct),
            {button: '2', keypress: KeyPress.KeyPressed}    => ToggleLight(Light.CouchOverhead, ct),
            {button: '2', keypress: KeyPress.KeyHeldDown}   => IncreaseBrightness(Light.CouchOverhead, ct),
            {button: '2', keypress: KeyPress.KeyPressed2x}  => ReduceBrightness(Light.CouchOverhead, ct),
            {button: '3', keypress: KeyPress.KeyPressed}    => _services.Api.TurnOff([Light.KitchenLights, Switch.DiningRoomLights, Light.FrontRoomLight, Switch.BackHallLight],ct),
            {button: '4', keypress: KeyPress.KeyPressed2x}  => RokuCommand(RokuCommands.find_remote),
            {button: '4', keypress: KeyPress.KeyPressed}    => RokuCommand(RokuCommands.play),
            {button: '5', keypress: KeyPress.KeyPressed}    => _services.Api.Toggle(Switch.PeacockLamp, ct),
            {button: '6', keypress: KeyPress.KeyPressed}    => ClearAllNotifications(),
            {button: '6', keypress: KeyPress.KeyPressed2x}    => ToggleMaintenanceMode(),
            {button: '8', keypress: KeyPress.KeyPressed}    => RokuCommand(RokuCommands.select),
            _ => Task.CompletedTask //unassigned 
        };    
    }

    public Task FallbackExecute(Exception ex, HaEntityStateChange stateChange, CancellationToken ct)
    {
        _logger.LogError("could not parse scene controller event: {data}", stateChange);
        return Task.CompletedTask;    
    }


    Task RokuCommand(RokuCommands command)
    {
        return _services.Api.RemoteSendCommand(Remote.RokuUltra, command.ToString());
    }

    Task ClearAllNotifications()
    {
        _notificationService.ClearAll();
        return Task.CompletedTask;
    }

    async Task ToggleMaintenanceMode()
    {
        var respose = await _services.Api.Toggle(Input_Boolean.MaintenanceMode);
        if (!respose.IsSuccessStatusCode)
        {
            await _diningRoomChannel("failed to toggle maintenance mode");
            //await _services.Api.SpeakPiper(MediaPlayers.DiningRoom, );
        }
        else
        {
            await _diningRoomChannel("maintenance mode toggled");
            //await _services.Api.SpeakPiper(MediaPlayers.DiningRoom, "maintenance mode toggled");
        }
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
                case Light.TvBacklight:
                    await _services.Api.LightTurnOn(_tvBacklightPresets, ct);
                    break;
                case Light.CouchOverhead:
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
        yield return "event.living_room_buttons_scene_007";
        yield return "event.living_room_buttons_scene_008";
    }

}
