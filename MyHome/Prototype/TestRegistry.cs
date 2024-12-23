﻿// using System.Diagnostics;
// using System.Text.Json;
// using HaKafkaNet;
// using MyHome.Models;

// namespace MyHome;

// public class TestRegistry : IAutomationRegistry
// {
//     IAutomationFactory _factory;
//     IAutomationBuilder _builder;
//     INotificationService _notifications;
//     private ILogger<TestRegistry> _logger;
//     private IHaServices _services;

//     const string TEST_BUTTON = "input_button.test_button";

//     private IHaEntity<OnOff, LightModel> _officeLeds;

//     public TestRegistry(IAutomationFactory factory, IAutomationBuilder builder, INotificationService notificationService, IHaServices services, ILogger<TestRegistry> logger, IUpdatingEntityProvider updatingProvider)
//     {
//         _factory = factory;
//         _builder = builder;
//         _notifications = notificationService;
//         _logger = logger;
//         _services = services;

//         _officeLeds = updatingProvider.GetLightEntity(Lights.OfficeLeds);
//     }

//     int tracker = 0;

//     public void Register(IRegistrar reg)
//     {
//         reg.TryRegister(
//             ReportAverage,
//             blarg
//         );
//     }

//     IAutomationBase blarg()
//     {
//         return _builder.CreateSimple<float?>()
//             .WithName("testing from HaKafkaNet")
//             .WithTriggers("input_number.trigger_from_hakafkanet")
//             .WithExecution(async (sc, ct) => {
//                 var stopwatch = new Stopwatch();
//                 stopwatch.Start();
//                 var response = await _services.Api.InputNumberIncrement("input_number.count_from_hakafkanet");
//                 times.Add(stopwatch.ElapsedMilliseconds);
//             })
//             .Build();
//     }

//     List<long> times = new();

//     IAutomationBase ReportAverage()
//     {
//         return _builder.CreateSimple()
//             .WithTriggers(TEST_BUTTON)
//             .WithExecution((sc, ct) => {
//                 _logger.LogInformation("average: {average}", times.Average());
//                 return Task.CompletedTask;
//             })
//             .Build();
//     }

//     public IAutomation TestButton()
//     {
//         return  _builder.CreateSimple()
//             .WithTriggers(TEST_BUTTON)
//             .WithName("Test Button")
//             .WithExecution(async (sc, ct) => {
//                 try
//                 {
//                     await TestButtonAction();
//                     _logger.LogTrace("test automation success");
//                 }
//                 catch (System.Exception ex)
//                 {
//                     _logger.LogError(ex, "Failure in test automation");
//                     throw;
//                 }
//             })
//             .Build();
//     }

//     private async Task TestButtonAction()
//     {
//         // await _services.Api.NotifyCommand(Phones.LeonardPhone, AndroidCommand.command_activity, new{
//         //     intent_package_name = "com.google.android.apps.maps",
//         //     intent_action = "android.intent.action.VIEW",
//         //     intent_uri = "google.navigation:q=arbys"
//         // });
//         // await _services.Api.NotifyCommand(Phones.LeonardPhone, AndroidCommand.command_high_accuracy_mode, new{
//         //     command = "turn_on"
//         // });
//         // await _services.Api.NotifyCommand(Phones.LeonardPhone, AndroidCommand.command_high_accuracy_mode, new{
//         //     high_accuracy_update_interval = 120,
//         //     command = "high_accuracy_set_update_interval"
//         // });
//     }

//     private void TestLAM(IRegistrar reg)
//     {
//         var red = _notifications.CreateMonkeyChannel(new LightTurnOnModel()
//         {
//             EntityId = [],
//             RgbColor = (255, 0, 0),
//             Brightness = Bytes._30pct,
//         });
//         var green = _notifications.CreateMonkeyChannel(new LightTurnOnModel()
//         {
//             EntityId = [],
//             ColorName = "green",
//             Brightness = Bytes._30pct
//         });
//         var blue = _notifications.CreateMonkeyChannel(new LightTurnOnModel()
//         {
//             EntityId = [],
//             XyColor = (0.136f, 0.04f),
//             Brightness = Bytes._30pct
//         });

//         var leoPhone = _notifications.CreateGroupOrDeviceChannel([Phones.LeonardPhone]);

//         var redSender = _notifications.CreateNotificationSender([_notifications.Persistent], [red]);
//         var greenSender = _notifications.CreateNotificationSender([_notifications.Persistent], [green]);
//         var blueSender = _notifications.CreateNotificationSender([leoPhone], [blue]);

//         reg.Register(_factory.SimpleAutomation(["input_button.test_button"], async (sc, ct) =>
//         {
//             switch (tracker % 3)
//             {
//                 case 0:
//                     await redSender("this is red");
//                     break;
//                 case 1:
//                     await greenSender("this is a green", id: new NotificationId("green"));
//                     break;
//                 case 2:
//                 default:
//                     await blueSender("this is a blue", id: new NotificationId("blue"));
//                     break;
//             }
//         }));

//         reg.Register(_factory.SimpleAutomation(["input_button.test_button_2"],
//             async (sc, ct) =>
//             {
//                 await _notifications.Clear(new NotificationId("blue"));
//             }));


//         reg.Register(_factory.SimpleAutomation(["input_button.test_button_3"], async (sc, ct) =>
//         {
//             var state = sc.ToDateTimeTyped();

//             await _notifications.ClearAll();
//         }));

//                 // disabled
//         // reg.Register(_builder.CreateSimple(false)
//         //     .WithName("Person arriving home")
//         //     .WithTriggers("person.leonard", "person.rachel")
//         //     .WithExecution(async (sc, ct) =>{
//         //         var person = sc.ToPerson();
//         //         if (person.CameHome())
//         //         {
//         //             //await _notifyKitchenLivingRoom($"{person?.New?.Attributes?.FriendlyName ?? "person"} is arriving home");
//         //         }
//         //     })
//         //     .Build()
//         // );
//     }
// }
