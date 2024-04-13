using HaKafkaNet;

namespace MyHome;

public class PeriodicRegistry : IAutomationRegistry
{
    readonly IHaServices _services;
    readonly IAutomationFactory _factory;
    readonly IAutomationBuilder _builder;
    readonly LightAlertModule _lam;

    public PeriodicRegistry(IHaServices services, IAutomationFactory factory, IAutomationBuilder builder, LightAlertModule lam)
    {
        _services = services;
        _factory = factory;
        _builder = builder;
        _lam = lam;
    }

    public void Register(IRegistrar reg)
    {
        reg.Register(_factory.SimpleAutomation(["schedule.periodic_schedule"], this.Periodic)
            .WithMeta("periodic", "ensure switches are off"));

        reg.RegisterMultiple(
            _factory.SunRiseAutomation(this.Sunrise).WithMeta("Sunrise", "turn off couch 1"),
            _factory.SunRiseAutomation(
                async ct => {
                    await _services.Api.TurnOff(Helpers.LivingRoomOverride, ct);
                    _lam.ConfigureStandByBrightness(Bytes.PercentToByte(9));
                }, 
                TimeSpan.FromHours(1))
                .WithMeta("re-enable living room lights","1 hour after sunrise, monkey standby brighter")
        );

        reg.Register(_factory.SunSetAutomation(ct => {
            _lam.ConfigureStandByBrightness(Bytes.PercentToByte(6));
            return Task.CompletedTask;
        }).WithMeta("Dusk", "dim monkey standby"));

        reg.Register(_factory.SunDuskAutomation(ct => {
            _lam.ConfigureStandByBrightness(Bytes.PercentToByte(3));
            return Task.CompletedTask;
        }).WithMeta("Dusk", "dim monkey standby"));
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
            await _services.Api.PersistentNotification($"{entityId} was left on", ct);
            await _services.Api.TurnOff(entityId);
        }
    }
}
