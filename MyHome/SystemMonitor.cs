using System.Text;
using System.Text.RegularExpressions;
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
        var filteredStates = badStates.Where(bs => !bs.EntityId.StartsWith("event"));
        if (!filteredStates.Any())
        {
            return Task.CompletedTask;
        }
        StringBuilder sb = new();
        sb.AppendLine("bad entity states");
        foreach (var item in filteredStates)
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
        return Task.WhenAll(
            _services.Api.PersistentNotification(sb.ToString(), default),
            _services.Api.NotifyGroupOrDevice(Phones.LeonardPhone, "Bad Entity Discovered."));
    }

    public Task StateHandlerInitialized()
    {
        return Task.CompletedTask;
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
}
