using HaKafkaNet;

namespace MyHome;

public class AutomationRegistry : IAutomationRegistry
{
    IAutomationFactory _factory;
    public AutomationRegistry(IAutomationFactory factory)
    {
        _factory = factory;
    }

    public IEnumerable<IAutomation> Register()
    {
        yield return _factory.LightOnMotion(
            //"binary_sensor.lumi_lumi_sensor_motion_aq2_motion", "light.office_lights");
            "binary_sensor.unknown", "light.office_lights");
    }

    public IEnumerable<IConditionalAutomation> RegisterContitionals()
    {
        yield return _factory.LightOffOnNoMotion("binary_sensor.lumi_lumi_sensor_motion_aq2_motion","light.office_led_light", TimeSpan.FromMinutes(10));
    }
}
