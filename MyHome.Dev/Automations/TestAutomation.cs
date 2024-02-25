using HaKafkaNet;

namespace MyHome.Dev;

public class TestAutomation : IAutomation, IAutomationMeta
{
    private IHaServices _services;

    public TestAutomation(IHaServices services)
    {
        _services = services;
    }

    public Task Execute(HaEntityStateChange stateChange, CancellationToken cancellationToken)
    {
        var converted = stateChange.ToSceneControllerEvent();
        System.Console.WriteLine($" converted state: {converted.New.State}");
        return Task.CompletedTask;
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
        yield return "event.living_room_buttons_scene_001";
    }
}
