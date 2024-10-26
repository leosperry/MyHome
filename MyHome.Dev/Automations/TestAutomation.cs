using System;
using System.Text.Json;
using HaKafkaNet;

namespace MyHome.Dev.Automations;

public class TestAutomation : IInitializeOnStartup, 
    //IAutomation, 
    IAutomation<OnOff>,
    // IAutomation<OnOff, JsonElement>,
    // IConditionalAutomation,
    // IConditionalAutomation<OnOff>,
    // IConditionalAutomation<OnOff, JsonElement>,
    // ISchedulableAutomation,
    // ISchedulableAutomation<OnOff>,
    // ISchedulableAutomation<OnOff,JsonElement>,
    // IDelayableAutomation<OnOff, JsonElement>,
    IAutomationMeta
{
    private readonly IStartupHelpers _helpers;
    private readonly IHaServices _services;
    private readonly ILogger<TestAutomation> _logger;

    const string date_helper = "input_datetime.test_date";

    public TimeSpan For => throw new NotImplementedException();

    public bool IsReschedulable => throw new NotImplementedException();

    const string 
        Test_Switch = "input_boolean.test_switch",
        LEDS = "light.office_led_light",
        Light_Bars = "light.office_light_bars";

    public TestAutomation(IStartupHelpers helpers, IHaServices services, ILogger<TestAutomation> logger)
    {
        this._helpers = helpers;
        _services = services;
        this._logger = logger;
    }

    public Task Initialize()
    {
        _logger.LogInformation("Simple Initialize");
        return Task.CompletedTask;
    }

    public  Task Execute(HaEntityStateChange stateChange, CancellationToken ct)
    {
        _logger.LogInformation("Execute");
        return Task.CompletedTask;
    }

    public IEnumerable<string> TriggerEntityIds()
    {
        yield return Test_Switch;
    }

    public Task Execute(HaEntityStateChange<HaEntityState<OnOff, JsonElement>> stateChange, CancellationToken ct)
    {
        _logger.LogInformation("Friendly name is : {friendly}", stateChange.New.FriendlyName("blarg"));
        return Task.CompletedTask;    
    }

    public Task<bool> ContinuesToBeTrue(HaEntityStateChange stateChange, CancellationToken ct)
    {
        _logger.LogInformation("Continues to be true");
        return Task.FromResult(stateChange.ToOnOff().IsOn());    
    }

    public Task Execute(CancellationToken ct)
    {
        _logger.LogInformation("Execute delayed");
        return Task.CompletedTask;    
    }

    public Task<bool> ContinuesToBeTrue(HaEntityStateChange<HaEntityState<OnOff, JsonElement>> stateChange, CancellationToken ct)
    {
        _logger.LogInformation("Continues to be true typed");
        return Task.FromResult(stateChange.IsOn());    
    }

    public DateTime? GetNextScheduled()
    {
        return DateTime.Now.AddSeconds(3);
    }

    int metaCount = 5;
    public AutomationMetaData GetMetaData()
    {
        return new()
        {
            Name = $"Uber {++metaCount}"
        };
    }
}


enum TestEnum
{
    One,
    Two,
    Three,
    Four,
    Five
}