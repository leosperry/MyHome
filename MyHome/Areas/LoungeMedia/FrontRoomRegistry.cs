using System;
using HaKafkaNet;

namespace MyHome.Areas.LoungeMedia;

public class FrontRoomRegistry : IAutomationRegistry
{
    private readonly IStartupHelpers _helpers;
    private readonly IHaServices _services;

    public FrontRoomRegistry(IStartupHelpers helpers, IHaServices services)
    {
        this._helpers = helpers;
        this._services = services;
    }

    public void Register(IRegistrar reg)
    {
        reg.TryRegister(() => _helpers.Factory.EntityOnOffWithAnother("binary_sensor.kazul_light_time_sensor", Devices.PlantPlug1)
            .WithMeta("Plant plug 1", "Uses Kazul's UVB schedule"));
    }

}
