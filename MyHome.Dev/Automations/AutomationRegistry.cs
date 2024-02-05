using HaKafkaNet;

namespace MyHome;

public class AutomationRegistry : IAutomationRegistry
{
    IAutomationFactory _factory;
    IAutomationBuilder _builder;

    public AutomationRegistry(IAutomationFactory factory, IAutomationBuilder builder)
    {
        _factory = factory;
        _builder = builder;
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
}
