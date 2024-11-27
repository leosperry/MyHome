using System;
using System.Text.Json;
using HaKafkaNet;

namespace MyHome.Dev.Automations;

public class MyNumberTracker : IAutomation<float>, IFallbackExecution
{
    public async Task Execute(HaEntityStateChange<HaEntityState<float, JsonElement>> stateChange, CancellationToken ct)
    {
        float trackerValue = stateChange.New.State;
        // implement
        await Task.CompletedTask;
    }

    public async Task FallbackExecute(Exception ex, HaEntityStateChange stateChange, CancellationToken ct)
    {
        // not likely to ever be called for a float
        // very useful for custom integrations
        await Task.CompletedTask;
    }

    public IEnumerable<string> TriggerEntityIds()
    {
        yield return "input_number.my_number_helper";
    }
}
