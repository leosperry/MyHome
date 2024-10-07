using HaKafkaNet;
using MyHome.Areas.Office;

namespace MyHome;

public class OfficeRegistry : IAutomationRegistry
{
    readonly IHaServices _services;
    readonly IAutomationFactory _factory;
    readonly IAutomationBuilder _builder;
    private readonly OfficeService _officeService;

    public OfficeRegistry(IHaServices services, IAutomationFactory factory, IAutomationBuilder builder, 
        OfficeService officeService, INotificationService notificationService)
    {
        _services = services;
        _factory = factory;
        _builder = builder;
        _officeService = officeService;

        //var channel = notificationService.CreateAudibleChannel([MediaPlayers.Office], Voices.Mundane);
        //_notifyOffice = notificationService.CreateNotificationSender([channel]);
    }
    
    public void Register(IRegistrar reg)
    {
        
        reg.RegisterMultiple(
            DynamicallySetLights(),
            ReportOverrideStatus(),
            OfficeFan(),
            TurnOffAllOfficeWithSwitch(),
            DyanicallyAdjustWithSwitch()
        );

        reg.RegisterMultiple(NoMotion());
    }

    private IAutomation TurnOffAllOfficeWithSwitch()
    {
        return _builder.CreateSimple()
            .WithName("Turn Off all office with switch")
            .WithTriggers("event.office_lights_scene_001")
            .WithExecution(async (sc, ct) => {
                if(sc.ToSceneControllerEvent().New.StateAndLastUpdatedWithin1Second())
                {
                    await _officeService.TurnOff();
                }
            })
            .Build();
    }

    private IAutomation DyanicallyAdjustWithSwitch()
    {
        return _builder.CreateSimple()
            .WithName("Turn Off all office with switch")
            .WithTriggers("event.office_lights_scene_002")
            .WithExecution(async (sc, ct) => {
                if(sc.ToSceneControllerEvent().New.StateAndLastUpdatedWithin1Second())
                {
                    await _officeService.SetLights(false ,ct);
                }
            })
            .Build();    }

    IAutomation OfficeFan()
    {
        return _builder.CreateSimple()
            .WithName("Office Fan")
            .WithDescription("Turn on fan when it gets warm")
            .WithTriggers(Sensors.OfficeTemp)
            .WithExecution(this.OfficeFan)
            .WithAdditionalEntitiesToTrack(Devices.OfficeFan)
            .Build();
    }

    ISchedulableAutomation NoMotion()
    {
        return _builder.CreateSchedulable()
            .WithName("Office off with no motion")
            .MakeDurable()
            .WithTriggers(Sensors.OfficeMotion)
            .While(sc => sc.ToOnOff().New.State == OnOff.Off)
            .For(TimeSpan.FromMinutes(10))
            .WithExecution(ct => _officeService.TurnOff(ct))
            .Build();
    }

    IAutomation DynamicallySetLights()
    {
        return _builder.CreateSimple()
            .WithName("Set Office Lights")
            .WithDescription("dynamically set the office lights")
            .WithTimings(EventTiming.PostStartup | EventTiming.PreStartupPostLastCached)
            .WithTriggers(
                Sensors.OfficeIlluminance, 
                Sensors.OfficeMotion, 
                Helpers.OfficeOverride, 
                Helpers.OfficeIlluminanceThreshold
            )
            .WithExecution(async (sc, ct) => {
                if (sc.EntityId == Sensors.OfficeMotion)
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
                    await _officeService.SetLights(sc.EntityId == Sensors.OfficeIlluminance, ct);
                }
                
            })
            .Build();
    }

    IAutomation ReportOverrideStatus()
    {
        return _builder.CreateSimple()
            .WithName("report office overrid status")
            .WithDescription("when office override changes, report on Office LED")
            .WithTriggers(Helpers.OfficeOverride)
            .WithExecution((sc, ct) => {
                return sc.New.State switch{
                    "on" => _services.Api.LightTurnOn(new LightTurnOnModel{
                        EntityId = [Lights.OfficeLeds],
                        Brightness = Bytes._10pct,
                        ColorName = "red"
                    }),
                    "off" => _services.Api.LightTurnOn(new LightTurnOnModel{
                        EntityId = [Lights.OfficeLeds],
                        Brightness = Bytes._10pct,
                        ColorName = "blue"
                    }),
                    _ => Task.CompletedTask
                };
            })
            .Build();
    }

    private async Task OfficeFan(HaEntityStateChange change, CancellationToken token)
    {
        var tempState = change.ToFloatTyped();
        var officeMotion = await _services.EntityProvider.GetOnOffEntity(Sensors.OfficeMotion, token);
        if (tempState.New.State > 90f && officeMotion?.State == OnOff.On)
        {
            await _services.Api.TurnOn(Devices.OfficeFan);
        }
        else
        {
            await _services.Api.TurnOff(Devices.OfficeFan);
        }
    }
}
