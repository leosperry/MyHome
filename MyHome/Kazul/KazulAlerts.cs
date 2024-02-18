using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
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
    public const string NOTIFY_GROUP = "critical_notification_group";

    Dictionary<string, (string id, string name)> _powerToSwitchMapping = new()
    {
        {CERAMIC_POWER, (CERAMIC_SWITCH, "ceramic heater")},
        {HALOGEN_POWER, (HALOGEN_SWITCH, "halogen lamp")}
    };

    LightTurnOnModel _lightAlert = new LightTurnOnModel()
    {
        EntityId = ["light.living_lamp_1"],
        RgbColor = (255, 0, 255),
        Flash = Flash.Long
    };

    private readonly IHaApiProvider _api;
    private readonly IHaEntityProvider _entities;

    public KazulAlerts(IHaApiProvider api, IHaEntityProvider entities)
    {
        _api = api;
        _entities = entities;
    }

    public IEnumerable<string> TriggerEntityIds()
    {
        return [TEMP, TEMP_BATTERY, CERAMIC_POWER, HALOGEN_POWER];
    }

    public Task Execute(HaEntityStateChange stateChange, CancellationToken cancellationToken)
    {
        return (stateChange.EntityId) switch 
        {
            TEMP => CheckTemp(stateChange.New, cancellationToken),
            TEMP_BATTERY => BatteryCheck(stateChange.New, cancellationToken),
            CERAMIC_POWER => CheckPower(stateChange.New, cancellationToken),
            HALOGEN_POWER => CheckPower(stateChange.New, cancellationToken),
            _ => throw new Exception("Kazul Automation Fault")
        };
    }

    private Task BatteryCheck(HaEntityState state, CancellationToken cancellationToken)
    {
        if (state.Bad() || state.GetState<float?>() < 50f)
        {
            return Task.WhenAll(
                _api.NotifyGroupOrDevice(NOTIFY_GROUP, "check Kazul temp sensor battery", cancellationToken),
                _api.LightTurnOn(_lightAlert, cancellationToken));
        }
        return Task.CompletedTask;
    }

    private Task CheckTemp(HaEntityState state, CancellationToken cancellationToken)
    {
        if(state.Bad() || state.GetState<float?>() < 65f)
        {
            return Task.WhenAll(
                _api.NotifyGroupOrDevice(NOTIFY_GROUP, "Kazul's enclosure temerature either can't be read or is below 60"),
                _api.LightTurnOn(_lightAlert, cancellationToken));
        }
        return Task.CompletedTask;
    }

    private async Task CheckPower(HaEntityState state, CancellationToken cancellationToken)
    {
        var switchState = await _entities.GetOnOffEntity(_powerToSwitchMapping[state.EntityId].id, cancellationToken);

        if (switchState.Bad() || (switchState?.State == OnOff.On && state.GetState<double?>() < 75))
        {
            await Task.WhenAll(
                _api.NotifyGroupOrDevice(NOTIFY_GROUP, $"problem with Kazul {_powerToSwitchMapping[state.EntityId].name}"),
                _api.LightTurnOn(_lightAlert, cancellationToken));
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
