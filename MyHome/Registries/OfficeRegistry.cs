using HaKafkaNet;

namespace MyHome;

public class OfficeRegistry : IAutomationRegistry
{
    readonly IHaServices _services;
    readonly IAutomationFactory _factory;
    readonly IAutomationBuilder _builder;

    public OfficeRegistry(IHaServices services, IAutomationFactory factory, IAutomationBuilder builder)
    {
        _services = services;
        _factory = factory;
        _builder = builder;
    }
    
    public void Register(IRegistrar reg)
    {
        reg.Register(_factory.DurableAutoOffOnEntityOff([Devices.OfficeFan, Lights.OfficeLights, Lights.OfficeLeds], Sensors.OfficeMotion, TimeSpan.FromMinutes(10))
            .WithMeta("Office Light Off","10 minutes"));

        reg.Register(_builder.CreateSimple()
            .WithName("Office Fan")
            .WithDescription("Turn on fan when it gets warm")
            .WithTriggers(Sensors.OfficeTemp)
            .WithExecution(this.OfficeFan)
            .WithAdditionalEntitiesToTrack(Devices.OfficeFan)
            .Build());
        
        reg.Register(_builder.CreateSimple()
            .WithName("report office overrid status")
            .WithDescription("when office override changes, report on alexa")
            .WithTriggers(Helpers.OfficeOverride)
            .WithExecution((sc, ct) => _services.Api.NotifyAlexaMedia($"override is {sc.New.State}", [Alexa.Office], ct))
            .Build());
    }

    private async Task OfficeFan(HaEntityStateChange change, CancellationToken token)
    {
        var tempState = change.ToDoubleTyped();
        var officeMotion = await _services.EntityProvider.GetOnOffEntity(Sensors.OfficeMotion, token);
        if (tempState.New.State > 83 && officeMotion?.State == OnOff.On)
        {
            await _services.Api.TurnOn(Devices.OfficeFan);
        }
        else
        {
            await _services.Api.TurnOff(Devices.OfficeFan);
        }
    }
}
