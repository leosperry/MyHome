using HaKafkaNet;

namespace MyHome;

public class TestAutomation : IAutomation
{
    private readonly IGarageService _garageService;

    public TestAutomation(IGarageService garageService)
    {
        this._garageService = garageService;
    }

    public async Task Execute(HaEntityStateChange stateChange, CancellationToken cancellationToken)
    {
        //await _garageService.EnsureGarageClosed(cancellationToken);
    }

    public IEnumerable<string> TriggerEntityIds()
    {
        yield return "input_button.test_button";
    }
}
