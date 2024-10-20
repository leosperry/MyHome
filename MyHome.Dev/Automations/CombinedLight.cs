using System;
using System.Text.Json;
using HaKafkaNet;

namespace MyHome.Dev.Automations;

public class CombinedLight : IAutomation<OnOff, ColorLightModel>, IInitializeOnStartup //, IAutomationMeta
{
    private readonly IHaApiProvider _api;
    private readonly ILogger<CombinedLight> logger;

    public CombinedLight(IHaApiProvider api, ILogger<CombinedLight> logger)
    {
        _api = api;
        this.logger = logger;
    }

    public async Task Execute(HaEntityStateChange<HaEntityState<OnOff, ColorLightModel>> stateChange, CancellationToken ct)
    {
        if (stateChange.New.IsOff())
        {
            await _api.TurnOff("light.office_light_bars", ct);
        }
        else
        {
            await _api.TurnOn("light.office_light_bars", ct);
        }
    }

    // public AutomationMetaData GetMetaData()
    // {
    //     return new()
    //     {
    //         Name = "my combined light"
    //     };
    // }

    public Task Initialize()
    {
        logger.LogInformation("Typed Initialize");
        return Task.CompletedTask;
    }

    public IEnumerable<string> TriggerEntityIds()
    {
        yield return "light.office_combined_light";
    }
}
