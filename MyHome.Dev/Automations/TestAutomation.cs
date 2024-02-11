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
        var log = await _services.Api.GetErrorLog(cancellationToken);
        Console.WriteLine(await log.Content.ReadAsStringAsync());
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
