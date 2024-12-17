using System.Collections.Concurrent;
using System.Text.Json;
using HaKafkaNet;

namespace MyHome;

public class LightAlertModule : IDisposable
{
    readonly IHaApiProvider _api;
    private readonly IHaEntity<OnOff, JsonElement> _officeMotion;
    readonly ILogger<LightAlertModule> _logger;

    readonly CancellationTokenSource _cancelSource = new();
    readonly PeriodicTimer _timer;
    Task? _timerTask;
    SemaphoreSlim _sem = new(1);
    DateTime _lastUpdate = DateTime.Now;

    LinkedListNode<(string, LightTurnOnModel)>? _current;
    LinkedList<(string, LightTurnOnModel)> _alerts =  new();

    ConcurrentQueue<(string, LightTurnOnModel)> _newItems = new();
    ConcurrentDictionary<string, object?> _itemsToRemove = new();

    static LightTurnOnModel _standby = new()
    {
        //ColorName = "gold",
        Brightness = Bytes._10pct,
        RgbColor = (255, 215, 2)
    };    

    public async Task Start()
    {
        // set it once to ensure state in cache
        await SetStandby();
        StartTracking();
    }

    public LightAlertModule(IHaApiProvider api, IStartupHelpers helpers, ILogger<LightAlertModule> logger)
    {
        _api = api;
        this._officeMotion = helpers.UpdatingEntityProvider.GetOnOffEntity(Binary_Sensor.OfficeMotionMotion);
        _logger = logger;
        _timer = new PeriodicTimer(TimeSpan.FromSeconds(5));
        StartTracking();
    }

    public void ConfigureStandByBrightness(Byte brightness)
    {
        _standby.Brightness = brightness;
        _standby.EntityId = GetAlertLights();
        _api.LightTurnOn(_standby);
    }

    public void Add(NotificationId id, LightTurnOnModel options)
    {
        _logger.LogWithStack("Adding Notification {notification_id}", id.Value);
        _newItems.Enqueue((id, options));
        _ = RunNow();
    }

    public void Clear(NotificationId id)
    {
        _logger.LogWithStack("Clearing Notification {notification_id}", id.Value);
        _itemsToRemove.TryAdd(id, null);
        _ = RunNow();
    }

    public void ClearAll()
    {
        _logger.LogWithStack("Clearing All Notifications");
        foreach (var item in _alerts)
        {
            _itemsToRemove.TryAdd(item.Item1, null);
        }
        _ = RunNow();
    }

    private IEnumerable<string> GetAlertLights()
    {
        if (_officeMotion.IsOn())
        {
            yield return Light.OfficeLedLight;
        }
        yield return Light.MonkeyLight;
    }

    private async Task SetStandby()
    {
        _logger.LogInformation("setting standby. settings : {standby}", _standby);
        await _api.LightTurnOn(_standby);
    }

    private async Task RunNow()
    {
        await _sem.WaitAsync();
        try
        {
            await Run();
        }
        finally
        {
            _sem.Release();
        }
    }

    /// <summary>
    /// should only be called when locked via _sem.
    /// therefore, runs atomically
    /// </summary>
    /// <returns></returns>
    private async Task Run()
    {
        _lastUpdate = DateTime.Now;
    
        if (_newItems.TryDequeue(out var newItem))
        {
            if (_current is null) // no active alerts
            {
                await SetCurrent(_alerts.AddFirst(newItem));
            }
            else
            {
                await SetCurrent(_alerts.AddAfter(_current, newItem));
            }
            // set the light
            return;
        }
        var currentTrack = _current;
        if (_itemsToRemove.Any())
        {
            var itemToTest = _alerts.First;
            
            while (itemToTest is not null)
            {
                var next = itemToTest.Next;
                if (_itemsToRemove.ContainsKey(itemToTest.Value.Item1))
                {                    
                    if (currentTrack == itemToTest)
                    {
                        //eventually we will move to next
                        //try to protect
                        currentTrack = itemToTest.Previous ?? _alerts.Last;
                    }
                    
                    // we need to remove it
                    _alerts.Remove(itemToTest);
                }
                itemToTest = next;
            }
            _itemsToRemove.Clear();        
        }

        var itemToSet = currentTrack?.Next ?? _alerts.First;
        await SetCurrent(itemToSet);
    }

    private async Task SetCurrent(LinkedListNode<(string, LightTurnOnModel)>? itemToSet)
    {
        try
        {
            if (itemToSet is null)
            {
                await SetStandby();
            } 
            else if (_current is null || itemToSet != _current)
            {
                var settings = itemToSet.Value.Item2;
                settings.EntityId = GetAlertLights();
                settings.Transition = 2;

                await _api.LightTurnOn(itemToSet.Value.Item2);
            }
            _current = itemToSet;
        }
        catch (System.Exception ex)
        {
            _logger.LogError(ex, "problem setting light alert");
            throw;
        }
    }

    private void StartTracking()
    {
        _timerTask = TimerTick();
    }

    private async Task TimerTick()
    {
        try
        {
            while (await _timer.WaitForNextTickAsync(_cancelSource.Token)
                && !_cancelSource.IsCancellationRequested)
            {
                await _sem.WaitAsync();
                try
                {
                    var diff = (DateTime.Now - _lastUpdate).TotalMilliseconds;
                    if (_alerts.Count <= 1 || diff < 4000)
                    {
                        // either we already set the light when 
                        // the alert came in or a
                        // new item came in. 
                        // in either case, we don't need to change it
                        continue;
                    }
                    await Run();
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex, "Critical Failure in LAM");
                }
                finally
                {
                    _sem.Release();
                }
            }
            _logger.LogCritical("LAM telemetry lost");
        }
        catch (OperationCanceledException){}
    }

    public void Dispose()
    {
        _cancelSource.Cancel();
        if (_timerTask is not null)
        {
            _timerTask.Wait();
        }
        
        _timer.Dispose();
        _cancelSource.Dispose();
    }
}

