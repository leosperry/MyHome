// using HaKafkaNet;

// namespace MyHome.Registries;

// public class ExampleRegistry : IAutomationRegistry
// {
//     readonly IAutomationFactory _factory;
//     readonly IAutomationBuilder _builder;
//     readonly IHaServices _services;

//     public ExampleRegistry(IAutomationFactory factory, IAutomationBuilder builder, IHaServices services)
//     {
//         _factory = factory;
//         _builder = builder;
//         _services = services;
//     }

//     public void Register(IRegistrar reg)
//     {
//         // register the simeple automations
//         reg.RegisterMultiple(
//             TurnOnLightWithMotion()
//             // add more as needed
//         );

//         // register the schedulables
//         reg.RegisterMultiple(
//             TurnOnLightAtSunset()
//             // add more as needed
//         );
//     }

//     private IAutomation TurnOnLightWithMotion()
//     {
//         return _builder.CreateSimple()
//             .WithName("Office lights on motion")
//             .WithTriggers(Sensors.OfficeMotion)
//             .WithExecution(async (sc, ct) =>{
//                 var motionState = sc.ToOnOff();
//                 if (!motionState.New.Bad() && motionState.New.IsOn())
//                 {
//                     await _services.Api.TurnOn(Lights.OfficeLights);
//                 }
//             })
//             .Build();
//     }

//     private ISchedulableAutomation TurnOnLightAtSunset()
//     {
//         return _factory.SunSetAutomation(
//             ct => _services.Api.TurnOn(Lights.FrontPorchLight)
//         );
//     }
// }
