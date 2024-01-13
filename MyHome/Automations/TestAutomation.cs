using HaKafkaNet;

namespace MyHome;

public class TestAutomation : IAutomation
{
    private readonly IHaApiProvider _api;

    public TestAutomation(IHaApiProvider api)
    {
        this._api = api;
    }

    public Task Execute(HaEntityStateChange stateChange, CancellationToken cancellationToken)
    {
        //await _garageService.EnsureGarageClosed(cancellationToken);
        return _api.NotifyAlexaMedia("test", ["Office"], cancellationToken);
    }

    public IEnumerable<string> TriggerEntityIds()
    {
        yield return "input_button.test_button";
    }
}
