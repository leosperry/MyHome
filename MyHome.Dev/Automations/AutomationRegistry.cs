using HaKafkaNet;

namespace MyHome;

public class AutomationRegistry : IAutomationRegistry
{
    IAutomationFactory _factory;
    IAutomationBuilder _builder;
    IHaServices _services;

    public AutomationRegistry(IAutomationFactory factory, IAutomationBuilder builder, IHaServices services)
    {
        _factory = factory;
        _builder = builder;
        _services = services;
    }

    public void Register(IRegistrar reg)
    {
        reg.RegisterMultiple(RegisterContitionals());
        reg.RegisterMultiple(RegisterSchedulable());
    }

    public IEnumerable<IAutomation> Register()
    {
        return Enumerable.Empty<IAutomation>();
        // yield return _factory.LightOnMotion(
        //     //"binary_sensor.lumi_lumi_sensor_motion_aq2_motion", "light.office_lights");
        //     "binary_sensor.unknown", "light.office_lights");
    }


    public IEnumerable<IConditionalAutomation> RegisterContitionals()
    {
        yield return _builder.CreateConditionalWithServices()
            .WithName("test disable")
            .WithTriggers("input_button.test_button_3")
            .When(sc => true)
            .ForSeconds(7)
            .Then((svc, ct) => svc.Api.LightToggle("light.office_led_light"))
            .Build();



        //yield return _factory.LightOffOnNoMotion("binary_sensor.lumi_lumi_sensor_motion_aq2_motion","light.office_led_light", TimeSpan.FromMinutes(10));
    }

    public IEnumerable<ISchedulableAutomation> RegisterSchedulable()
    {
        yield return _factory.SunSetAutomation(ct => _services.Api.PersistentNotification("pre sunset"), TimeSpan.FromMinutes(-1));
        yield return _factory.SunSetAutomation(ct => _services.Api.PersistentNotification("at sunset"));
        yield return _factory.SunSetAutomation(ct => _services.Api.PersistentNotification("post sunset"), TimeSpan.FromMinutes(1));

        yield return new SunNoonAutomation(ct => _services.Api.PersistentNotification("pre noon"), TimeSpan.FromMinutes(-1));
        yield return new SunNoonAutomation(ct => _services.Api.PersistentNotification("at noon"));
        yield return new SunNoonAutomation(ct => _services.Api.PersistentNotification("post noon"), TimeSpan.FromMinutes(1));
    }
}
