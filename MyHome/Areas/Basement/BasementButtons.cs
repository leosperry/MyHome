using System;
using HaKafkaNet;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using MyHome.People;

namespace MyHome;

public class BasementButtons : IAutomation_SceneController , IAutomationMeta
{
    private readonly AsherService _asherService;
    private readonly ILogger<BasementButtons> _logger;
    private readonly IHaServices _services;
    private readonly IUpdatingEntity<OnOff, LightModel> _basementGroup;

    public BasementButtons(AsherService asherService, IHaServices services, IStartupHelpers helpers, ILogger<BasementButtons> logger)
    {
        _asherService = asherService;
        _logger = logger;
        _services = services;

        _basementGroup = helpers.UpdatingEntityProvider.GetLightEntity(Light.BasementLightGroup);
    }

    public IEnumerable<string> TriggerEntityIds()
    {
        yield return "event.basement_stair_light_scene_001";
        //yield return "event.basement_stair_light_scene_002";
        yield return "event.basement_stair_light_scene_003";
        yield return "event.basement_stair_light_scene_004";
        // yield return "event.basement_stair_light_scene_005";
    }

    public Task Execute(HaEntityStateChange<HaEntityState<DateTime?, SceneControllerEvent>> stateChange, CancellationToken ct)
    {
        if (!stateChange.New.StateAndLastUpdatedWithin1Second()) return Task.CompletedTask;
        
        var btn = stateChange.EntityId.Last();
        var press = stateChange?.New.Attributes?.GetKeyPress();
        return (btn, press) switch
        {
            {btn : '1', press: KeyPress.KeyPressed} => _services.Api.TurnOn(Light.BasementLightGroup),
            {btn : '3', press: KeyPress.KeyPressed} => _asherService.TurnOff(ct),
            {btn : '1', press: KeyPress.KeyHeldDown} => _asherService.IncreaseLights(ct), //_services.Api.LightTurnOn(new LightTurnOnModel(){EntityId = [Lights.BasementGroup], BrightnessStepPct = 5}),
            {btn : '3', press: KeyPress.KeyHeldDown} => _asherService.DecreaseLights(ct), //_services.Api.LightTurnOn(new LightTurnOnModel(){EntityId = [Lights.BasementGroup], BrightnessStepPct = -5}),
            {btn: '4', press: KeyPress.KeyPressed} => _services.Api.Toggle(Input_Boolean.BasementOverride),
            _ => HandleNoMatch(btn, press)
        };
    }

    Task HandleNoMatch(char  button, KeyPress? press)
    {
        _logger.LogWarning("No match found for button {button} and press {press}", button, press.ToString() ?? "null");
        return Task.CompletedTask;
    }

    public AutomationMetaData GetMetaData()
    {
        return new ()
        {
            Name = "Basement Buttons",
            Description = "Control lights as a group and override"
        };
    }
}
