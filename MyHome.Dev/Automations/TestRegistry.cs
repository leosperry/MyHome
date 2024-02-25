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
    _factory.DurableAutoOffOnEntityOff(
        "light.my_light", "binary_sensor.my_motion_sensor", 
        TimeSpan.FromMinutes(5));
    }
}
