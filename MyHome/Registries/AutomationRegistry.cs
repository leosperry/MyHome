using System.Net;
using HaKafkaNet;

namespace MyHome;

public class AutomationRegistry : IAutomationRegistry
{
    readonly IHaServices _services;
    readonly IAutomationFactory _factory;
    readonly IAutomationBuilder _builder;

    public AutomationRegistry(IHaServices services, IAutomationFactory factory, IAutomationBuilder builder)
    {
        _services = services;
        _factory = factory;
        _builder = builder;
    }

    public void Register(IRegistrar reg)
    {
        reg.Register(_factory.LightOffOnNoMotion(
            ["binary_sensor.lumi_lumi_sensor_motion_aq2_motion"],
            ["light.office_led_light", "light.office_lights"], TimeSpan.FromMinutes(10)).WithMeta("Office Light Off","10 minutes"));
        
        reg.RegisterMultiple(
            _factory.EntityAutoOff("switch.back_flood", 30).WithMeta("auto off back flood","30 min"),
            _factory.EntityAutoOff("switch.back_porch_light", 30).WithMeta("auto off back porch","30 min"),
            _factory.EntityAutoOff("switch.back_hall_light", 10).WithMeta("auto off back hall","10 min"),
            _factory.EntityAutoOff("light.front_porch", 10).WithMeta("auto off front porch","10 min"),
            _factory.EntityAutoOff("light.upstairs_hall", 30).WithMeta("auto off upstairs hall","30 min"),
            _factory.EntityAutoOff("light.entry_light", 30).WithMeta("auto off entry light","30 min")
        );

        reg.RegisterMultiple(
            WhenDoorStaysOpen_Alert("binary_sensor.inside_garage_door_contact_opening", "Inside Garage Door"),
            WhenDoorStaysOpen_Alert("binary_sensor.front_door_contact_opening", "Front Door"),
            WhenDoorStaysOpen_Alert("binary_sensor.back_door_contact_opening", "Back Door")
        );

        reg.Register(_builder.CreateSimple()
            .WithName("Lyra Brush Hair")
            .WithTriggers("binary_sensor.lyra_brush_hair")
            .WithExecution((sc, ct) => _services.Api.NotifyAlexaMedia("Time to brush Lyra's hair", ["Living Room", "Kitchen"], ct))
            .Build());
    }

    private IConditionalAutomation WhenDoorStaysOpen_Alert(string doorId, string doorName)
    {
        int seconds = 8;
        return _builder.CreateConditional()
            .WithName($"{doorName} Alert")
            .WithDescription($"Notify when {doorName} stays open for {seconds} seconds")
            .WithTriggers(doorId)
            .When((stateChange) => stateChange.New.GetStateEnum<OnOff>() == OnOff.On)
            .ForSeconds(seconds)
            .Then(async ct => 
            {
                string doorState;
                int count = 0;
                do
                {
                    await _services.Api.NotifyAlexaMedia($"{doorName} open", ["Living Room", "Kitchen"]);
                    await Task.Delay(TimeSpan.FromSeconds(seconds));
                    var (doorResponse, reportedState)= await _services.Api.GetEntity(doorId, ct);
                    
                    if (doorResponse.StatusCode != HttpStatusCode.OK || reportedState is null)
                    {
                        await _services.Api.PersistentNotification($"failure in checking door state:{doorName}", ct);
                        break;
                    }
                    doorState = reportedState.State;
                } while (doorState == "on" && ++count <= 10);
                
                if (count == 10)
                {
                    await _services.Api.NotifyGroupOrDevice("critical_notification_group", $"{doorName} has remained open for more than a minute", ct);
                }
            })
            .Build();
    }
}
