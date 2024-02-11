using HaKafkaNet;

namespace MyHome;

public class AutomationRegistry : IAutomationRegistry
{
    readonly IHaServices _services;
    readonly IAutomationFactory _factory;

    public AutomationRegistry(IHaServices services, IAutomationFactory factory)
    {
        _services = services;
        _factory = factory;
    }

    public void Register(IRegistrar reg)
    {
        reg.Register(_factory.LightOffOnNoMotion(
            ["binary_sensor.lumi_lumi_sensor_motion_aq2_motion"],
            ["light.office_led_light", "light.office_lights"], TimeSpan.FromMinutes(10)));
    }
}
