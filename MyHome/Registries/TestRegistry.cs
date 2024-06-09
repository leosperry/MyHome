using HaKafkaNet;

namespace MyHome;

public class TestRegistry : IAutomationRegistry
{
    IAutomationFactory _factory;
    INotificationService _notifications;
    private ILogger<TestRegistry> _logger;

    public TestRegistry(IAutomationFactory factory, INotificationService notificationService, ILogger<TestRegistry> logger)
    {
        _factory = factory;
        _notifications = notificationService;
        this._logger = logger;
    }

    int tracker = 0;

    public void Register(IRegistrar reg)
    {
        //TestLAM(reg);
        reg.Register(_factory.SimpleAutomation(["input_button.test_button"], (sc, ct) =>
        {
            System.Console.WriteLine("test button pushed");
            _logger.LogInformation("test button pushed");
            return Task.CompletedTask;
        }).WithMeta("test", "test logging"));
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
            //tracker++;
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
    }
}
