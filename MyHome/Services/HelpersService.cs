using System;
using HaKafkaNet;

namespace MyHome.Services;

public class HelpersService
{
    private readonly IHaServices _services;
    private readonly ILogger<HelpersService> _logger;

    public HelpersService(IHaServices haServices, ILogger<HelpersService> logger)
    {
        this._services = haServices;
        this._logger = logger;
    }

    public Task<bool> MaintenanceModeIsOn()
    {
        return ToggleIsOn(Helpers.MaintenanceMode);
    }

    private async Task<bool> ToggleIsOn(string entity_id)
    {
        try
        {
            var maintenanceHelper = await _services.EntityProvider.GetOnOffEntity(entity_id);
            return !maintenanceHelper.Bad() && maintenanceHelper.IsOn();
        }
        catch (System.Exception ex)
        {
            _logger.LogError(ex, "could not get state of diagnostic helper");
        }
        return false;    }
}
