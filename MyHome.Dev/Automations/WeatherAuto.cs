// using System;
// using System.Text.Json.Serialization;
// using HaKafkaNet;


// namespace MyHome.Automations;

// public class WeatherAlerts : IAutomation<int, WeatherAlertsAttributes>, IAutomationMeta, IFallbackExecution
// {
//     readonly IHaServices _services;
//     readonly ILogger<WeatherAlerts> _logger;
//     readonly AutomationMetaData _meta;

//     public EventTiming EventTimings { get => EventTiming.Durable | EventTiming.PreStartupSameAsLastCached; }

//     public WeatherAlerts(IHaServices services, ILogger<WeatherAlerts> logger)
//     {
//         _services = services;
//         _logger = logger;


//         _meta = new AutomationMetaData()
//         {
//             Name = "Weather Alerts",
//             Description = "Alert the family on Warnings"
//         };
//     }

//     public async Task FallbackExecute(Exception ex, HaEntityStateChange stateChange, CancellationToken ct)
//     {
//         await _services.Api.NotifyGroupOrDevice("mobile_app_leonard_phone", "Got a weather alert that didn't parse");
//     }

//     public AutomationMetaData GetMetaData() => _meta;

//     public IEnumerable<string> TriggerEntityIds()
//     {
//         yield return "sensor.nws_alerts";
//         yield return "sensor.nws_alerts_2";
//     }

//     public async Task Execute(HaEntityStateChange<HaEntityState<int, WeatherAlertsAttributes>> stateChange, CancellationToken ct)
//     {
//         if (stateChange.New.State > 0)
//         {
//             var atts = stateChange.New.Attributes?.Alerts?[0];
//             _logger.LogInformation(atts?.ToString() ?? "nope");
//         }
//         await Task.CompletedTask;
//     }
// }

// public record WeatherAlertsAttributes
// {
//     public WeatherAlert[]? Alerts { get; set; }  

//     [JsonPropertyName("friendly_name")]
//     public string? FriendlyName { get; set; }  
// }

// public record WeatherAlert
// {
//     public string? Event { get; set; }
//     public Guid? ID { get; set; }
//     public string? URL { get; set; }
//     public string? Headline { get; set; }
//     public string? Type { get; set; }
//     public string? Status { get; set; }
//     public string? Severity { get; set; }
//     public string? Certainty { get; set; }
//     public DateTime? Sent { get; set; }
//     public DateTime? Onset { get; set; }
//     public DateTime? Expires { get; set; }
//     public DateTime? Ends { get; set; }
//     public string? AreasAffected { get; set; }
//     public string? Description { get; set; }
//     public string? Instruction { get; set; }
    

// }
