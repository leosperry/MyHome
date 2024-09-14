using HaKafkaNet;

namespace MyHome.Dev;

public class SystemMonitor : ISystemMonitor
{
    readonly IHaServices _services;
    readonly LightAlertModule _lam;
    readonly ILogger<SystemMonitor> _logger;

    private bool _sendBadStateEvents = true;

    public SystemMonitor(IHaServices services, LightAlertModule lam, ILogger<SystemMonitor> logger)
    {
        _services = services;
        _lam = lam;
        _logger = logger;
    }

    public Task BadEntityStateDiscovered(BadEntityState badStates)
    {
        if (badStates.EntityId.StartsWith("event"))
        {
            return Task.CompletedTask;
        }

        if (_sendBadStateEvents)
        {
            var message = $"Bad Entity State{Environment.NewLine}{badStates.EntityId} has a state of {badStates?.State?.State ?? "null"}";

            return Task.WhenAll(
                _services.Api.PersistentNotification(message, default),
                _services.Api.NotifyGroupOrDevice(Phones.LeonardPhone, "Bad Entity Discovered."));
        }
        return Task.CompletedTask;
    }

    public async Task StateHandlerInitialized()
    {
        await _lam.Start();
        _logger.LogInformation("State Handler Initialized");
        
        // this is jenky and will not handle all circumstances
        // var logResponse = await _services.Api.GetErrorLog();
        // if (logResponse.StatusCode == System.Net.HttpStatusCode.OK)
        // {
        //     Regex kafkaErrorCheck = new Regex(@"(\d{4}-\d{2}-\d{2}\s\d{2}:\d{2}:\d{2}\.\d{3})\sERROR.*/apache_kafka/.*/aiokafka/", RegexOptions.Singleline);

        //     var match = kafkaErrorCheck.Match(await logResponse.Content.ReadAsStringAsync());
        //     if (match.Success)
        //     {   
        //         Console.WriteLine($"Home Assistant kafka error detected at {DateTime.Parse(match.Groups[1].Value)}");
        //         await _services.Api.PersistentNotification("Kafka Error detected");
        //         await Task.Delay(1000); // attempt to let the notification persist before restart
        //         //await _services.Api.RestartHomeAssistant();
        //     }
        // }
    }

    public Task UnhandledException(AutomationMetaData automationMetaData, Exception exception)
    {
        return Task.WhenAll(
            _services.Api.PersistentNotification($"automation: [{automationMetaData.Name}] failed with [{exception.Message}]", default),
            _services.Api.NotifyGroupOrDevice(Phones.LeonardPhone, $"Uncaught Automation Error in {automationMetaData.Name}"));
    }

    public Task HaNotificationUpdate(HaNotification notification, CancellationToken ct)
    {
        if (notification.UpdateType == "removed" && notification.Id is not null)
        {
            _lam.Clear(new NotificationId(notification.Id));
        }    
        return Task.CompletedTask;
    }

    public async Task HaStartUpShutDown(StartUpShutDownEvent evt, CancellationToken ct)
    {
        _logger.LogInformation("Home Assistant {HaEvent}", evt.Event);
        const int three_min = 3 * 60 * 1000;

        await evt.ShutdownStartupActions(
            () => _sendBadStateEvents = false, 
            ()  => _sendBadStateEvents = true , 
            three_min, _logger);
    }
}
