using System.Text.Json;
using HaKafkaNet;
using HaKafkaNet.Implementations.AutomationBuilder;

namespace MyHome.Dev;

public class TestRegistry : IAutomationRegistry, IInitializeOnStartup
{
    IHaServices _services;
    IAutomationFactory _factory;
    IAutomationBuilder _builder;
    private TimeProvider _time;
    ILogger<TestRegistry> _logger;

    const string 
        Test_Switch = "input_boolean.test_switch",
        LEDS = "light.office_led_light",
        Light_Bars = "light.office_light_bars";

    public TestRegistry(IStartupHelpers startupHelpers, IHaServices services, TimeProvider time, ILogger<TestRegistry> logger) 
    {
        _services = services;
        _factory = startupHelpers.Factory;
        _builder = startupHelpers.Builder;
        this._time = time;
        _logger = logger;
    }

    public Task Initialize()
    {
        //_logger.LogInformation("Registry Initialize");
        return Task.CompletedTask;
    }

    public void Register(IRegistrar reg)
    {
        //reg.TryRegister(TestNewTimeHelper);
    }


    IAutomationBase NoOccupancy_for5min_TurnOff()
    {
        var minutesToLeaveOn = 5;

        return _builder.CreateSchedulable<int?>()
            .WithName("Turn off the kitchen lights")
            .WithDescription($"Turn off the kitchen lights when unoccupied for {minutesToLeaveOn} minutes")
            .MakeDurable()
            .WithTriggers(Sensor.EsphomekitchenmotionPresenceTargetCount)
            .While(sc => sc.New.State == 0)
            .For(TimeSpan.FromMinutes(minutesToLeaveOn))
            .WithExecution(ct => {
                return _services.Api.TurnOff(Light.KitchenLights);
            })
            .Build();
    }
}
