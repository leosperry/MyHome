using System.Net.Http.Headers;
using System.Text.Json;
using HaKafkaNet;

namespace MyHome;

public class LoungeButtons : IAutomation, IAutomationMeta
{
    IHaServices _services;

    const string LOUNG_LIGHTS = "light.lounge_lights";

    public LoungeButtons(IHaServices services)
    {
        _services = services;
    }

    public IEnumerable<string> TriggerEntityIds()
    {
        yield return "event.lounge_buttons_scene_001";
        yield return "event.lounge_buttons_scene_002";
        // yield return "event.lounge_buttons_scene_003";
        // yield return "event.lounge_buttons_scene_004";
        // yield return "event.lounge_buttons_scene_005";
        // yield return "event.lounge_buttons_scene_006";
        // yield return "event.lounge_buttons_scene_007";
        // yield return "event.lounge_buttons_scene_008";
    }  

    public Task Execute(HaEntityStateChange stateChange, CancellationToken ct)
    {
        var sceneEvent = stateChange.ToSceneControllerEvent();

        if(!sceneEvent.New.StateAndLastUpdatedWithin1Second()) return Task.CompletedTask;

        var button = sceneEvent.EntityId.Last();
        var keypress = sceneEvent.New.Attributes?.GetKeyPress();
        return (button,keypress) switch 
        {
            {button: '1', keypress: KeyPress.KeyPressed} => SetLights((255,255,255), ct),
            {button: '2', keypress: KeyPress.KeyPressed} => SetLights((255, 20, 255), ct),
            _ => Task.CompletedTask //unassigned 
        };    
    }

    private async Task SetLights(RgbTuple color, CancellationToken ct)
    {
        var lights = await GetLoungeLights(ct);
          
        if (lights.Attributes?.RGB == color && lights.State == OnOff.On)
        {
            await _services.Api.TurnOff(LOUNG_LIGHTS);
        }
        else
        {
            LightTurnOnModel lightSettings = new LightTurnOnModel()
            {
                EntityId = [LOUNG_LIGHTS],
                RgbColor = color
            };  
            await _services.Api.LightTurnOn(lightSettings, ct);
        }
    }

    private async Task<HaEntityState<OnOff, ColorLightModel>> GetLoungeLights(CancellationToken cancellationToken)
    {
        var lights = await _services.EntityProvider.GetEntity<OnOff, ColorLightModel>(LOUNG_LIGHTS, cancellationToken);
        if (lights.Bad() == true)
        {
            throw new Exception("lounge lights unavailable");
        }
        return lights!;
    }

    public AutomationMetaData GetMetaData()
    {
        return new AutomationMetaData()
        {
            Name = "Lounge Buttons",
            Description = "Set the lights",
            AdditionalEntitiesToTrack = [LOUNG_LIGHTS]
        };
    }
}
