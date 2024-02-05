using HaKafkaNet;

namespace MyHome;

/// <summary>
/// most of the automations for bed time run in Home Assistant
/// One of the more complicated routines is ensuring the garage doors are closed
/// </summary>
public class BedTime : IAutomation, IAutomationMeta
{
    private readonly IGarageService _garageService;
    private readonly ILogger<BedTime> _logger;

    public BedTime(IGarageService garageService, ILogger<BedTime> logger)
    {
        this._garageService = garageService;
        this._logger = logger;
    }

    public Task Execute(HaEntityStateChange stateChange, CancellationToken cancellationToken)
    {
        if (stateChange.New.State == "on")
        {
            _logger.LogInformation("bed time triggered");
            return RunBedtimeRoutine(cancellationToken);
        }
        return Task.CompletedTask;
    }

    public AutomationMetaData GetMetaData()
    {
        return new AutomationMetaData()
        {
            Name = "Bed Time",
            Description = "Ensure garage closed",
            AdditionalEntitiesToTrack = [
                GarageService.BACK_HALL_LIGHT,
                GarageService.GARAGE1_CONTACT,
                GarageService.GARAGE1_DOOR_OPENER,
                GarageService.GARAGE1_TILT,
                GarageService.GARAGE2_CONTACT,
                GarageService.GARAGE2_DOOR_OPENER,
                GarageService.GARAGE2_TILT
            ]
        };
    }

    public IEnumerable<string> TriggerEntityIds()
    {
        yield return "input_boolean.bedtime_switch";
    }

    Task RunBedtimeRoutine(CancellationToken cancellationToken)
    {
        return Task.WhenAll(
            _garageService.EnsureGarageClosed(cancellationToken)
        );
    }
}
