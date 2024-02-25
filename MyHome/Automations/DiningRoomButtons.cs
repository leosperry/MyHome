using System.Diagnostics;
using System.Text.Json;
using HaKafkaNet;

namespace MyHome;

public class DiningRoomButtons : IAutomation
{
    readonly IHaServices _services;
    public DiningRoomButtons(IHaServices servcies)
    {
        _services = servcies;
    }

    public Task Execute(HaEntityStateChange stateChange, CancellationToken ct)
    {
        if (stateChange.EntityId == Helpers.LivingRoomOverride || stateChange.EntityId == Helpers.PorchMotionEnable)
        {
            return SetHelperState((HaEntityState<OnOff, JsonElement>)stateChange.New, ct);
        }
        
        var sceneState = stateChange.ToSceneControllerEvent();

        if(!sceneState.New.StateAndLastUpdatedWithin1Second()) return Task.CompletedTask;
        
        var btn = stateChange.EntityId.Last();
        var press = sceneState?.New.Attributes?.GetKeyPress();
        return (btn, press) switch
        {
            {btn: '1', press: KeyPress.KeyPressed} => _services.Api.Toggle(Helpers.LivingRoomOverride, ct),
            {btn: '3', press: KeyPress.KeyPressed} => _services.Api.Toggle(Helpers.PorchMotionEnable, ct),
            {btn: '4', press: KeyPress.KeyPressed} => AsherButton(ct),
            _ => Task.CompletedTask
        };
    }

    public IEnumerable<string> TriggerEntityIds()
    {
        yield return Helpers.LivingRoomOverride;
        yield return Helpers.PorchMotionEnable;
        yield return "event.dining_room_scene_001";
        //yield return "event.dining_room_scene_002";
        yield return "event.dining_room_scene_003";
        yield return "event.dining_room_scene_004";
        //yield return "event.dining_room_scene_005";
    }

    async Task AsherButton(CancellationToken ct)
    {
        /* 
        Step 1
            Play random
            Set 1 light on and 1 off
        Step 2
            repeat 4 times
            toggle lights
        Step 3
            set lights to 40%*/
        await Task.WhenAll(
            PlayRandom(ct),
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

    Task PlayRandom(CancellationToken ct)
    {
        string[] messages = [ 
            "My lord. Your presence is requested in the main chamber",  
            "Prince Asher, the king and queen have summoned you",
            "Hey you. Yes, you. Please come upstairs",
            "The parental units have requested of the carbon based lifeform known as Asher to vacate his domicile and return upstairs", 
            "The crown has requested a status report from the dungeons forthwith",
            "Your assignment is as follows, collect dishes and return to base. This message will self destruct",
            "Brave knight. You have been given a valiant quest to return to the overworld" ];
        Random r = new();
        
        return _services.Api.NotifyAlexaMedia(messages[r.Next(0, messages.Length)], [Alexa.Asher], ct);
    }

    Task SetHelperState(HaEntityState<OnOff, JsonElement>? currentHelperState, CancellationToken ct)
    {
        ZoozColor color = currentHelperState?.State switch
        {
            OnOff.On => ZoozColor.Red,
            OnOff.Off => ZoozColor.Green,
            _ => ZoozColor.White
        };

        int parameter = currentHelperState?.EntityId switch
        {
            Helpers.LivingRoomOverride => 7,
            Helpers.PorchMotionEnable => 9,
            _ => throw new Exception("unknown helper. Tell Leonard to fix his code")
        };

        return _services.Api.ZwaveJs_SetConfigParameter(new{
            entity_id = Lights.DiningRoomLights,
            endpoint = 0,
            parameter,
            value = (int)color
        }, ct);
    }

    enum ZoozColor
    {
        White = 0,
        Blue = 1,
        Green = 2,
        Red = 3
    }
}
