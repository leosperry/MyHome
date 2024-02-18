using HaKafkaNet;

namespace MyHome.Dev;

public class TestAutomation : IAutomation, IAutomationMeta
{
    private IHaServices _services;

    public TestAutomation(IHaServices services)
    {
        _services = services;
    }

    public async Task Execute(HaEntityStateChange stateChange, CancellationToken cancellationToken)
    {
        var onoffChange = stateChange.ToOnOff();
        if ((onoffChange.Old is null || onoffChange.Old.State != OnOff.On) && onoffChange.New.State == OnOff.On)
        {
            
        }


        var sensor =    await _services.EntityProvider.GetEntity("sensor.lumi_lumi_sensor_motion_aq2_illuminance_3");
        var intSensor = await _services.EntityProvider.GetIntegerEntity("sensor.lumi_lumi_sensor_motion_aq2_illuminance_3");
        
        var illuminance = sensor?.GetState<int>();
        illuminance = intSensor?.State;


        System.Console.WriteLine(illuminance);
        //var log = await _services.Api.GetErrorLog(cancellationToken);
        //Console.WriteLine(await log.Content.ReadAsStringAsync());
        //return Task.CompletedTask;
    }

    public AutomationMetaData GetMetaData()
    {
        return new()
        {
            Name = "Test Automamtion",
            Description = "Used for testing quick scenarios",
            Enabled = true
        };
    }

    public IEnumerable<string> TriggerEntityIds()
    {
        //yield return "sun.sun";
        yield return "input_button.test_button";
    }
}
