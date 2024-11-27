using System;
using System.Text.Json;
using HaKafkaNet;

namespace MyHome.Dev.Automations;

public class TestAutomation : IInitializeOnStartup, 
    IAutomation<float?>,
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

    public async Task Execute(HaEntityStateChange stateChange, CancellationToken ct)
    {
        var lightStateChange =  stateChange.ToOnOff<ColorLightModel>();
        // work with stronly typed properties
        await Task.CompletedTask;
    }

    public IEnumerable<string> TriggerEntityIds()
    {
        //yield return Test_Switch;
        yield return "input_number.test_input_number";
    }

    int metaCount = 5;
    public AutomationMetaData GetMetaData()
    {
        return new()
        {
            Name = $"testing the mode",
            Enabled = true,
            Mode = AutomationMode.Parallel
        };
    }

    public async Task Execute(HaEntityStateChange<HaEntityState<float?, JsonElement>> stateChange, CancellationToken ct)
    {
        try
        {
            _logger.LogInformation("running test automation. Time: {time}", stateChange.New.LastChanged);
            await Task.Delay(1000, ct);
        }
        catch (System.Exception)
        {
            throw;
        }
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