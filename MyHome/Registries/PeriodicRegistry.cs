using HaKafkaNet;

namespace MyHome;

public class PeriodicRegistry : IAutomationRegistry
{
    readonly IHaServices _services;
    readonly IAutomationFactory _factory;
    readonly IAutomationBuilder _builder;

    public PeriodicRegistry(IHaServices services, IAutomationFactory factory, IAutomationBuilder builder)
    {
        _services = services;
        _factory = factory;
        _builder = builder;
    }

    public void Register(IRegistrar reg)
    {
        reg.Register(_factory.SimpleAutomation(["schedule.periodic_schedule"], this.Periodic)
            .WithMeta("periodic", "ensure switches are off"));

        reg.RegisterMultiple(
            _factory.SunRiseAutomation(this.Sunrise).WithMeta("Sunrise", "turn off couch 1"),
            _factory.SunDawnAutomation(
                ct => _services.Api.TurnOff(Helpers.LivingRoomOverride, ct), 
                TimeSpan.FromHours(1))
                .WithMeta("re-enable living room lights","1 hour after sunrise")
        );
    }

    Task Sunrise(CancellationToken ct) => _services.Api.TurnOff([Lights.Couch1], ct);
    
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
            await _services.Api.PersistentNotification($"{entityId} was left on");
            await _services.Api.TurnOff(entityId);
        }
    }
}
