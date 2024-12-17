using System.Text.Json;
using HaKafkaNet;

namespace MyHome;

public class OfficeRegistry : IAutomationRegistry, IInitializeOnStartup
{
    readonly IHaServices _services;
    readonly IStartupHelpers _helpers;
    private readonly OfficeService _officeService;
    private readonly IHaEntity<OnOff, JsonElement> _officeMotion;

    public OfficeRegistry(IHaServices services, IStartupHelpers helpers, 
        OfficeService officeService)
    {
        _services = services;
        _helpers = helpers;
        _officeService = officeService;
        _officeMotion = _helpers.UpdatingEntityProvider.GetOnOffEntity(Binary_Sensor.OfficeMotionMotion);
    }

    public async Task Initialize()
    {
        var trackerState = await _services.EntityProvider.GetLightEntity(Light.OfficeCombinedLight);
        if (!trackerState.Bad())
        {
            _officeService.Init(trackerState.Attributes?.Brightness ?? 127);
        }
    }

    public void Register(IRegistrar reg)
    {
        var exceptions = reg.TryRegister(
            DynamicallySetLights,
            DynamicallySetLightsFromTarget,
            ReportOverrideStatus,
            OfficeFan,
            TurnOffAllOfficeWithSwitch,
            DyanicallyAdjustWithSwitch,
            NoMotion
        );
    }

    private IAutomation<DateTime?, SceneControllerEvent> TurnOffAllOfficeWithSwitch()
    {
        return _helpers.Builder.CreateSimple<DateTime?, SceneControllerEvent>()
            .WithName("Turn Off all office with switch")
            .WithTriggers("event.office_display_lights_scene_001")
            .WithExecution(async (sc, ct) => {
                if(sc.New.StateAndLastUpdatedWithin1Second())
                {
                    await _officeService.TurnOff();
                }
            })
            .Build();
    }

    private IAutomation<DateTime?, SceneControllerEvent> DyanicallyAdjustWithSwitch()
    {
        return _helpers.Builder.CreateSimple<DateTime?, SceneControllerEvent>()
            .WithName("Dynamically set when switch turned on")
            .WithTriggers("event.office_display_lights_scene_002")
            .WithExecution(async (sc, ct) => {
                if(sc.New.StateAndLastUpdatedWithin1Second())
                {
                    await _officeService.SetLights(false ,ct);
                }
            })
            .Build();   
    }

    IAutomation<float, JsonElement> OfficeFan()
    {
        return _helpers.Builder.CreateSimple<float>()
            .WithName("Office Fan")
            .WithDescription("Turn on fan when it gets warm")
            .WithTriggers(Sensor.OfficeMotionDeviceTemperature)
            .WithExecution(async (sc, ct) => {
                if (sc.New.State > 90f && _officeMotion.State == OnOff.On)
                {
                    await _services.Api.TurnOn(Switch.OfficeFanSwitch);
                }
                else
                {
                    await _services.Api.TurnOff(Switch.OfficeFanSwitch);
                }
            })
            .WithAdditionalEntitiesToTrack(Switch.OfficeFanSwitch)
            .Build();
    }

    ISchedulableAutomation<OnOff, JsonElement> NoMotion()
    {
        return _helpers.Builder.CreateSchedulable<OnOff>()
            .WithName("Office off with no motion")
            .MakeDurable()
            .WithTriggers(Binary_Sensor.OfficeMotionMotion)
            .While(sc => sc.New.State == OnOff.Off)
            .For(TimeSpan.FromMinutes(10))
            .WithExecution(ct => _officeService.TurnOff(ct))
            .Build();
    }

    IAutomation DynamicallySetLights()
    {
        return _helpers.Builder.CreateSimple()
            .WithName("Set Office Lights")
            .WithDescription("dynamically set the office lights")
            .WithTimings(EventTiming.PostStartup | EventTiming.PreStartupPostLastCached)
            .WithTriggers(
                Binary_Sensor.OfficeMotionMotion, 
                Input_Boolean.OfficeOverride, 
                Sensor.OfficeMotionIlluminance
            )
            .WithExecution(async (sc, ct) => {
                if (sc.EntityId == Binary_Sensor.OfficeMotionMotion)
                {
                    var detected = sc.ToOnOff().New.State == OnOff.On;
                    if (detected)
                    {
                        await _officeService.SetLights(false);
                        return;
                    }
                }
                else
                {
                    await _officeService.SetLights(sc.EntityId == Sensor.OfficeMotionIlluminance, ct);
                }
            })
            .Build();
    }

    IAutomation DynamicallySetLightsFromTarget()
    {
        return _helpers.Builder.CreateSimple()
            .WithName("Set Office Lights from illuminance")
            .WithDescription("dynamically set the office lights")
            .WithTimings(EventTiming.PostStartup | EventTiming.PreStartupPostLastCached)
            .WithMode(AutomationMode.Smart)
            .WithTriggers(
                Input_Number.OfficeIlluminanceThreshold
            )
            .WithExecution(async (sc, ct) => {
                await _officeService.SetLights(true, ct);
            })
            .Build();
        }

    IAutomation<OnOff, JsonElement> ReportOverrideStatus()
    {
        return _helpers.Builder.CreateSimple<OnOff>()
            .WithName("report office overrid status")
            .WithDescription("when office override changes, report on Office LED")
            .WithTriggers(Input_Boolean.OfficeOverride)
            .WithExecution((sc, ct) => {
                return (sc.New.State switch{
                    OnOff.On => _services.Api.LightTurnOn(new LightTurnOnModel{
                        EntityId = [Light.OfficeLedLight],
                        Brightness = Bytes._10pct,
                        ColorName = "red"
                    }),
                    OnOff.Off => _services.Api.LightTurnOn(new LightTurnOnModel{
                        EntityId = [Light.OfficeLedLight],
                        Brightness = Bytes._10pct,
                        ColorName = "blue"
                    }),
                    _ => Task.CompletedTask
                }).ContinueWith(async t => 
                {
                    await Task.Delay(3000);
                    await _services.Api.TurnOff(Light.OfficeLedLight);
                });
            })
            .Build();
    }
}
