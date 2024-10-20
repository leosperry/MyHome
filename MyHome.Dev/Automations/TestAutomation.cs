using System;
using System.Text.Json;
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
        _logger.LogInformation("Simple Initialize");
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
                "input_select.test_dropdown" => DropDown(stateChange, ct),
                _ => Task.CompletedTask
            };  
        }
        catch (System.Exception ex)
        {
            throw;
        }
    }

    private async Task DropDown(HaEntityStateChange stateChange, CancellationToken ct)
    {
        // await _services.Api.InputSelect_SelectFirst(stateChange.EntityId);
        // await Task.Delay(1000);
        // await _services.Api.InputSelect_SelectNext(stateChange.EntityId);
        // await Task.Delay(1000);
        // await _services.Api.InputSelect_SelectLast(stateChange.EntityId);
        // await Task.Delay(1000);
        // await _services.Api.InputSelect_SelectPrevious(stateChange.EntityId);
        // await _services.Api.InputSelect_Select(stateChange.EntityId, TestEnum.Three);
    }

    async Task Button1()
    {
        var dropdown = "input_select.test_dropdown";
        await _services.Api.InputSelect_SelectFirst(dropdown);
        await Task.Delay(1000);
        await _services.Api.InputSelect_SelectNext(dropdown);
        await Task.Delay(1000);
        await _services.Api.InputSelect_SelectLast(dropdown);
        await Task.Delay(1000);
        await _services.Api.InputSelect_SelectPrevious(dropdown);
        await Task.Delay(1000);
        await _services.Api.InputSelect_Select(dropdown, TestEnum.Three);

        //var state = await _services.EntityProvider.GetEntity<HaEntityState<TestEnum, JsonElement>>(dropdown);
        var state = await _services.EntityProvider.GetStateTypedEntity<TestEnum>(dropdown);
        
        var statStr = state?.ToString() ?? "unknown";
        _logger.LogInformation(statStr);
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
        yield return "input_select.test_dropdown";
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