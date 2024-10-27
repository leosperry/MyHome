
using System.Text.Json;
using HaKafkaNet;

namespace MyHome;

public interface IGarageService
{
    Task EnsureGarageClosed(NotificationSender notify, CancellationToken cancellationToken);
    Task OpenCloseGarage1(bool open);
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

    private IHaEntityProvider _entityProvider;
    private readonly IHaApiProvider _api;

    private readonly NotificationSender _notifyOffice;

    public GarageService(IHaEntityProvider cache, IHaApiProvider api, INotificationService notificationService)
    {
        _entityProvider = cache;
        _api = api;

        var diningRoomChannel = notificationService.CreateAudibleChannel([MediaPlayers.DiningRoom], Voices.Mundane);
        _notifyOffice = notificationService.CreateNotificationSender([diningRoomChannel]);
    }


    public Task EnsureGarageClosed(NotificationSender notify, CancellationToken cancellationToken)
    {
        return Task.WhenAll(
            EnsureDoorClosed(GARAGE1_CONTACT, GARAGE1_TILT, GARAGE1_DOOR_OPENER, "Garage Door 1", notify, cancellationToken),
            EnsureDoorClosed(GARAGE2_CONTACT, GARAGE2_TILT, GARAGE2_DOOR_OPENER, "Garage Door 2", notify,  cancellationToken)
        );            
    }

    private async Task EnsureDoorClosed(string contact, string tilt, string opener, string doorName, NotificationSender notify, CancellationToken cancellationToken)
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
                    notify($"Attempting to close {doorName}"),
                    Task.Delay(TimeSpan.FromSeconds(15)) // wait for door to close
                );

                //make sure it is closed
                var doorState = await getGarageDoorState(contact, tilt);
                if (doorState != GarageDoorState.Closed)
                {
                    await notify($"Could not verify {doorName} is closed");
                }

                break;
            case GarageDoorState.Unknown:
                // alert
                await Task.WhenAll(
                    notify($"{doorName} is in an unknown state"),
                    _api.TurnOn(BACK_HALL_LIGHT, cancellationToken));
                break;
        }
    }

    private async Task<GarageDoorState> getGarageDoorState(string garageContact, string garageTilt)
    {
        Task<IHaEntity<OnOff,JsonElement>?> contactTask;
        Task<IHaEntity<OnOff,JsonElement>?> tiltTask;
        
        contactTask = _entityProvider.GetOnOffEntity(garageContact);
        tiltTask = _entityProvider.GetOnOffEntity(garageTilt);

        await Task.WhenAll(contactTask, tiltTask);
        
        if (contactTask.Result.Bad() || tiltTask.Result.Bad())
        {
            return GarageDoorState.Unknown;
        }

        return (contactTask.Result , tiltTask.Result) switch
        {
            {Item1.State: OnOff.On, Item2.State: OnOff.On} => GarageDoorState.Open,
            {Item1.State: OnOff.Off, Item2.State: OnOff.Off} => GarageDoorState.Closed,
            _ => GarageDoorState.Unknown
        };
    }

    public async Task OpenCloseGarage1(bool open)
    {
        var doorState = await getGarageDoorState(GARAGE1_CONTACT, GARAGE1_TILT);

        switch((open, doorState))
        {
            case { doorState: GarageDoorState.Unknown}:
                await _notifyOffice("Door in unknown state");
                break;
            case {open: true, doorState : GarageDoorState.Open }:
                await _notifyOffice("The door is already open");
                break;
            case {open: false, doorState : GarageDoorState.Closed }:

                await _notifyOffice("The door is already closed");
                break;
            case {open: true, doorState : GarageDoorState.Closed }:
            case {open: false, doorState : GarageDoorState.Open }:
                await _api.TurnOn(GARAGE1_DOOR_OPENER);
                break;
            default:
                break;
        };
    }
    enum GarageDoorState
    {
        Open, Closed, Unknown
    }
}
