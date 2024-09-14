using HaKafkaNet;

namespace MyHome;

public class LaundryAlerts : IAutomation, IAutomationMeta
{
    INotificationService _notifications;
    IHaEntityProvider _entityProvider;

    public const string
        DryerDoor = "binary_sensor.dv102683g_laundry_door",
        DryerEndOfCycle = "binary_sensor.dv102683g_laundry_end_of_cycle",
        DryerTimeRemaining = "sensor.dv102683g_laundry_time_remaining",
        DryerState = "sensor.dv102683g_laundry_machine_state",
        WasherDoor = "binary_sensor.av339078n_laundry_door",
        WasherEndOfCycle = "binary_sensor.av339078n_laundry_end_of_cycle",
        WasherTimeRemaining = "sensor.av339078n_laundry_time_remaining",
        WasherSoap = "sensor.av339078n_laundry_washer_smart_dispense_tank_status",
        WasherState = "sensor.av339078n_laundry_machine_state";
    
    NotificationId _laundryId = new NotificationId("laundry");
    NotificationId _lowSoapId = new NotificationId("washing-machine-low-soap");
    ILogger _logger;
    NotificationSender _regularAlert;
    NotificationSender _soapAlert;


    public LaundryAlerts(IHaEntityProvider entityProvider, INotificationService notifications, ILogger<LaundryAlerts> logger)
    {
        _entityProvider = entityProvider;
        _notifications = notifications;
        _logger = logger;

        var monkeyChannel = _notifications.CreateMonkeyChannel(new()
        {
            EntityId = [Lights.Monkey],
            ColorName = "aquamarine",
            Brightness = Bytes._30pct
        });
        var audioChannel = _notifications.CreateAudibleChannel([MediaPlayers.Kitchen, MediaPlayers.LivingRoom]);
        _regularAlert = _notifications.CreateNotificationSender([audioChannel], [monkeyChannel]);

        _soapAlert = _notifications.CreateInformationalSender();
    }

    public IEnumerable<string> TriggerEntityIds()
    {
        return [
            DryerDoor,
            DryerEndOfCycle,
            //DryerTimeRemaining,
            DryerState,
            WasherDoor,
            WasherEndOfCycle, 
            //WasherTimeRemaining,
            WasherSoap,
            WasherState
        ];
    }

    public Task Execute(HaEntityStateChange stateChange, CancellationToken ct)
    {
        return stateChange.EntityId switch
        {
            DryerDoor => DoorAction(stateChange, ct),
            DryerEndOfCycle => EndOfCycle(stateChange, ct),
            DryerState => StateUpdate(stateChange, ct),
            WasherDoor => DoorAction(stateChange, ct),
            WasherEndOfCycle => EndOfCycle(stateChange, ct),
            WasherState => StateUpdate(stateChange, ct),
            WasherSoap => SoapLevelUpdate(stateChange, ct),
            _ => Task.CompletedTask
        };
    }

    Task DoorAction(HaEntityStateChange stateChange, CancellationToken ct)
    {
        // off means closed
        var onOff = stateChange.ToOnOff();
        if (onOff.TurnedOn())
        {
            _notifications.Clear(_laundryId);    
        }
        return Task.CompletedTask;
    }

    Task EndOfCycle(HaEntityStateChange stateChange, CancellationToken ct)
    {
        _logger.LogDebug($"end of cyle state: {stateChange.New.State}");
        return Task.CompletedTask;
    }

    async Task StateUpdate(HaEntityStateChange stateChange, CancellationToken ct)
    {
        if (stateChange.Old?.State == "Run" && stateChange.New.State == "Finished")
        {
            if (stateChange.EntityId == DryerState)
            {
                await _regularAlert("The Dryer is done", id: _laundryId);
            }
            else if (stateChange.EntityId == WasherState)
            {
                //check the dryer state
                var dryerState = await _entityProvider.GetEntity(DryerState);
                if (dryerState?.State == "Run")
                {
                    await _regularAlert("The Washer is done, but the dryer is still running.", id: _laundryId);
                }
                else
                {
                    await _regularAlert("Wet clothes need to move to the dryer", id: _laundryId);
                }
            }
        }
    }

    async Task SoapLevelUpdate(HaEntityStateChange stateChange, CancellationToken ct)
    {
        
        if (stateChange.New.State.ToLower() == "low")
        {
            await _soapAlert("The Washing Mashine is hungry for detergent", "Wahing Machine Low Soap", _lowSoapId);
        }
        else
        {
            await _notifications.Clear(_lowSoapId);
        }
    }

    AutomationMetaData _meta = new()
    {
        Name = "Laundry Alerts",
    };
    public AutomationMetaData GetMetaData() => _meta;
}
