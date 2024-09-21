using HaKafkaNet;

namespace MyHome;

public class PeriodicRegistry : IAutomationRegistry
{
    readonly IHaServices _services;
    readonly IAutomationFactory _factory;

    public PeriodicRegistry(IHaServices services, IAutomationFactory factory)
    {
        _services = services;
        _factory = factory;
    }

    public void Register(IRegistrar reg)
    {
        reg.Register(_factory.SimpleAutomation(["schedule.periodic_schedule"], this.Periodic)
            .WithMeta("periodic", "ensure switches are off"));
    }
    
    Task Periodic(HaEntityStateChange stateChange, CancellationToken ct)
    {
        return Task.WhenAll(
            EnforceOff(Helpers.BedTime, ct),
            EnforceOff(GarageService.GARAGE1_DOOR_OPENER, ct),
            EnforceOff(GarageService.GARAGE2_DOOR_OPENER, ct)
        );
    }

    async Task EnforceOff(string entityId, CancellationToken ct)
    {
        // hopefully we can get rid of this
        var state = await _services.EntityProvider.GetOnOffEntity(entityId, ct);
        if (!state.Bad() && state!.State == OnOff.On)
        {
            await _services.Api.PersistentNotification($"{entityId} was left on", ct);
            await _services.Api.TurnOff(entityId);
        }
    }
}
