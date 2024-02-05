using System.Text;
using Confluent.Kafka;
using HaKafkaNet;

namespace MyHome.Dev;

public class SystemMonitor : ISystemMonitor
{
    readonly IHaServices _services;
    public SystemMonitor(IHaServices services)
    {
        _services = services;
    }

    public Task BadEntityStateDiscovered(IEnumerable<BadEntityState> badStates)
    {
        StringBuilder sb = new();
        sb.AppendLine("bad entity states");
        foreach (var item in badStates)
        {
            if (item.State is null)
            {
                sb.AppendLine($"{item.EntityId} could not be found");            
            }
            else
            {
                sb.AppendLine($"{item.EntityId} has a state of {item.State.State}");
            }
        }
        return _services.Api.PersistentNotification(sb.ToString());
    }

    public Task StateHandlerInitialized() => Task.CompletedTask;

    public Task UnhandledException(AutomationMetaData automationMetaData, Exception exception)
    {
        return _services.Api.PersistentNotification($"automation of type: [{automationMetaData.UnderlyingType}] failed with [{exception.Message}]");
    }
}
