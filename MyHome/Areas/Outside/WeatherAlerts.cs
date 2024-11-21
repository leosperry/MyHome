using System.Text.Json.Serialization;
using HaKafkaNet;

namespace MyHome.Automations;

public class WeatherAlerts : IAutomation<int, WeatherAlertsAttributes>, IAutomationMeta, IFallbackExecution
{
    readonly IHaServices _services;
    readonly ILogger<WeatherAlerts> _logger;
    readonly AutomationMetaData _meta;
    readonly NotificationSender _alertCritical;

    public EventTiming EventTimings { get => EventTiming.Durable | EventTiming.PreStartupSameAsLastCached; }

    public WeatherAlerts(IHaServices services, ILogger<WeatherAlerts> logger, INotificationService notificationService)
    {
        _services = services;
        _logger = logger;

        _alertCritical = notificationService.GetCritical();

        _meta = new AutomationMetaData()
        {
            Name = "Weather Alerts",
            Description = "Alert the family on Warnings"
        };
    }

    public async Task FallbackExecute(Exception ex, HaEntityStateChange stateChange, CancellationToken ct)
    {
        await _services.Api.NotifyGroupOrDevice(Phones.LeonardPhone, "Got a weather alert that didn't parse");
    }

    public AutomationMetaData GetMetaData() => _meta;

    public IEnumerable<string> TriggerEntityIds()
    {
        yield return "sensor.nws_alerts";
        yield return "sensor.nws_alerts_2";
    }

    public async Task Execute(HaEntityStateChange<HaEntityState<int, WeatherAlertsAttributes>> stateChange, CancellationToken ct)
    {
        var alerts = stateChange.New.Attributes?.Alerts;
        if (alerts?.Length > 0)
        {
            foreach (var alert in alerts)
            {
                if (alert.ID is not null)
                {
                    var alertId = alert.ID.ToString()!;
                    var cached = await _services.Cache.GetUserDefinedObject<WeatherAlert>(alertId);
                    HandleNewWeatherAlert(cached, alert);
                    await _services.Cache.SetUserDefinedObject(alertId, alert);
                }                
            }
        }
        
        await Task.CompletedTask;
    }

    private void HandleNewWeatherAlert(WeatherAlert? cached, WeatherAlert alert)
    {
        if (cached is null)
        {
            // it's a new one
            if ( alert.Certainty >= WeatherAlertCertainty.Likely && alert.Severity >= WeatherAlertSeverity.Severe)
            {
                _alertCritical(alert.Description, alert.Headline ?? "Severe Weather Alert", new NotificationId(alert.ID.ToString()!));
            }
        }
    }
}

public record WeatherAlertsAttributes
{
    public WeatherAlert[]? Alerts { get; set; }  

    [JsonPropertyName("friendly_name")]
    public string? FriendlyName { get; set; }  
}

public record WeatherAlert
{
    public string? Event { get; set; }
    public Guid? ID { get; set; }
    public string? URL { get; set; }
    public string? Headline { get; set; }
    public WeatherAlertType? Type { get; set; }
    public WeatherAlertStatus? Status { get; set; }
    public WeatherAlertSeverity? Severity { get; set; }
    public WeatherAlertCertainty? Certainty { get; set; }
    public DateTime? Sent { get; set; }
    public DateTime? Onset { get; set; }
    public DateTime? Expires { get; set; }
    public DateTime? Ends { get; set; }
    public string? AreasAffected { get; set; }
    public required string Description { get; set; }
    public string? Instruction { get; set; }
    public WeatherAlertResponseType? Response { get; set; }

    public string? Urgency { get; set; }

    public string? Category { get; set; }
}

public enum WeatherAlertSeverity
{
    Unknown = 0,
    Minor = 1,
    Moderate = 2,
    Severe = 3,
    Extreme = 4
}

public enum WeatherAlertResponseType
{
    Shelter = 8,
    Evacuate = 7,
    Prepare = 6,
    Execute = 5,
    Avoid = 4,
    Monitor = 3,
    Assess = 2,
    AllClear = 1,
    None = 0
}

public enum WeatherAlertCertainty
{
    Unknown = 0,
    Unlikely = 1,
    Possible = 2,
    Likely = 3,
    Observed = 4
}

public enum WeatherAlertStatus
{
    Actual, Exercise, System, Test, Draft
}

public enum WeatherAlertType
{
    Alert, Update, Cancel, Ack, Error
}

public enum WeatherAlertUrgency
{
    Unknown,
    Immediate, Expected, Future, Past, 
}
