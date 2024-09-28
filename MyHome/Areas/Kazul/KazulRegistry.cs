using System.Collections;
using HaKafkaNet;
using MyHome.Services;

namespace MyHome;

public class KazulRegistry : IAutomationRegistry
{
    private readonly HelpersService _helpersService;
    readonly IHaServices _services;
    readonly IAutomationFactory _factory;
    readonly IAutomationBuilder _builder;

    string[] KAZUL_UVB = ["switch.kazul_power_strip_3", "switch.kazul_power_strip_4"];

    NotificationSender _notifyCritical;

    public KazulRegistry(HelpersService helpersService, IHaServices services, IAutomationFactory factory, IAutomationBuilder builder, INotificationService notificationService)
    {
        _helpersService = helpersService;
        _services = services;
        _factory = factory;
        _builder = builder;
        _notifyCritical = notificationService.GetCritical();
    }

    public void Register(IRegistrar reg)
    {
        var kazulSunset = _factory.SunSetAutomation(async ct => {
            var turnOnVerification = await _services.Api.TurnOnAndVerify(KazulAlerts.CERAMIC_SWITCH, ct);
            
            if(!turnOnVerification)
            {
                await _notifyCritical("Failed to turn on Kazul Ceramic");
                return;
            }
            
            var turnOffVerification  = await _services.Api.TurnOffAndVerify(KazulAlerts.HALOGEN_SWITCH, ct);
            if(!turnOffVerification)
            {
                await _notifyCritical("Failed to turn off Kazul Halogen");
            }
            
        }).WithMeta(new AutomationMetaData(){
            Name = "Kazul sunset",
            Description = "Turn on Ceramic. Turn off Halogen",
            AdditionalEntitiesToTrack = [KazulAlerts.CERAMIC_SWITCH, KazulAlerts.HALOGEN_SWITCH],
        });

        var kazulSunrise = _factory.SunRiseAutomation(async ct => {
            bool turnOnVerification = await _services.Api.TurnOnAndVerify(KazulAlerts.HALOGEN_SWITCH, ct);

            if(!turnOnVerification)
            {
                await _notifyCritical("Failed to turn on Kazul Halogen");
                return;
            }            
            bool turnOffVerification = await _services.Api.TurnOffAndVerify(KazulAlerts.CERAMIC_SWITCH, ct);
            if(!turnOnVerification)
            {
                await _notifyCritical("Failed to turn off Kazul Ceramic");
                return;
            }
        }).WithMeta("Kazul sunrise", "Turn off Ceramic. Turn on Halogen");

        var kazulUVB =_factory.EntityOnOffWithAnother("binary_sensor.kazul_light_time_sensor", KAZUL_UVB)
            .WithMeta("Kazul UVB", "On at 8am. Off at 8pm");

        var ensureOneIsOn = _builder.CreateSimple()
            .WithName("Kazul - ensure 1 is on")
            .WithDescription("when either the ceramic or halogen change state, ensure at least 1 is on")
            .WithTriggers(KazulAlerts.CERAMIC_SWITCH, KazulAlerts.HALOGEN_SWITCH)
            .TriggerOnBadState()
            .WithExecution(async (sc, ct) =>{
                if(await _helpersService.MaintenanceModeIsOn()) return;

                if(sc.New.Bad())
                {
                    await _notifyCritical("Kazul power strip in bad state");
                }
                try
                {
                    var swithcStates = await Task.WhenAll(
                        _services.EntityProvider.GetOnOffEntity(KazulAlerts.CERAMIC_SWITCH),
                        _services.EntityProvider.GetOnOffEntity(KazulAlerts.HALOGEN_SWITCH)
                    );
                    bool noneOn = 
                        swithcStates[0]?.State != OnOff.On && 
                        swithcStates[1]?.State != OnOff.On;
                    if (noneOn)
                    {
                        await _notifyCritical("Kazul - Neither the ceramic nor halogen are on");
                    }
                }
                catch (System.Exception)
                {
                    await _notifyCritical("Could not verify Kazul power swites");
                    throw;
                }
            })
            .Build();
        
        reg.RegisterMultiple(kazulSunrise, kazulSunset);
        reg.RegisterMultiple(kazulUVB, ensureOneIsOn);
    }
}
