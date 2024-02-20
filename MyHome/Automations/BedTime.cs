using HaKafkaNet;

namespace MyHome;

/// <summary>
/// most of the automations for bed time run in Home Assistant
/// One of the more complicated routines is ensuring the garage doors are closed
/// </summary>
public class BedTime : IAutomation, IAutomationMeta
{
    private readonly IHaServices _services;
    private readonly IGarageService _garageService;
    private readonly ILogger<BedTime> _logger;

    public BedTime(IHaServices services, IGarageService garageService, ILogger<BedTime> logger)
    {
        this._services = services;
        this._garageService = garageService;
        this._logger = logger;
    }

    public Task Execute(HaEntityStateChange stateChange, CancellationToken cancellationToken)
    {
        if (stateChange.New.GetStateEnum<OnOff>() == OnOff.On)
        {
            return RunBedtimeRoutine(cancellationToken);
        }
        return Task.CompletedTask;
    }

    public IEnumerable<string> TriggerEntityIds()
    {
        yield return Helpers.BedTime;
    }

    async Task RunBedtimeRoutine(CancellationToken ct)
    {
        IEnumerable<Task> taskList = [
            _garageService.EnsureGarageClosed(ct),
            EnsureOfficeClosed(ct),
            _services.Api.LightSetBrightness(Lights.EntryLight, Bytes._40pct ,ct),
            _services.Api.LightSetBrightness(Lights.Couch1, Bytes._10pct),
            _services.Api.TurnOff([
                Lights.FrontRoomLight, Lights.LoungeCeiling, Lights.UpstairsHall, 
                Lights.Couch2, Lights.Couch3, Lights.TvBacklight, Lights.PeacockLamp, Devices.Roku,
                Lights.KitchenLights, Lights.DiningRoomLights,
                Lights.OfficeLeds, Lights.OfficeLights, Devices.OfficeFan,
                Lights.BackFlood, Lights.BackPorch, Lights.FrontPorchLight
            ], ct)];
        
        try
        {
            await Task.WhenAll(taskList);
        }
        catch (System.Exception ex)
        {
            _logger.LogError(ex, "Bedtime failure.");
            throw;
        }
    }

    async Task EnsureOfficeClosed(CancellationToken ct)
    {
        var officeDoor = await _services.EntityProvider.GetOnOffEntity(Devices.OfficeDoor, ct);
        if (officeDoor.Bad() || officeDoor!.State == OnOff.On)
        {
            await Task.WhenAll(
                _services.Api.NotifyAlexaMedia("The office is open", [Alexa.MainBedroom, Alexa.Kitchen]),
                _services.Api.TurnOn(Lights.BackHallLight, ct));
        }
    }

    public AutomationMetaData GetMetaData()
    {
        return new AutomationMetaData()
        {
            Name = "Bed Time",
            Description = "Ensure garage and office are closed, Turn on entryway light. Turn off all other lights/devices",
            AdditionalEntitiesToTrack = [
                GarageService.BACK_HALL_LIGHT,
                GarageService.GARAGE1_CONTACT,
                GarageService.GARAGE1_DOOR_OPENER,
                GarageService.GARAGE1_TILT,
                GarageService.GARAGE2_CONTACT,
                GarageService.GARAGE2_DOOR_OPENER,
                GarageService.GARAGE2_TILT,
                Lights.EntryLight
            ]
        };
    }}
