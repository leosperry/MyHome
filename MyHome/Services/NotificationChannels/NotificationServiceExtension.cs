using System.Diagnostics.CodeAnalysis;
using HaKafkaNet;

namespace MyHome;

public static class NotificationServiceExtension
{
    [AllowNull]
    static NotificationSender _critical;

    public static NotificationSender GetCritical(this INotificationService service)
        => MakeCritical(service);

    private static NotificationSender MakeCritical(INotificationService service)
    {
        var audible = service.CreateAudibleChannel([Media_Player.DiningRoomSpeaker, Media_Player.MainBedroomSpeaker]);
        var phones = service.CreateGroupOrDeviceChannel(Phones.LeonardPhone, Phones.RachelPhone);
        var monkey = service.CreateMonkeyChannel(new()
        {
            EntityId = [Light.MonkeyLight],
            RgbColor = (255, 0, 88),
            Flash = HaKafkaNet.Flash.Short,
            Brightness = Bytes.Max
        });    
        return service.CreateNotificationSender([audible, phones, service.Persistent],[monkey]);
    }
}
