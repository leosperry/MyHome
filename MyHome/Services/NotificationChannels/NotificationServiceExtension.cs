using System.Diagnostics.CodeAnalysis;
using HaKafkaNet;

namespace MyHome;

public static class NotificationServiceExtension
{
    [AllowNull]
    static NotificationSender _critical;

    public static NotificationSender GetCritical(this INotificationService service)
        => _critical ??= MakeCritical(service);

    private static NotificationSender MakeCritical(INotificationService service)
    {
        var audible = service.CreateAudibleChannel([MediaPlayers.Kitchen, MediaPlayers.LivingRoom, MediaPlayers.Office, MediaPlayers.MainBedroom]);
        var phones = service.CreateGroupOrDeviceChannel(Phones.LeonardPhone, Phones.RachelPhone);
        var monkey = service.CreateMonkeyChannel(new()
        {
            EntityId = [Lights.Monkey],
            RgbColor = (255, 0, 88),
            Flash = HaKafkaNet.Flash.Short,
            Brightness = Bytes.Max
        });    
        return service.CreateNotificationSender([audible, phones, service.Persistent],[monkey]);
    }
}
