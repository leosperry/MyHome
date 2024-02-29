using HaKafkaNet;

namespace MyHome.Dev;

public class TestRegistry : IAutomationRegistry
{
    IHaServices _services;
    IAutomationFactory _factory;
    IAutomationBuilder _builder;
    public TestRegistry(IHaServices services, IAutomationFactory factory, IAutomationBuilder builder) 
    {
        _services = services;
        _factory = factory;
        _builder = builder;
    }

    public void Register(IRegistrar reg)
    {
        reg.Register(_factory.DurableAutoOff("light.office_led_light", TimeSpan.FromSeconds(10)));
    }
}
