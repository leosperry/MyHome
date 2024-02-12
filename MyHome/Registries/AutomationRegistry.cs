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
            ["light.office_led_light", "light.office_lights"], TimeSpan.FromMinutes(10)));
        
        reg.Register(_factory.EntityAutoOff("switch.back_flood", 30).WithMeta("auto off back flood"));
        reg.Register(_factory.EntityAutoOff("switch.back_porch_light", 30).WithMeta("auto off back porch"));
        reg.Register(_factory.EntityAutoOff("switch.back_hall_light", 10).WithMeta("auto off back hall"));
        reg.Register(_factory.EntityAutoOff("light.front_porch", 10).WithMeta("auto off front porch"));
        reg.Register(_factory.EntityAutoOff("light.upstairs_hall", 30).WithMeta("auto off upstairs hall"));

        reg.Register(WhenDoorStaysOpen_Alert("binary_sensor.inside_garage_door_contact_opening", "Inside Garage Door"));
        reg.Register(WhenDoorStaysOpen_Alert("binary_sensor.front_door_contact_opening", "Front Door"));
        reg.Register(WhenDoorStaysOpen_Alert("binary_sensor.back_door_contact_opening", "Back Door"));



        reg.Register(_builder.CreateSimple()
            .WithName("Lyra Brush Hair")
            .WithTriggers("input_datetime.lyra_brush_hair", "input_datetime.lyra_brush_hair_2")
            .WithExecution((sc, ct) => _services.Api.NotifyAlexaMedia("Time to brush Lyra's hair", ["Living Room", "Kitchen"], ct))
            .Build());
    }

    private IConditionalAutomation WhenDoorStaysOpen_Alert(string doorId, string doorName)
    {
        const int seconds = 7;
        return _builder.CreateConditional()
            .WithName($"{doorName} Alert")
            .WithDescription($"Notify when {doorName} stays open for 7 seconds")
            .WithTriggers(doorId)
            .When((stateChange) => stateChange.New.State == "on")
            .ForSeconds(seconds)
            .Then(async ct => 
            {
                string doorState;
                do
                {
                    await _services.Api.NotifyAlexaMedia($"{doorName} open", ["Living Room", "Kitchen"]);
                    await Task.Delay(TimeSpan.FromSeconds(seconds));
                    var (doorResponse, reportedState)= await _services.Api.GetEntityState(doorId);
                    
                    if (doorResponse.StatusCode != HttpStatusCode.OK || reportedState is null)
                    {
                        await _services.Api.PersistentNotification($"failure in checking door state:{doorName}");
                        break;
                    }
                    doorState = reportedState.State;
                } while (doorState == "on");
            })
            .Build();
    }
}
