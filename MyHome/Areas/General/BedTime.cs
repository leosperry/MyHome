using System.Text.Json;
using HaKafkaNet;

namespace MyHome;

/// <summary>
/// most of the automations for bed time run in Home Assistant
/// One of the more complicated routines is ensuring the garage doors are closed
/// </summary>
public class BedTime : IAutomation<OnOff>, IAutomationMeta
{
    private readonly IHaServices _services;
    private readonly IGarageService _garageService;
    
    readonly NotificationSender _notify;

    private readonly ILogger<BedTime> _logger;


    public BedTime(IHaServices services, IGarageService garageService, INotificationService notification, ILogger<BedTime> logger)
    {
        this._services = services;
        this._garageService = garageService;
        this._logger = logger;

        var voiceChannel = notification.CreateAudibleChannel([Media_Player.DiningRoomSpeaker]);
        this._notify = notification.CreateNotificationSender([voiceChannel]);
    }

    public Task Execute(HaEntityStateChange<HaEntityState<OnOff, JsonElement>> stateChange, CancellationToken ct)
    {
        if (stateChange.IsOn())
        {
            return RunBedtimeRoutine(ct);
        }
        return Task.CompletedTask;
    }

    public IEnumerable<string> TriggerEntityIds()
    {
        yield return Input_Boolean.BedtimeSwitch;
    }

    async Task RunBedtimeRoutine(CancellationToken ct)
    {
        var couch1 = Light.WizRgbwTunable79A59C;

        Task[] taskList = [
            _services.Api.TurnOff(Input_Boolean.BedtimeSwitch),
            _services.Api.LockLock(Lock.AqaraSmartLockU100, ct),
            _garageService.EnsureGarageClosed(_notify,ct),
            EnsureOfficeClosed(ct),
            _services.Api.TurnOn([Switch.MbrFloorLights], ct),
            _services.Api.LightSetBrightness(Light.EntryLight, Bytes._20pct ,ct),
            _services.Api.LightSetBrightness(couch1, Bytes._10pct),
            _services.Api.TurnOffByLabel("bedtimeoff"),
            ];
        try
        {
            await Task.WhenAll(taskList);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Bedtime failure.");
            await _notify("Bedtime routine failure");
            throw;
        }
    }

    async Task EnsureOfficeClosed(CancellationToken ct)
    {
        var officeDoor = await _services.EntityProvider.GetOnOffEntity(Binary_Sensor.OfficeDoorOpening, ct);
        if (officeDoor.Bad() || officeDoor!.State == OnOff.On)
        {
            await Task.WhenAll(
                _notify("The office is open"),
                _services.Api.TurnOn(Switch.BackHallLight, ct));
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
                Light.EntryLight
            ]
        };
    }


}
