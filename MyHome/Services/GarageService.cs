
using Confluent.Kafka;
using HaKafkaNet;

namespace MyHome;

public interface IGarageService
{
    Task EnsureGarageClosed(CancellationToken cancellationToken);
}

public class GarageService : IGarageService
{
    public const string GARAGE1_CONTACT = "binary_sensor.garage_1_contact_opening";
    public const string GARAGE2_CONTACT = "binary_sensor.garage_2_contact_opening";
    public const string GARAGE1_TILT = "binary_sensor.garage_door_1_tilt_sensor_state_any";
    public const string GARAGE2_TILT = "binary_sensor.garage_door_2_tilt_sensor_state_any";
    public const string GARAGE1_DOOR_OPENER = "switch.garage_door_opener";
    public const string GARAGE2_DOOR_OPENER = "switch.garage_door_opener_2";
    public const string BACK_HALL_LIGHT = "switch.back_hall_light";

    private IHaStateCache _cache;
    private readonly IHaApiProvider _api;

    public GarageService(IHaStateCache cache, IHaApiProvider api)
    {
        _cache = cache;
        _api = api;
    }

    public Task EnsureGarageClosed(CancellationToken cancellationToken)
    {
        return Task.WhenAll(
            EnsureDoorClosed(GARAGE1_CONTACT, GARAGE1_TILT, GARAGE1_DOOR_OPENER, "Garage Door 1", cancellationToken),
            EnsureDoorClosed(GARAGE2_CONTACT, GARAGE2_TILT, GARAGE2_DOOR_OPENER, "Garage Door 2", cancellationToken)
        );            
    }

    private async Task EnsureDoorClosed(string contact, string tilt, string opener, string doorName, CancellationToken cancellationToken)
    {
        switch (await getGarageDoorState(contact, tilt))
        {
            case GarageDoorState.Closed:
                // do nothing
                break;
            case GarageDoorState.Open:
                //notify and close the door
                await Task.WhenAll(
                    _api.TurnOn(opener, cancellationToken), // close the door
                    _api.TurnOn(BACK_HALL_LIGHT, cancellationToken), // turn onn the back hall light
                    _api.NotifyGroupOrDevice("my_notify_group", $"Attempting to close {doorName}", cancellationToken),
                    Task.Delay(TimeSpan.FromSeconds(15)) // wait for door to close
                );

                //make sure it is closed
                var doorState = await getGarageDoorState(contact, tilt);
                if (doorState != GarageDoorState.Closed)
                {
                    await _api.NotifyGroupOrDevice("my_notify_group", $"Could not verify {doorName} is closed", cancellationToken);
                }

                break;
            case GarageDoorState.Unknown:
                // alert
                await Task.WhenAll(
                    _api.NotifyGroupOrDevice("my_notify_group", $"{doorName} is in an unknown state", cancellationToken),
                    _api.TurnOn(BACK_HALL_LIGHT, cancellationToken));
                break;
        }
    }

    private async Task<GarageDoorState> getGarageDoorState(string garageContact, string garageTilt)
    {
        var contact = await _cache.Get(garageContact);
        var tilt = await _cache.Get(garageTilt);

        return (contact, tilt) switch
        {
            {contact.State: "on", tilt.State: "on"} => GarageDoorState.Open,
            {contact.State: "off", tilt.State: "off"} => GarageDoorState.Closed,
            _ => GarageDoorState.Unknown
        };

    }

    enum GarageDoorState
    {
        Open, Closed, Unknown
    }


}
