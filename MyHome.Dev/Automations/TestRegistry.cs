﻿using System.Text.Json;
using HaKafkaNet;

namespace MyHome.Dev;

public class TestRegistry : IAutomationRegistry, IInitializeOnStartup
{
    IHaServices _services;
    IAutomationFactory _factory;
    IAutomationBuilder _builder;
    ILogger<TestRegistry> _logger;

    const string 
        Test_Switch = "input_boolean.test_switch",
        LEDS = "light.office_led_light",
        Light_Bars = "light.office_light_bars";

    public TestRegistry(IStartupHelpers startupHelpers, IHaServices services, ILogger<TestRegistry> logger) 
    {
        _services = services;
        _factory = startupHelpers.Factory;
        _builder = startupHelpers.Builder;
        _logger = logger;
    }

    public Task Initialize()
    {
        _logger.LogInformation("Registry Initialize");
        return Task.CompletedTask;
    }

    public void Register(IRegistrar reg)
    {
        // var simpleAutomation = _builder.CreateSimple()
        //     .WithTriggers("switch.my_switch")
        //     .WithName("This is my simple automation")
        //     .WithExecution(async (sc, ct) => {
        //         // do work
        //         return;
        //     })
        //     .Build();

        //reg.Register(simpleAutomation);

        var typedDurable = _builder.CreateSchedulable<OnOff>()
            .WithTriggers("input_boolean.my_motion_sensor")
            .WithName("Turn off the lights when no one is around")
            .WithDescription("an optional description")
            .While(sc => sc.IsOff())
            .ForMinutes(30)  
            .WithExecution(ct => _services.Api.TurnOffByLabel("my_label", ct))
            .MakeDurable()
            .Build();

        reg.TryRegister(typedDurable);
    }

    // IAutomationBase Simple()
    // {
    //     return _builder.CreateSimple()
    //         .WithTriggers("switch.my_switch")
    //         .WithName("This is my simple automation")
    //         .WithExecution(async (sc, ct) => {
    //             // do work
    //             return;
    //         })
    //         .Build();
    // }

    IAutomationBase SimpleTyped()
    {
        string name = "Simple Typed";
        return _builder.CreateSimple<OnOff>()
            .WithTriggers(Test_Switch)
            .WithName(name)
            .WithExecution((sc, ct) => {
                _logger.LogInformation(name);
                return Task.CompletedTask;
            })
            .Build();
    }

    IAutomationBase SimpleTyped2()
    {
        string name = "Simple Typed 2";
        return _builder.CreateSimple<OnOff>()
            .WithTriggers(Test_Switch)
            .WithName(name)
            .WithExecution((sc, ct) => {
                _logger.LogInformation(name);
                return Task.CompletedTask;
            })
            .Build();
    }

    

    IAutomationBase Conditional()
    {
        string name = "Conditional";
        return _builder.CreateConditional()
            .WithTriggers(Test_Switch)
            .WithName(name)
            .When((sc, ct) => {
                return Task.FromResult<bool>(sc.ToOnOff().IsOn());
            })
            .ForSeconds(3)
            .WithExecution((ct) => {
                _logger.LogInformation(name);
                return Task.CompletedTask;
            })
            .Build();
    }

    IAutomationBase ConditionalTyped()
    {
        string name = "Conditional Typed";
        return _builder.CreateConditional<OnOff>()
            .WithTriggers(Test_Switch)
            .WithName(name)
            .When((sc, ct) => {
                return Task.FromResult<bool>(sc.IsOn());
            })
            .ForSeconds(3)
            .WithExecution((ct) => {
                _logger.LogInformation(name);
                return Task.CompletedTask;
            })
            .Build();    

        }
    
    IAutomationBase ConditionalTyped2()
    {
        string name = "Conditional Typed2";
        return _builder.CreateConditional<OnOff, JsonElement>()
            .WithTriggers(Test_Switch)
            .WithName(name)
            .When((sc, ct) => {
                return Task.FromResult<bool>(sc.IsOn());
            })
            .ForSeconds(3)
            .WithExecution((ct) => {
                _logger.LogInformation(name);
                return Task.CompletedTask;
            })
            .Build();  
    }
    
    IAutomationBase Scheduled()
    {
        string name = "Scheduled";
        return _builder.CreateSchedulable()
            .WithTriggers(Test_Switch)
            .WithName(name)
            .While((sc) => {
                return sc.ToOnOff().IsOn();
            })
            .ForSeconds(3)
            .WithExecution((ct) => {
                _logger.LogInformation(name);
                return Task.CompletedTask;
            })
            .Build();
    }

    IAutomationBase ScheduledTyped()
    {
        string name = "Scheduled Typed";
        return _builder.CreateSchedulable<OnOff?>()
            .WithTriggers(Test_Switch)
            .WithName(name)
            .While((sc) => {
                return true;
            })
            .ForSeconds(3)
            .WithExecution((ct) => {
                _logger.LogInformation(name);
                return Task.CompletedTask;
            })
            .Build();
    }

    IAutomationBase ScheduledTyped2()
    {
        string name = "Scheduled Typed 2";
        return _builder.CreateSchedulable<OnOff, JsonElement>()
            .WithTriggers(Test_Switch)
            .WithName(name)
            .GetNextScheduled((sc, ct) => {
                if (sc.IsOn())
                {
                    return Task.FromResult<DateTime?>(DateTime.Now.AddSeconds(3));
                }
                return Task.FromResult<DateTime?>(null);
            })
            .WithExecution((ct) => {
                _logger.LogInformation(name);
                return Task.CompletedTask;
            })
            .Build();
    }

}
