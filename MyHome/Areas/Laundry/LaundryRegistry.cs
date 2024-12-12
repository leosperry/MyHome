using System;
using HaKafkaNet;

namespace MyHome;

public class LaundryRegistry : IAutomationRegistry
{
    private IStartupHelpers _helpers;
    private IHaServices _services;

    public LaundryRegistry(IStartupHelpers helpers, IHaServices services)
    {
        this._helpers = helpers;
        this._services = services;
    }
    public void Register(IRegistrar reg)
    {
        reg.TryRegister(CoatCloset);
    }

    IAutomationBase CoatCloset()
    {
        const int seconds = 5;
        return _helpers.Builder.CreateSimple<OnOff>()
            .WithName("Coat Closet Door")
            .WithDescription($"Turn on the back hall light when the coat closet opens, and turn it off {seconds} seconds after closing")
            .WithTriggers(Sensors.BackHallCoatClosetContact)
            .WithExecution(async (sc, ct) =>
            {
                if (sc.IsOn())
                {
                    await _services.Api.TurnOn(Lights.BackHallLight);
                }
                else
                {
                    await Task.Delay(TimeSpan.FromSeconds(seconds));
                    await _services.Api.TurnOff(Lights.BackHallLight);
                }
            })
            .Build();
    }
}
