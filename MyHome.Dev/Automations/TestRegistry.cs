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

    public void Register(IRegistrar reg)
    {


    }
}
