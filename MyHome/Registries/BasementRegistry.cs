using HaKafkaNet;

namespace MyHome;

public class BasementRegistry : IAutomationRegistry
{
    readonly IHaServices _services;
    readonly IAutomationBuilder _builder;
    readonly IAutomationFactory _factory;

    public BasementRegistry(IHaServices services, IAutomationBuilder builder, IAutomationFactory factory)
    {
        _services = services;
        _builder = builder;
        _factory = factory;
    }

    public void Register(IRegistrar reg)
    {
        reg.Register(_factory.EntityOnOffWithAnother(Sensors.BasementStairMotion, Lights.BasementStair)
            .WithMeta("Basement Stair motion", "set stair light with motion sensor"));

        reg.Register(_builder.CreateSimple()
            .WithName("Basement Motion")
            .WithDescription("turn on basement lights")
            .WithTriggers("binary_sensor.basement_motion_motion_detection")
            .WithExecution(async (sc, ct) =>{
                if (await GetBasementAverageBrighness(ct) < Bytes._70pct)
                {
                    await _services.Api.LightSetBrightness([Lights.Basement1, Lights.Basement2, Lights.BasementWork], Bytes._75pct);
                }
            })
            .Build());

        //dim over time
        reg.Register(_builder.CreateSchedulable(true)
            .WithName("Dim Basement over time")
            .WithDescription("after 10 minutes, normalize light, then dim every minute until minimum")
            .MakeDurable()
            .WithTriggers(Sensors.BasementMotion)
            .GetNextScheduled((sc, ct) => {
                var motion = sc.ToOnOff();
                return Task.FromResult<DateTime?>(motion.New.State == OnOff.Off ? motion.New.LastUpdated.AddMinutes(10): null);
            })
            .WithExecution(DimOverTime)
            .Build());

    }

    async Task<byte> GetBasementAverageBrighness(CancellationToken ct)
    {
        var lights = await Task.WhenAll(
            _services.EntityProvider.GetLightEntity(Lights.Basement1, ct),
            _services.EntityProvider.GetLightEntity(Lights.Basement2, ct),
            _services.EntityProvider.GetLightEntity(Lights.BasementWork, ct)
        );

        var average = lights.Sum(l => (int)(l?.Attributes?.Brightness ?? 0)) / lights.Length;
        return (byte)average;
    }

    private async Task DimOverTime(CancellationToken ct)
    {
        var average = await GetBasementAverageBrighness(ct);
        await _services.Api.LightSetBrightness([Lights.Basement1, Lights.Basement2, Lights.BasementWork], average);
        while (average > Bytes._25pct && !ct.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromMinutes(1), ct);
            var newB = (byte)(average - Bytes._10pct);
            await _services.Api.LightSetBrightness([Lights.Basement1, Lights.Basement2, Lights.BasementWork], newB, ct);
            average = await GetBasementAverageBrighness(ct);
        }
    }
}
