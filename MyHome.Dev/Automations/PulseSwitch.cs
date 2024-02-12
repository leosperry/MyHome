using HaKafkaNet;

namespace MyHome.Dev;

public class PulseSwitch : IConditionalAutomation
{
    private readonly IHaServices _services;
    const string LIGHT = "light.office_led_light";
    const string BUTTON = "event.lounge_buttons_scene_008";
    readonly TimeSpan CAPTURE_TIME = TimeSpan.FromSeconds(2);
    private DateTime _lastPress = default;

    public PulseSwitch(IHaServices haServices)
    {
        _services = haServices;
    }

    public IEnumerable<string> TriggerEntityIds()
    {
        yield return BUTTON;
    }
    public TimeSpan For => TimeSpan.FromMinutes(3);

    public async Task<bool> ContinuesToBeTrue(HaEntityStateChange haEntityStateChange, CancellationToken cancellationToken)
    {
        try
        {
            var lightState = await _services.EntityProvider.GetEntityState(LIGHT, cancellationToken);
            if (lightState!.State == "off")
            {
                await _services.Api.TurnOn(LIGHT);
                return true;
            }
            else
            {
                if (haEntityStateChange.New.LastUpdated - _lastPress > CAPTURE_TIME)
                {
                    // light is on, but after the capture time
                    // is the expected behavior to turn it off?
                    await _services.Api.TurnOn(LIGHT, cancellationToken);
                }
                return false; //cancels the delayed task to turn it off
                //either we turned it off already or we don't want to turn it off in 3 minutes
            }        
        }
        finally
        {
            _lastPress = haEntityStateChange.New.LastUpdated;
        }
    }

    public Task Execute(CancellationToken cancellationToken)
    {
        // turns off the light in 3 minutes
        return _services.Api.TurnOff(LIGHT, cancellationToken); 
    }


}
