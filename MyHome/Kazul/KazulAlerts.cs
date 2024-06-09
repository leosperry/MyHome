using HaKafkaNet;

namespace MyHome;

/// <summary>
/// Kazul is the name of our bearded dragon
/// This automation is set up to alert us when one of his heatter bulbs dies
/// The ceramic heater gives off no light at night so we can not visually seen when it burns out
/// Additional sensors are used to make sure he has good conditions
/// </summary>
public class KazulAlerts : IAutomation, IAutomationMeta
{
    public const string TEMP = "sensor.kazul_temp_humidity_air_temperature";
    public const string TEMP_BATTERY = "sensor.kazul_temp_humidity_battery_level";
    public const string CERAMIC_SWITCH = "switch.kazul_power_strip_1";
    public const string CERAMIC_POWER = "sensor.kazul_power_strip_electric_consumption_w_1";
    public const string HALOGEN_SWITCH = "switch.kazul_power_strip_2";
    public const string HALOGEN_POWER = "sensor.kazul_power_strip_electric_consumption_w_2";

    Dictionary<string, (string id, string name)> _powerToSwitchMapping = new()
    {
        {CERAMIC_POWER, (CERAMIC_SWITCH, "ceramic heater")},
        {HALOGEN_POWER, (HALOGEN_SWITCH, "halogen lamp")}
    };

    private readonly IHaApiProvider _api;
    private readonly IHaEntityProvider _entities;
    private readonly NotificationSender _notifyCritical;

    public KazulAlerts(IHaApiProvider api, IHaEntityProvider entities, INotificationService notificationService)
    {
        _api = api;
        _entities = entities;
        _notifyCritical = notificationService.GetCritical();
    }

    public IEnumerable<string> TriggerEntityIds()
    {
        return [TEMP, TEMP_BATTERY, CERAMIC_POWER, HALOGEN_POWER];
    }

    public Task Execute(HaEntityStateChange stateChange, CancellationToken cancellationToken)
    {
        return stateChange.EntityId switch 
        {
            TEMP => CheckTemp(stateChange.New),
            TEMP_BATTERY => BatteryCheck(stateChange.New),
            CERAMIC_POWER => CheckPower(stateChange.New),
            HALOGEN_POWER => CheckPower(stateChange.New),
            _ => throw new Exception("Kazul Automation Fault")
        };
    }

    private Task BatteryCheck(HaEntityState state)
    {
        float? batteryLevel = null;
        if (state.Bad() || (batteryLevel = state.GetState<float?>()) < 25f)
        {
            return _notifyCritical($"Kazul temp sensor battery rerports {batteryLevel?.ToString() ?? "unknown"} percent");
        }
        return Task.CompletedTask;
    }

    private Task CheckTemp(HaEntityState state)
    {
        float? temp = null;
        if(state.Bad() || (temp = state.GetState<float?>()) < 65f)
        {
            return _notifyCritical($"Kazul temerature reports {temp?.ToString() ?? "unknown"} degrees");
        }
        return Task.CompletedTask;
    }

    private async Task CheckPower(HaEntityState state)
    {
        var switchState = await _entities.GetOnOffEntity(_powerToSwitchMapping[state.EntityId].id);

        if (switchState.Bad() || (switchState!.State == OnOff.On && state.GetState<float?>() < 75f))
        {
            await _notifyCritical($"problem with Kazul {_powerToSwitchMapping[state.EntityId].name}");
        }
    }

    public AutomationMetaData GetMetaData()
    {
        return new AutomationMetaData()
        {
            Name = "Kazul alerts",
            Description = "Ensure Kazul's environment is healthy",
            AdditionalEntitiesToTrack = [
                CERAMIC_SWITCH, HALOGEN_SWITCH
            ]
        };
    }
}
