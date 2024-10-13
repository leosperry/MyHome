using System;
using HaKafkaNet;

namespace MyHome.Dev.Automations;

public class TestAutomation : IAutomation, IInitializeOnStartup
{
    private readonly IStartupHelpers _helpers;
    private readonly IHaServices _services;
    private readonly ILogger<TestAutomation> _logger;

    const string date_helper = "input_datetime.test_date";

    public TestAutomation(IStartupHelpers helpers, IHaServices services, ILogger<TestAutomation> logger)
    {
        this._helpers = helpers;
        _services = services;
        this._logger = logger;
    }

    public Task Initialize()
    {
        return Task.CompletedTask;
    }

    public  Task Execute(HaEntityStateChange stateChange, CancellationToken ct)
    {
        try
        {
            return stateChange.EntityId switch{
                "input_button.test_button" => Button1(),
                "input_button.test_button_2" => Button2(),
                "input_button.test_button_3" => Button3(),
                _ => Task.CompletedTask
            };  
        }
        catch (System.Exception ex)
        {
            throw;
        }
    }

    async Task Button1()
    {
        await _services.Api.InputDateTimeSetDateTime(date_helper, DateTime.Now);
    }

    async Task Button2()
    {
        //await _services.Api.InputDateTimeSetDate(date_helper, DateOnly.FromDateTime(DateTime.Now));
        await _services.Api.InputDateTimeSetDate(date_helper, DateTime.Now);
    }


    async Task Button3()
    {
        await _services.Api.InputDateTimeSetTime(date_helper, DateTime.Now.TimeOfDay);
    }


    public IEnumerable<string> TriggerEntityIds()
    {
        yield return "input_button.test_button";
        yield return "input_button.test_button_2";
        yield return "input_button.test_button_3";
    }
}
