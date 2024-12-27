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
        _override = helpers.UpdatingEntityProvider.GetOnOffEntity(Input_Boolean.BasementOverride);
    }

    public void Register(IRegistrar reg)
    {
        reg.TryRegister(() => _factory.EntityOnOffWithAnother(Binary_Sensor.BasementStairMotionAq2Motion2, Switch.BasementStairLight)
            .WithMeta("Basement Stair motion", "set stair light with motion sensor"));

        reg.TryRegister(() => _factory.DurableAutoOff(Input_Boolean.BasementOverride, TimeSpan.FromHours(6))
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
            .WithTriggers(Input_Boolean.BasementOverride)
            .WithExecution(async (sc, ct) => 
            {
                await _services.Api.SetZoozSceneControllerButtonColorFromOverrideState(
                    Switch.BasementStairLight, sc.New.State, 4, ct);
            })
            .Build();
    }

    IAutomationBase BasementMotion()
    {
        return _builder.CreateSimple<OnOff>()
            .WithName("Basement Motion")
            .WithDescription("turn on basement lights")
            .WithTriggers(Binary_Sensor.BasementMotionMotionDetection)
            .WithExecution(async (sc, ct) =>{
                // if no motion or override is on , do nothing
                if (!sc.New.IsOn() || _override.IsOn()) return;

                if (await GetBasementAverageBrightness(ct) < Bytes._70pct)
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
            .WithTriggers(Binary_Sensor.BasementMotion2Motion)
            .While(sc => sc.IsOff())
            .For(TimeSpan.FromMinutes(10))
            .WithExecution(DimOverTime)
            .Build();
    }

    IAutomationBase CallAsherFromDashboard()
    {
        return _builder.CreateSimple<DateTime?>()
            .WithName("Call Asher from dashboard")
            .WithDescription("Uses a helper to trigger random message to play to call Asher")
            .WithTriggers(Input_Button.AsherButton)
            .WithExecution(async (sc, ct) =>
            {
                if (sc.New.StateAndLastUpdatedWithin1Second())
                {
                    await _asherService.PlayRandom(0.25f, ct);
                }
            })
            .Build();
    }

    async Task<byte> GetBasementAverageBrightness(CancellationToken ct)
    {
        var group = await _services.EntityProvider.GetLightEntity(Light.BasementLightGroup);
        return group?.Attributes?.Brightness ?? throw new Exception("could not get basement group brightness");
    }

    private async Task DimOverTime(CancellationToken ct)
    {
        if (_override.IsOn()) return;
        
        try
        {
            var average = await GetBasementAverageBrightness(ct);
            await _services.Api.LightSetBrightnessByLabel(Labels.BasementLights, average);
            while (average > Bytes._25pct && !ct.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromMinutes(1), ct);
                await _asherService.DecreaseLights(ct);

                // var newB = (byte)(average - Bytes._10pct);
                // await _services.Api.LightSetBrightnessByLabel(Labels.BasementLights, newB, ct);
                average = await GetBasementAverageBrightness(ct);
            }
        }
        catch (Exception ex) when (ex is TaskCanceledException || ex is OperationCanceledException)
        {
            // someone walked by the sensor
        }
    }
}
