using HaKafkaNet;

namespace MyHome;

public class AutomationRegistry : IAutomationRegistry
{
    public IEnumerable<IAutomation> Register(IAutomationFactory automationFactory)
    {
        yield return automationFactory.LightOnMotion(
            //"binary_sensor.lumi_lumi_sensor_motion_aq2_motion", "light.office_lights");
            "binary_sensor.unknown", "light.office_lights");
    }

    public IEnumerable<IConditionalAutomation> RegisterContitionals(IAutomationFactory automationFactory)
    {
        yield return automationFactory.LightOffOnNoMotion("binary_sensor.lumi_lumi_sensor_motion_aq2_motion","light.office_led_light", TimeSpan.FromMinutes(10));
    }
}
