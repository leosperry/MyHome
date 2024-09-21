using System;
using HaKafkaNet;
using Microsoft.VisualBasic;

namespace MyHome.Automations;

public class WeatherAlerts : IAutomation, IAutomationMeta
{
    readonly IHaServices _services;
    readonly ILogger<WeatherAlerts> _logger;
    readonly AutomationMetaData _meta;
    readonly NotificationSender _alertCritical;

    Dictionary<string, WeatherAlertAttributes> _weatherAlerts = new();

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

    public async Task Execute(HaEntityStateChange stateChange, CancellationToken ct)
    {
        var alertState = stateChange.ToOnOff<WeatherAlertAttributes>();
        var alert = alertState.New.Attributes;
        if (alert?.alert_id is not null)
        {
            bool isNew = _weatherAlerts.ContainsKey(alert.alert_id);
            _weatherAlerts[alert.alert_id] = alert;
            if (isNew && alert.alert_event?.ToLower().Contains("warning") == true)
            {
                await _alertCritical(alert.spoken_message ?? alert.spoken_title ?? alert.alert_title ?? "check weather alerts", alert.alert_id);
            }
        }
        else
        {
            _logger.LogWarning("Could not parse weather alert");
        }
    }

    public AutomationMetaData GetMetaData() => _meta;

    public IEnumerable<string> TriggerEntityIds()
    {
        //yield return "sensor.weatheralerts_1_active_alerts";
        yield return "sensor.weatheralerts_1_alert_1";
        yield return "sensor.weatheralerts_1_alert_2";
        yield return "sensor.weatheralerts_1_alert_3";
        yield return "sensor.weatheralerts_1_alert_4";
        yield return "sensor.weatheralerts_1_alert_5";
    }
}


internal class WeatherAlertAttributes
{
    public string? alert_id { get; set; }
    public string? alert_event { get; set; }
    public string? alert_area { get; set; }
    public string? alert_NWSheadline { get; set; }
    public string? alert_description { get; set; }
    public string? alert_messageType { get; set; }
    public string? alert_status { get; set; }
    public string? alert_category { get; set; }
    public string? alert_urgency { get; set; }
    public string? alert_severity { get; set; }
    public string? alert_certainty { get; set; }
    public string? alert_response { get; set; }
    public string? alert_instruction { get; set; }
    public string? alert_sent { get; set; }
    public string? alert_effective { get; set; }
    public string? alert_onset { get; set; }
    public string? alert_expires { get; set; }
    public string? alert_title { get; set; }
    public string? display_title { get; set; }
    public string? alert_zoneid { get; set; }
    public string? display_message { get; set; }
    public string? spoken_title { get; set; }
    public string? spoken_message { get; set; }
    /*
alert_id: null
alert_event: null
alert_area: null
alert_NWSheadline: null
alert_description: null
alert_messageType: null
alert_status: null
alert_category: null
alert_urgency: null
alert_severity: null
alert_certainty: null
alert_response: null
alert_instruction: null
alert_sent: null
alert_effective: null
alert_onset: null
alert_expires: null
alert_title: null
display_title: null
alert_zoneid: null
display_message: null
spoken_title: null
spoken_message: null
icon: hass:alert-rhombus
friendly_name: Weather Alert 1
    */
}