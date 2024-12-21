using System.Text.Json;
using HaKafkaNet;
using MyHome.Services;

namespace MyHome;

/// <summary>
/// Kazul is the name of our bearded dragon
/// This automation is set up to alert us when one of his heatter bulbs dies
/// The ceramic heater gives off no light at night so we can not visually seen when it burns out
/// Additional sensors are used to make sure he has good conditions
/// </summary>
public class KazulAlerts : IAutomation, IAutomationMeta
{
    public const string 
        TEMP = Sensor.KazulTempHumidityAirTemperature,
        TEMP_BATTERY = Sensor.KazulTempHumidityBatteryLevel,
        CERAMIC_SWITCH = Switch.KazulPowerStrip1,
        CERAMIC_POWER = Sensor.KazulPowerStripElectricConsumptionW,
        HALOGEN_SWITCH = Switch.KazulPowerStrip2,
        HALOGEN_POWER = Sensor.KazulPowerStripElectricConsumptionW2;

    Dictionary<string, (string id, string name)> _powerToSwitchMapping = new()
    {
        {CERAMIC_POWER, (CERAMIC_SWITCH, "ceramic heater")},
        {HALOGEN_POWER, (HALOGEN_SWITCH, "halogen lamp")}
    };
    private readonly IHaEntityProvider _entities;
    private readonly NotificationSender _notifyCritical;
    private readonly NotificationSender _notifyInformational;
    private readonly IHaEntity<OnOff, JsonElement> _maintenanceMode;

    public KazulAlerts(IHaEntityProvider entities, INotificationService notificationService, IUpdatingEntityProvider updatingEntityProvider)
    {
        _entities = entities;
        _notifyCritical = notificationService.GetCritical();
        _notifyInformational = notificationService.CreateInformationalSender();
        this._maintenanceMode = updatingEntityProvider.GetOnOffEntity(Input_Boolean.MaintenanceMode);
    }


    public IEnumerable<string> TriggerEntityIds()
    {
        return [TEMP, TEMP_BATTERY, CERAMIC_POWER, HALOGEN_POWER];
    }

    public async Task Execute(HaEntityStateChange stateChange, CancellationToken cancellationToken)
    {
        if (_maintenanceMode.IsOn())
        {
            return;
        }
        await internalExecute(stateChange);
    }

    private Task internalExecute(HaEntityStateChange stateChange)
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
        if (state.Bad() || (batteryLevel = state.GetState<float?>()) < 20f)
        {
            return _notifyInformational($"Kazul temp sensor battery rerports {batteryLevel?.ToString() ?? "unknown"} percent");
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
            ],
            TriggerOnBadState = true
        };
    }
}
