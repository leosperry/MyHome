using HaKafkaNet;

namespace MyHome.Dev;

public class TestAutomation : IAutomation, IAutomationMeta
{
    private IHaServices _services;

    public TestAutomation(IHaServices services)
    {
        _services = services;
    }

    public async Task Execute(HaEntityStateChange stateChange, CancellationToken cancellationToken)
    {
        var state = await _services.Api.GetEntityState("button.back_door_contact_identify");
        Console.WriteLine(state);
        //return Task.CompletedTask;
    }

    public AutomationMetaData GetMetaData()
    {
        return new()
        {
            Name = "Test Automamtion",
            Description = "Used for testing quick scenarios",
            Enabled = true
        };
    }

    public IEnumerable<string> TriggerEntityIds()
    {
        //yield return "sun.sun";
        yield return "input_button.test_button";
    }
}
