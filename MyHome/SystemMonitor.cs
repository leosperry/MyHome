using System.Text.Json;
using HaKafkaNet;
using MyHome.Services;

namespace MyHome;

public class SystemMonitor : ISystemMonitor
{
    readonly IHaServices _services;
    readonly LightAlertModule _lam;
    readonly ILogger<SystemMonitor> _logger;
    private readonly IHaEntity<OnOff, JsonElement> _maintenanceMode;
    private bool _sendBadStateEvents = true;

    /// <summary>
    /// hack
    /// </summary>
    public static DateTime StartTime { get; private set; }

    public SystemMonitor(IHaServices services, LightAlertModule lam, IStartupHelpers helpers, ILogger<SystemMonitor> logger)
    {
        _services = services;
        _lam = lam;
        _logger = logger;
        this._maintenanceMode = helpers.UpdatingEntityProvider.GetOnOffEntity(Input_Boolean.MaintenanceMode);
    }

    public async Task BadEntityStateDiscovered(BadEntityState badStates)
    {
        var maintenanceModeIsOn = false;
        try
        {
            maintenanceModeIsOn = _maintenanceMode.IsOn();
        }
        catch (System.Exception)
        {
            _logger.LogInformation("maintenance mode was not initialilzed");
        }
        if (badStates.EntityId.StartsWith("event") || maintenanceModeIsOn)
        {
            return;
        }

        if (_sendBadStateEvents)
        {
            var message = $"{badStates.EntityId} has a state of {badStates?.State?.State ?? "null"}";

            await Task.WhenAll(
                _services.Api.PersistentNotification("Bad Entity State", message, default),
                _services.Api.NotifyGroupOrDevice(Phones.LeonardPhone, "Bad Entity Discovered."));
        }
    }

    public async Task StateHandlerInitialized()
    {
        StartTime = DateTime.Now; //hack

        await _lam.Start();
        _logger.LogInformation("State Handler Initialized");
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

    public async Task HaApiResponse(HaServiceResponseArgs args, CancellationToken ct)
    {
        if (_maintenanceMode.IsOn())
        {
            return;
        }

        _logger.LogError("HA service response:{response_code}, reason:{reason}, data:{data}, exception {exception}", args.Response?.StatusCode, args.Response?.ReasonPhrase, args.Data, args.Exception);

        string title = "HA Service call failed.";
        string message = $"{args.Domain}.{args.Service} was sent {args.Data}";

        if(!(args.Domain == "notify" && args.Service == "persistent_notification"))
        {
            // dont send persistent if failure was on persistent
            await _services.Api.PersistentNotificationDetail(message, title);
        }

        if (!(args.Domain == "notify" && args.Service == "mobile_app_leonard_phone"))
        {
            // dont send to leonard if failure was to leonard
            await _services.Api.NotifyGroupOrDevice(Phones.LeonardPhone, message, title);
        }
    }

    public async Task AutomationTypeConversionFailure(IAutomationBase auto, HaEntityStateChange sc, CancellationToken ct)
    {
        if(sc.New.State != "unknown" && sc.New.State != "unavailable")
        {
            string? automationName = null;
            if (auto is IAutomationMeta automationMeta)
            {
                automationName = automationMeta.GetMetaData().Name;
            }
            _logger.LogError("Type Conversion Failure in {automation_name} of type {automation_type}. State: {failed_state}", auto.GetType().Name, automationName, sc);
            await _services.Api.NotifyGroupOrDevice(Phones.LeonardPhone, "Type conversion failed","Breaking Change alert");

        }
    }
}
