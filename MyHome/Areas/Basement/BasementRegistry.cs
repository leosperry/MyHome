using System.Text.Json;
using HaKafkaNet;
using MyHome.People;

namespace MyHome;

public class BasementRegistry : IAutomationRegistry
{
    readonly IHaServices _services;
    readonly IAutomationBuilder _builder;
    readonly IAutomationFactory _factory;
    private readonly AsherService _asherService;
    private readonly IUpdatingEntity<OnOff, JsonElement> _override;

    public BasementRegistry(AsherService asherService, IHaServices services, IStartupHelpers helpers)
    {
        _services = services;
        _builder = helpers.Builder;
        _factory = helpers.Factory;
        _asherService = asherService;
        _override = helpers.UpdatingEntityProvider.GetOnOffEntity(Helpers.BasementOverride);
    }

    public void Register(IRegistrar reg)
    {
        reg.TryRegister(() => _factory.EntityOnOffWithAnother(Sensors.BasementStairMotion, Lights.BasementStair)
            .WithMeta("Basement Stair motion", "set stair light with motion sensor"));

        reg.TryRegister(() => _factory.DurableAutoOn(Helpers.BasementOverride, TimeSpan.FromHours(6))
            .WithMeta("Auto off basement override"));

        reg.TryRegister(
            BasementMotion,
            DimBasementOverTime,
            CallAsherFromDashboard,
            UpdateOverrideDisplay);
    }

    IAutomationBase UpdateOverrideDisplay()
    {
        return _builder.CreateSimple<OnOff>()
            .WithName("Update basement override display")
            .WithTriggers(Helpers.BasementOverride)
            .WithExecution(async (sc, ct) => 
            {
                await _services.Api.SetZoozSceneControllerButtonColorFromOverrideState(
                    Lights.BasementStair, sc.New.State, 4, ct);
            })
            .Build();
    }

    IAutomationBase BasementMotion()
    {
        return _builder.CreateSimple<OnOff>()
            .WithName("Basement Motion")
            .WithDescription("turn on basement lights")
            .WithTriggers("binary_sensor.basement_motion_motion_detection")
            .WithExecution(async (sc, ct) =>{
                // if no motion or override is on , do nothing
                if (!sc.New.IsOn() || _override.IsOn()) return;

                if (await GetBasementAverageBrighness(ct) < Bytes._70pct)
                {
                    await _services.Api.LightSetBrightnessByLabel(Labels.BasementLights, Bytes._75pct);
                }
            })
            .Build();
    }

    IAutomationBase DimBasementOverTime()
    {
        return _builder.CreateSchedulable<OnOff>(true)
            .WithName("Dim Basement over time")
            .WithDescription("after 10 minutes, normalize light, then dim every minute until minimum")
            .MakeDurable()
            .WithTriggers(Sensors.BasementMotion)
            .While(sc => sc.IsOff())
            .For(TimeSpan.FromMinutes(10))
            .WithExecution(DimOverTime)
            .Build();
    }

    IAutomationBase CallAsherFromDashboard()
    {
        return _builder.CreateSimple<DateTime?>()
            .WithName("Call Asher from dashboard")
            .WithDescription("Uses a helpert to trigger random message to play to call Asher")
            .WithTriggers(Helpers.AsherDashboardButton)
            .WithExecution(async (sc, ct) =>
            {
                if (sc.New.StateAndLastUpdatedWithin1Second())
                {
                    await _asherService.PlayRandom(0.25f, ct);
                }
            })
            .Build();
    }

    async Task<byte> GetBasementAverageBrighness(CancellationToken ct)
    {
        var group = await _services.EntityProvider.GetLightEntity(Lights.BasementGroup);
        return group?.Attributes?.Brightness ?? throw new Exception("could not get basement group brightness");
        // var lights = await Task.WhenAll(
        //     _services.EntityProvider.GetLightEntity(Lights.Basement1, ct),
        //     _services.EntityProvider.GetLightEntity(Lights.Basement2, ct),
        //     _services.EntityProvider.GetLightEntity(Lights.BasementWork, ct)
        // );

        // var average = lights.Sum(l => (int)(l?.Attributes?.Brightness ?? 0)) / lights.Length;
        // return (byte)average;
    }

    private async Task DimOverTime(CancellationToken ct)
    {
        if (_override.IsOn()) return;
        
        try
        {
            var average = await GetBasementAverageBrighness(ct);
            await _services.Api.LightSetBrightnessByLabel(Labels.BasementLights, average);
            while (average > Bytes._25pct && !ct.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromMinutes(1), ct);
                await _asherService.DecreaseLights(ct);

                // var newB = (byte)(average - Bytes._10pct);
                // await _services.Api.LightSetBrightnessByLabel(Labels.BasementLights, newB, ct);
                average = await GetBasementAverageBrighness(ct);
            }
        }
        catch (Exception ex) when (ex is TaskCanceledException || ex is OperationCanceledException)
        {
            // someone walked by the sensor
        }
    }
}
