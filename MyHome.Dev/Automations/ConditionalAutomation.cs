using System;
using HaKafkaNet;

namespace MyHome.Dev.Automations;

public class ConditionalAutomation : IConditionalAutomation, IInitializeOnStartup
{
    private readonly ILogger<ConditionalAutomation> logger;

    public TimeSpan For => TimeSpan.Zero;

    public ConditionalAutomation(ILogger<ConditionalAutomation> logger)
    {
        this.logger = logger;
    }

    public Task<bool> ContinuesToBeTrue(HaEntityStateChange haEntityStateChange, CancellationToken ct)
    {
        return Task.FromResult(false);
    }

    public Task Execute(CancellationToken ct)
    {
        return Task.CompletedTask;
    }

    public Task Initialize()
    {
        logger.LogInformation ("Conditional Initialize");
        return Task.CompletedTask;
    }

    public IEnumerable<string> TriggerEntityIds()
    {
        yield return "light.test_light";
    }
}
