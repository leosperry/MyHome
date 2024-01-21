using HaKafkaNet;

namespace MyHome;

public class AutomationRegistry : IAutomationRegistry
{
    public IEnumerable<IAutomation> Register(IAutomationFactory automationFactory)
    {
        return Enumerable.Empty<IAutomation>();
    }

    public IEnumerable<IConditionalAutomation> RegisterContitionals(IAutomationFactory automationFactory)
    {
        yield return automationFactory.LightOffOnNoMotion(
            ["binary_sensor.lumi_lumi_sensor_motion_aq2_motion"],
            ["light.office_led_light", "light.office_lights"], TimeSpan.FromMinutes(10));
    }
}
