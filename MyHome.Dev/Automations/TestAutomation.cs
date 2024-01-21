using HaKafkaNet;

namespace MyHome.Dev;

public class TestAutomation : IAutomation
{
    private IHaServices _services;

    public TestAutomation(IHaServices services)
    {
        _services = services;
    }

    public Task Execute(HaEntityStateChange stateChange, CancellationToken cancellationToken)
    {
        return _services.Api.LightTurnOff(["light.office_lights","light.office_led_light"]);
        //return Task.CompletedTask;
    }

    public IEnumerable<string> TriggerEntityIds()
    {
        //yield return "sun.sun";
        yield return "input_button.test_button";
    }
}
