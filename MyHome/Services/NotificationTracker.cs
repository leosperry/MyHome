using System;
using System.Data;
using HaKafkaNet;

namespace MyHome.Services;

public interface INotificationObserver
{
    void OnNotificationSent(string message);
    Task Init();
}

public class NotificationObserver : INotificationObserver
{
    LinkedList<string> _notifications = new();
    private readonly IHaServices _services;

    public NotificationObserver(IHaServices services)
    {
        this._services = services;
    }

    public async Task Init()
    {
        for (int i = 3; i >= 1; i--)
        {
            var msg = (await _services.EntityProvider.GetEntity($"input_text.audible_alert_{i}"))?.State;
            if(msg is not null) Add(msg);
        }
    }

    public void OnNotificationSent(string message)
    {
        Add(message);
        Update();
    }

    private async void Update()
    {
        int index = 1;
        foreach (var item in _notifications)
        {
            await _services.Api.InputTextSet($"input_text.audible_alert_{index}", item);
            index++;
        }
    }

    private void Add(string message)
    {
        _notifications.AddFirst(message);
        if (_notifications.Count > 3)
        {
            _notifications.RemoveLast();
        }
    }
}
