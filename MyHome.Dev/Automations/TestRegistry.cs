using HaKafkaNet;

namespace MyHome.Dev;

public class TestRegistry : IAutomationRegistry, IInitializeOnStartup
{
    IHaServices _services;
    IAutomationFactory _factory;
    IAutomationBuilder _builder;
    ILogger<TestRegistry> _logger;

    const string 
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
        _logger.LogInformation ("Registry Initialize");
        return Task.CompletedTask;
    }

    public void Register(IRegistrar reg)
    {
        reg.Register(SetTest());

        // reg.RegisterTyped(
        //     _builder.CreateSimpleTyped<OnOff, ColorLightModel>()
        //         .WithName("Test Combined")
        //         .WithDescription("this is my description")
        //         .WithTriggers("light.office_combined_light")
        //         .WithExecution(async(sc, ct) => {
        //             if (sc.New.IsOff())
        //             {
        //                 await _services.Api.TurnOff("light.office_light_bars", ct);
        //             }
        //             else
        //             {
        //                 await _services.Api.TurnOn("light.office_light_bars", ct);
        //             }
        //         })
        //         .Build()
        // );
    }

    IAutomation SetTest()
    {
        return _builder.CreateSimple()
            .WithName("test maker")
            .WithTriggers("light.my_combined_light")
            .WithExecution(async (sc, ct) => {
                var colorLightState = sc.ToColorLight().New;
                
                if (colorLightState.IsOn())
                {
                    await _services.Api.LightTurnOn(new LightTurnOnModel()
                    {
                        EntityId = [Light_Bars],
                        Brightness = colorLightState.Attributes?.Brightness,
                        RgbColor = colorLightState.Attributes?.RGB,
                        Kelvin = colorLightState.Attributes?.TempKelvin
                    });
                }
                else
                {
                    await _services.Api.TurnOff(Light_Bars);
                }
            })
            .Build();
    }


}
