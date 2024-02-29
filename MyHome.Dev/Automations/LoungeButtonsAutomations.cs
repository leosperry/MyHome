// using HaKafkaNet;

// namespace MyHome.Dev;

// public class LoungeButtonsAutomations : IAutomation
// {
//     readonly IHaServices _services;
//     public LoungeButtonsAutomations(IHaServices services)
//     {
//         _services = services;
//     }

//     public Task Execute(HaEntityStateChange stateChange, CancellationToken cancellationToken)
//     {
//         var sceneEvent = stateChange.ToSceneControllerEvent();
//         var button = sceneEvent.EntityId.Last();
//         return button switch 
//         {
//             '7' => SetScene1(sceneEvent.New.Attributes!),
//             _ => Task.CompletedTask
//         };
//     }

//     private async Task SetScene1(SceneControllerEvent attributes)
//     {
//         LightTurnOnModel lightSettings = new()
//         {
//             EntityId = ["light.office_led_light"]
//         };
//         switch (attributes.GetKeyPress())
//         {
//             case KeyPress.KeyPressed:
//                 lightSettings.XyColor = ( 0.664f, 0.287f);
//                 await _services.Api.LightTurnOn(lightSettings);
//                 break;
//             case KeyPress.KeyPressed2x:
//                 lightSettings.XyColor = (0.175f, 0.715f);
//                 await _services.Api.LightTurnOn(lightSettings);
//                 break;
//             case KeyPress.KeyHeldDown:
//                 lightSettings.XyColor = (0.136f, 0.043f);
//                 await _services.Api.LightTurnOn(lightSettings);
//                 break;
//             default:
//                 //action not assigned
//                 break;

//         }
//     }

//     public IEnumerable<string> TriggerEntityIds()
//     {
//         yield return "event.lounge_buttons_scene_001";
//         yield return "event.lounge_buttons_scene_002";
//         yield return "event.lounge_buttons_scene_003";
//         yield return "event.lounge_buttons_scene_004";
//         yield return "event.lounge_buttons_scene_005";
//         yield return "event.lounge_buttons_scene_006";
//         yield return "event.lounge_buttons_scene_007";
//         yield return "event.lounge_buttons_scene_008";
//     }
// }
