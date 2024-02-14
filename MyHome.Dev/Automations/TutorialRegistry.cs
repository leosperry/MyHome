using HaKafkaNet;

namespace MyHome;

public class TutorialRegistry : IAutomationRegistry
{
    readonly IAutomationFactory _factory;
    readonly IAutomationBuilder _builder;
    readonly IHaServices _services;

    public TutorialRegistry(IAutomationFactory factory, IAutomationBuilder builder, IHaServices services)
    {
        _factory = factory;
        _builder = builder;
        _services = services;
    }

    public void Register(IRegistrar reg)
    {
        var frontPorchAtSunset = _builder.CreateSunAutomation(SunEventType.Set)
            .WithOffset(TimeSpan.FromMinutes(-15))
            .WithExecution(ct => _services.Api.TurnOn("light.front_porch"))
            .Build();

        reg.Register(frontPorchAtSunset);
        
        var turnOffFrontPorch = _factory.EntityAutoOff("light.front_porch", TimeSpan.FromHours(2));
        
        var turnOnBackPorchWithMotion = _factory.LightOnMotion("binary_sensor.back_porch", "light.back_porch");

        var turnOffBackPorch = _factory.LightOffOnNoMotion("binary_sensor.back_porch", "light.back_porch", TimeSpan.FromMinutes(5));
        
        reg.Register(turnOnBackPorchWithMotion);
        reg.RegisterMultiple([turnOffBackPorch, turnOffFrontPorch]);
    }
}
