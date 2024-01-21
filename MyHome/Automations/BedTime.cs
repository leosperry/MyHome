﻿using HaKafkaNet;

namespace MyHome;

public class BedTime : IAutomation
{
    private readonly IGarageService _garageService;
    private readonly ILogger<BedTime> _logger;

    public BedTime(IGarageService garageService, ILogger<BedTime> logger)
    {
        this._garageService = garageService;
        this._logger = logger;
    }

    public Task Execute(HaEntityStateChange stateChange, CancellationToken cancellationToken)
    {
        if (stateChange.New.State == "on")
        {
            _logger.LogInformation("bed time triggered");
            return RunBedtimeRoutine(cancellationToken);
        }
        return Task.CompletedTask;
    }

    public IEnumerable<string> TriggerEntityIds()
    {
        yield return "input_boolean.bedtime_switch";
    }

    Task RunBedtimeRoutine(CancellationToken cancellationToken)
    {
        return Task.WhenAll(
            _garageService.EnsureGarageClosed(cancellationToken)
        );
    }
}
