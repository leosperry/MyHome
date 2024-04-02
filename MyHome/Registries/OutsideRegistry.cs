using System.Net;
using HaKafkaNet;

namespace MyHome;

public class OutsideRegistry : IAutomationRegistry
{
    readonly IHaServices _services;
    readonly IAutomationFactory _factory;
    readonly IAutomationBuilder _builder;
    readonly ILogger _logger;

    public OutsideRegistry(IHaServices services, IAutomationFactory factory, IAutomationBuilder builder, ILogger<OutsideRegistry> logger)
    {
        _services = services;
        _factory = factory;
        _builder = builder;
        _logger = logger;
    }    
    public void Register(IRegistrar reg)
    {
        reg.RegisterMultiple(
            _factory.DurableAutoOff("switch.back_flood", TimeSpan.FromMinutes(30)).WithMeta("auto off back flood","30 min"),
            _factory.DurableAutoOff("switch.back_porch_light", TimeSpan.FromMinutes(30)).WithMeta("auto off back porch","30 min"),
            _factory.DurableAutoOff("light.front_porch", TimeSpan.FromMinutes(10)).WithMeta("auto off front porch","10 min")
        );

        //door open alerts
        reg.RegisterMultiple(
            WhenDoorStaysOpen_Alert("binary_sensor.inside_garage_door_contact_opening", "Inside Garage Door"),
            WhenDoorStaysOpen_Alert("binary_sensor.front_door_contact_opening", "Front Door"),
            WhenDoorStaysOpen_Alert("binary_sensor.back_door_contact_opening", "Back Door")
        );
        
        reg.Register(_factory.DurableAutoOn(Helpers.PorchMotionEnable, TimeSpan.FromHours(1)).WithMeta("Auto enable front porch motion","1 hour"));
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

                try
                {
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
                }
                catch (TaskCanceledException)
                {
                    _logger.LogInformation("Task Canceled");
                }
            })
            .Build();
    }
}
