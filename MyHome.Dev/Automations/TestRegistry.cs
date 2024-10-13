using HaKafkaNet;

namespace MyHome.Dev;

public class TestRegistry : IAutomationRegistry, IInitializeOnStartup
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

    public Task Initialize()
    {
        return Task.CompletedTask;
    }

    public void Register(IRegistrar reg)
    {
        //throw new Exception("blarg");
    }


}
