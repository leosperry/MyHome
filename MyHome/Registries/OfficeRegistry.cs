using HaKafkaNet;

namespace MyHome;

public class OfficeRegistry : IAutomationRegistry
{
    readonly IHaServices _services;
    readonly IAutomationFactory _factory;
    readonly IAutomationBuilder _builder;
    readonly NotificationSender _notifyOffice;

    public OfficeRegistry(IHaServices services, IAutomationFactory factory, IAutomationBuilder builder, INotificationService notificationService)
    {
        _services = services;
        _factory = factory;
        _builder = builder;

        var channel = notificationService.CreateAudibleChannel([MediaPlayers.Office], Voices.Mundane);
        _notifyOffice = notificationService.CreateNotificationSender([channel]);
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
            .WithExecution((sc, ct) => _notifyOffice($"override is {sc.New.State}"))
            .Build());
    }

    private async Task OfficeFan(HaEntityStateChange change, CancellationToken token)
    {
        var tempState = change.ToFloatTyped();
        var officeMotion = await _services.EntityProvider.GetOnOffEntity(Sensors.OfficeMotion, token);
        if (tempState.BecameGreaterThan(83f) && officeMotion?.State == OnOff.On)
        {
            await _services.Api.TurnOn(Devices.OfficeFan);
        }
        else
        {
            await _services.Api.TurnOff(Devices.OfficeFan);
        }
    }
}
