using System.Text.Json;
using HaKafkaNet;

namespace MyHome;

/// <summary>
/// Uses the solar pannels on the roof as a light meter
/// When the sun is getting low or on exceptional cloudy day
///     gradually turn on the lights in the living room
///     using a parabolic curve so that it is not jarring
/// </summary>
public class LivingRoomLights : IAutomation, IAutomationMeta
{
    private readonly IHaEntityProvider _entityProvider;
    private readonly ILivingRoomService _livingRoom;
    public const string TRIGGER = "sensor.solaredge_current_power";

    public LivingRoomLights(IHaEntityProvider entityProvider, ILivingRoomService livingRoomService)
    {
        this._entityProvider = entityProvider;
        this._livingRoom = livingRoomService;
    }

    public async Task Execute(HaEntityStateChange stateChange, CancellationToken ct)
    {
        var powerState = stateChange.ToFloatTyped();

        await _livingRoom.SetLightsBasedOnPower(powerState.New.State, ct);
    }

    public IEnumerable<string> TriggerEntityIds()
    {
        yield return TRIGGER;
    }

    public AutomationMetaData GetMetaData()
    {
        return new AutomationMetaData()
        {
            Name = "Living Room Lights",
            Description = "set lights based on solar power generation",
            AdditionalEntitiesToTrack = [
                Lights.Couch1, Lights.Couch2, Lights.Couch3, 
                Lights.LivingLamp1, Lights.LivingLamp2
            ]
        };
    }
}
