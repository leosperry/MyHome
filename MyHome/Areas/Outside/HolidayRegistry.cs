using System;
using HaKafkaNet;

namespace MyHome.Areas.Outside;

public class HolidayRegistry : IAutomationRegistry
{
    private readonly IStartupHelpers _helpers;
    private readonly IHaServices _services;

    public HolidayRegistry(IStartupHelpers helpers, IHaServices services)
    {
        this._helpers = helpers;
        this._services = services;
    }

    public void Register(IRegistrar reg)
    {
        reg.TryRegister(
            FrontHolidayLights_On,
            FrontHolidayLights_Off
        );
    }

    IAutomationBase FrontHolidayLights_On()
    {
        return _helpers.Builder.CreateSunAutomation(SunEventType.Set)
            .WithName("Turn on holiday lights")
            .WithDescription("on 30 min after sunset")
            .WithOffset(TimeSpan.FromMinutes(30))
            .WithExecution(async ct =>
            {
                await _services.Api.TurnOn(Switch.OutsideDualPlug);
            })
            .Build();        
    }

    IAutomationBase FrontHolidayLights_Off()
    {
        return _helpers.Builder.CreateSunAutomation(SunEventType.Midnight)
            .WithName("Turn off holiday lights")
            .WithDescription("at solar midnight")
            .WithExecution(async ct => {
                await _services.Api.TurnOff(Switch.OutsideDualPlug);
            })
            .Build();
    }
}
