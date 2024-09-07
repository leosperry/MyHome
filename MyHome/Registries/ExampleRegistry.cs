using System;
using HaKafkaNet;

namespace MyHome.Registries;

public class ExampleRegistry : IAutomationRegistry
{
    readonly IAutomationFactory _factory;
    readonly IAutomationBuilder _builder;
    readonly IHaServices _services;

    public ExampleRegistry(IAutomationFactory factory, IAutomationBuilder builder, IHaServices services)
    {
        _factory = factory;
        _builder = builder;
        _services = services;
    }

    public void Register(IRegistrar reg)
    {
        var light = "light.dator_kontor1";
        var helper = "input_boolean.hakafkatest";

        var example1 = _factory.EntityOnOffWithAnother(helper, light);
        /*
        some short-hand nomenclature I use:
        sc - State Change
        ct - Cancellation Token 
        */

        var example2 = _factory.SimpleAutomation([helper], async (sc, ct) => {
            var onOff = sc.ToOnOff();
            if (onOff.New.State == OnOff.On)
            {
                await _services.Api.TurnOn(light);
            }
            else
            {
                await _services.Api.TurnOff(light);
            }
        });

        var example3 = _builder.CreateSimple()
            .WithTriggers(helper)
            .WithExecution(async (sc, ct) => {
                if (sc.New.State == "on")
                {
                    await _services.Api.TurnOn(light);
                }
                else
                {
                    await _services.Api.TurnOff(light);
                }            
            }).Build();

            //reg.Register(example1);
            // reg.Register(example2);
             //reg.Register(example3);
    }
}
