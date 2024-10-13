using System;
using HaKafkaNet;

namespace MyHome.Dev;

public class SystemMonitor : ISystemMonitor
{
    public Task BadEntityStateDiscovered(BadEntityState badState)
    {
        return Task.CompletedTask;
    }

    public Task StateHandlerInitialized()
    {
        return Task.CompletedTask;
    }

    public Task UnhandledException(AutomationMetaData automationMetaData, Exception exception)
    {
        return Task.CompletedTask;
    }

    // public Task InitializationFailure(InitializationError[] errors)
    // {
    //     //throw new NotImplementedException();
    //     return Task.CompletedTask;
    // }
}
