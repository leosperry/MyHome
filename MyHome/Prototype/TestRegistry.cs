using HaKafkaNet;
using MyHome.Models;

namespace MyHome;

public class TestRegistry : IAutomationRegistry
{
    IAutomationFactory _factory;
    IAutomationBuilder _builder;
    INotificationService _notifications;
    private ILogger<TestRegistry> _logger;
    private IHaServices _services;

    public TestRegistry(IAutomationFactory factory, IAutomationBuilder builder, INotificationService notificationService, IHaServices services, ILogger<TestRegistry> logger)
    {
        _factory = factory;
        _builder = builder;
        _notifications = notificationService;
        _logger = logger;
        _services = services;
    }

    int tracker = 0;

    public void Register(IRegistrar reg)
    {
        var test_button = "input_button.test_button";
        var testSonos = _builder.CreateSimple()
            .WithTriggers(test_button)
            .WithExecution(async (sc, ct) => {
                try
                {
                    var playerState = await _services.EntityProvider.GetMediaPlayer<SonosAttributes>(MediaPlayers.Asher);
                }
                catch (System.Exception)
                {
                    
                    throw;
                }
                
            })
            .Build();
        
        reg.Register(testSonos);

        //TestLAM(reg);
        // reg.Register(_factory.SimpleAutomation(["input_button.test_button"], (sc, ct) =>
        // {
        //     _services.Api.Toggle(Lights.OfficeLeds);
        //     System.Console.WriteLine("test button pushed");
        //     _logger.LogWarning("test button pushed");
        //     return Task.CompletedTask;
        // }).WithMeta("test", "test logging"));
    }

    private void TestLAM(IRegistrar reg)
    {
        var red = _notifications.CreateMonkeyChannel(new LightTurnOnModel()
        {
            EntityId = [],
            RgbColor = (255, 0, 0),
            Brightness = Bytes._30pct,
        });
        var green = _notifications.CreateMonkeyChannel(new LightTurnOnModel()
        {
            EntityId = [],
            ColorName = "green",
            Brightness = Bytes._30pct
        });
        var blue = _notifications.CreateMonkeyChannel(new LightTurnOnModel()
        {
            EntityId = [],
            XyColor = (0.136f, 0.04f),
            Brightness = Bytes._30pct
        });

        var leoPhone = _notifications.CreateGroupOrDeviceChannel([Phones.LeonardPhone]);

        var redSender = _notifications.CreateNotificationSender([_notifications.Persistent], [red]);
        var greenSender = _notifications.CreateNotificationSender([_notifications.Persistent], [green]);
        var blueSender = _notifications.CreateNotificationSender([leoPhone], [blue]);

        reg.Register(_factory.SimpleAutomation(["input_button.test_button"], async (sc, ct) =>
        {
            switch (tracker % 3)
            {
                case 0:
                    await redSender("this is red");
                    break;
                case 1:
                    await greenSender("this is a green", id: new NotificationId("green"));
                    break;
                case 2:
                default:
                    await blueSender("this is a blue", id: new NotificationId("blue"));
                    break;
            }
        }));

        reg.Register(_factory.SimpleAutomation(["input_button.test_button_2"],
            async (sc, ct) =>
            {
                await _notifications.Clear(new NotificationId("blue"));
            }));


        reg.Register(_factory.SimpleAutomation(["input_button.test_button_3"], async (sc, ct) =>
        {
            var state = sc.ToDateTimeTyped();

            await _notifications.ClearAll();
        }));

                // disabled
        // reg.Register(_builder.CreateSimple(false)
        //     .WithName("Person arriving home")
        //     .WithTriggers("person.leonard", "person.rachel")
        //     .WithExecution(async (sc, ct) =>{
        //         var person = sc.ToPerson();
        //         if (person.CameHome())
        //         {
        //             //await _notifyKitchenLivingRoom($"{person?.New?.Attributes?.FriendlyName ?? "person"} is arriving home");
        //         }
        //     })
        //     .Build()
        // );
    }
}
