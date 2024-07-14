using HaKafkaNet;

namespace MyHome.Dev;

public class TestRegistry : IAutomationRegistry
{
    IHaServices _services;
    IAutomationFactory _factory;
    IAutomationBuilder _builder;
    ILogger<TestRegistry> _logger;
    public TestRegistry(IHaServices services, IAutomationFactory factory, IAutomationBuilder builder, ILogger<TestRegistry> logger) 
    {
        _services = services;
        _factory = factory;
        _builder = builder;
        _logger = logger;
    }

    //private bool _tracker;

    public void Register(IRegistrar reg)
    {
        // reg.Register(_builder.CreateSimple()
        //     .WithTriggers("input_button.test_button")
        //     .WithExecution(async (sc, ct) => {
        //         await _services.Api.ToggleByLabel("bedtimeoff");
        //     })
        //     .Build());

        //reg.Register(_factory.DurableAutoOff("light.office_led_light", TimeSpan.FromSeconds(1)));

        // reg.Register(
        //     _factory.SimpleAutomation(["input_button.test_button"], async (stateChange,ct) =>{

        //         var rgbLight2 = await _services.EntityProvider.GetEntity<OnOff, ColorLightModel>("light.living_room");
        //         var lights = await _services.EntityProvider.GetEntity<OnOff, ColorLightModel>("light.lounge_lights");
        //     }));
        
        // reg.Register(_builder.CreateSchedulable()
        //     .WithName("Test scheduled")
        //     .WithDescription("This is an incredibly long description. This is an incredibly long description. This is an incredibly long description. This is an incredibly long description. This is an incredibly long description. ")
        //     .WithTriggers("input_button.test_button_3")
        //     .GetNextScheduled((sc, ct) => Task.FromResult<DateTime?>(DateTime.Now.AddSeconds(1)))
        //     .WithExecution(ct => _services.Api.PersistentNotification("test bwahahah", default))
        //     .Build());

        // reg.Register(_factory.DurableAutoOn("light.office_led_light", TimeSpan.FromSeconds(2)));

        // reg.Register(_factory.SunRiseAutomation(ct => Task.CompletedTask));

    }
}
