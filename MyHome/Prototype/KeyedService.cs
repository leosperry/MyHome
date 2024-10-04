// using System;
// using System.Text.Json;
// using HaKafkaNet;

// namespace MyHome.Prototype;

// [HaEntityStateAware]
// public class KeyedService
// {
//     ThreadSafeEntity<OnOff> _theLight;

//     public KeyedService(IEntityObserver observer)
//     {
//         //_theLight = light;
//         _theLight = observer.GetAutoUpdatedEntity<OnOff>("light.the_light");
//     }

//     async Task SomeMethodRequiringState()
//     {
//         // get an entity state
//         //var state = TheLight.Get();
//     }
// }


