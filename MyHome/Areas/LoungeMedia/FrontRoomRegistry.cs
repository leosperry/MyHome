using System;
using System.Diagnostics;
using HaKafkaNet;

namespace MyHome;

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
        reg.TryRegister(
            () => _helpers.Factory.EntityOnOffWithAnother(Binary_Sensor.KazulLightTimeSensor, Switch.PlantPlug1).WithMeta("Plant plug 1", "Uses Kazul's UVB schedule"),
            () => _helpers.Factory.EntityOnOffWithAnother(Light.FrontRoomLight, Switch.FrontRoomComputerLamp).WithMeta("Front Room Computer Lamp","Turn on/off the computer lamp wiht the room light"));
    }


}
