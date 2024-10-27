﻿using HaKafkaNet;
using MyHome.Areas.Office;

namespace MyHome;

public class PeriodicRegistry : IAutomationRegistry
{
    readonly IHaServices _services;
    readonly IAutomationFactory _factory;
    readonly OfficeService _officeService;
    private readonly ILogger<PeriodicRegistry> _logger;

    public PeriodicRegistry(IHaServices services, IAutomationFactory factory, OfficeService officeService, ILogger<PeriodicRegistry> logger)
    {
        _services = services;
        _factory = factory;
        _officeService = officeService;
        this._logger = logger;
    }

    public void Register(IRegistrar reg)
    {
        reg.Register(_factory.SimpleAutomation(["schedule.periodic_schedule"], this.Periodic)
            .WithMeta("periodic", "ensure switches are off"));
    }
    
    // runs at top and bottom of every hour
    Task Periodic(HaEntityStateChange stateChange, CancellationToken ct)
    {
        return Task.WhenAll(
            EnforceOff(Helpers.BedTime, ct),
            EnforceOff(GarageService.GARAGE1_DOOR_OPENER, ct),
            EnforceOff(GarageService.GARAGE2_DOOR_OPENER, ct),
            _officeService.PeriodicTasks()
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

    async Task MakeSureReplaySetToZero()
    {
        var audible_alert_to_play = await _services.EntityProvider.GetFloatEntity(Helpers.AudibleAlertToPlay);
        if (audible_alert_to_play?.State != 0)
        {
            _logger.LogWarning(Helpers.AudibleAlertToPlay + " was not set to zero");
            await _services.Api.InputNumberSet(Helpers.AudibleAlertToPlay, 0);
        }
    }
}
