using HaKafkaNet;
using MyHome.Models;

namespace MyHome;

public static class ApiExtensions
{
    public static async Task SetZoozSceneControllerButtonColor(this IHaApiProvider api, 
        string entity_id, int buttonNumber, ZoozColor color, CancellationToken ct = default)
    {
        if (buttonNumber < 1 || buttonNumber > 4)
        {
            throw new ArgumentOutOfRangeException(nameof(buttonNumber));
        }
        int parameter = 6 + buttonNumber;
        await api.ZwaveJs_SetConfigParameter(new {
            entity_id,
            endpoint = 0,
            parameter,
            value = (int)color
        }, ct);
    }

    public static async Task SetZoozSceneControllerButtonColorFromOverrideState(this IHaApiProvider api,
        string zoozControllerId,
        OnOff state, int buttonNumber, CancellationToken ct = default)
    {
        ZoozColor color = state switch
        {
            OnOff.On => ZoozColor.Yellow,
            OnOff.Off => ZoozColor.Cyan,
            _ => ZoozColor.Red
        };

        await api.SetZoozSceneControllerButtonColor(zoozControllerId, buttonNumber, color, ct);
    }
}
