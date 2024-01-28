using HaKafkaNet;

namespace MyHome;

public class AutomationRegistry : IAutomationRegistry
{
    readonly IAutomationFactory _factory;

    public AutomationRegistry(IAutomationFactory factory)
    {
        _factory = factory;
    }

    public IEnumerable<IAutomation> Register()
    {
        return Enumerable.Empty<IAutomation>();
    }

    public IEnumerable<IConditionalAutomation> RegisterContitionals()
    {
        yield return _factory.LightOffOnNoMotion(
            ["binary_sensor.lumi_lumi_sensor_motion_aq2_motion"],
            ["light.office_led_light", "light.office_lights"], TimeSpan.FromMinutes(10));
    }
}
