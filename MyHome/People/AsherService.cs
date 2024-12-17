using System;
using System.Text.Json;
using HaKafkaNet;
using Microsoft.Extensions.Logging;
using MyHome.Models;

namespace MyHome.People;

public class AsherService
{
    private readonly IHaServices _services;
    private readonly ILogger<AsherService> _logger;

    private readonly IHaEntity<MediaPlayerState, SonosAttributes> _asherMediaPlayer;
    private readonly IUpdatingEntity<OnOff, JsonElement> _override;
    private readonly NotificationSender _diningRoomChannel;
    private readonly IUpdatingEntity<OnOff, LightModel> _basementGroup;

    public AsherService(INotificationService notificationService, IHaServices services, IUpdatingEntityProvider updatingEntityProvider, ILogger<AsherService> logger)
    {
        this._services = services;
        this._logger = logger;

        _asherMediaPlayer = updatingEntityProvider.GetMediaPlayer<SonosAttributes>(Media_Player.AsherRoomSpeaker);
        _override = updatingEntityProvider.GetOnOffEntity(Input_Boolean.BasementOverride);

        this._diningRoomChannel = notificationService.CreateNotificationSender(
            [notificationService.CreateAudibleChannel([Media_Player.DiningRoomSpeaker])]);

        _basementGroup = updatingEntityProvider.GetLightEntity(Light.BasementLightGroup);
    }

    static readonly string[] _messages = [ 
        "My lord. Your presence is requested in the main chamber",
        "Prince Asher, the king and queen have summoned you",
        "Hey you. Yes, you. Please come upstairs",
        "The parental units have requested of the carbon based lifeform known as Asher to vacate his domicile and return upstairs",
        "The crown has requested a status report from the dungeons forthwith",
        "Your assignment is as follows, collect dishes and return to base. This message will self destruct",
        "Brave knight. You have been given a valiant quest to return to the overworld",
        "Random message to see if you're paying attention" 
    ];
    
    static readonly PiperSettings[] _voices = [Voices.Buttler, Voices.Female, Voices.Mundane];
    static readonly Random _random = new();

    public async Task Toggle3Times(CancellationToken ct)
    {
        /* 
        Step 1
            Set 1 light on and 1 off
        Step 2
            repeat 4 times
            toggle lights
        Step 3
            set lights to 40%*/
        await Task.WhenAll(
            _services.Api.TurnOn(Light.BasementLight1, ct),
            _services.Api.TurnOff(Light.BasementLight2, ct)
        );

        int count = 0;
        while(++count <= 4)
        {
            await Task.WhenAll(
                Task.Delay(3000, ct),
                _services.Api.Toggle([Light.BasementLight1, Light.BasementLight2], ct)
            );
        }
        await _services.Api.LightSetBrightness([Light.BasementLight1, Light.BasementLight2], Bytes._40pct, ct);
    }

    public async Task PlayRandom(float targetVolume, CancellationToken ct)
    {
        // get a message
        var message = _messages[_random.Next(0, _messages.Length)];
        var voice = _voices[_random.Next(0, _voices.Length)];

        // get the volume

        if (!_asherMediaPlayer.Bad())
        {
            float? previousVolume = _asherMediaPlayer.Attributes?.VolumeLevel;
            if (previousVolume < targetVolume)
            {
                await _services.Api.MediaPlayerSetVolume(Media_Player.AsherRoomSpeaker, targetVolume);
                await Task.Delay(TimeSpan.FromSeconds(2));
            }

            await _services.Api.SpeakPiper(Media_Player.AsherRoomSpeaker, message, true, voice);
            await Task.Delay(TimeSpan.FromSeconds(10));

            if (previousVolume is not null && previousVolume != targetVolume)
            {
                await _services.Api.MediaPlayerSetVolume(Media_Player.AsherRoomSpeaker, previousVolume.Value);
            }
        }
        else
        {
            _logger.LogWarning("Asher speaker state is {AsherState}", _asherMediaPlayer?.State.ToString() ?? "null" );
            await _diningRoomChannel("Something is wrong with Asher's speaker");
        }
    }

    public async Task IncreaseLights(CancellationToken ct)
    {
        switch (_basementGroup.Snapshot().State)
        {
            case OnOff.Off:
                await _services.Api.LightSetBrightness(Light.BasementLightGroup, Bytes._20pct, ct);
                break;
            case OnOff.On:
                if (_basementGroup.Attributes?.Brightness < Bytes._20pct)
                {
                    await _services.Api.LightSetBrightness(Light.BasementLightGroup, Bytes._20pct, ct);
                }
                else
                {
                    await _services.Api.LightTurnOn(new LightTurnOnModel(){EntityId = [Light.BasementLightGroup], BrightnessStepPct = 10});
                }
                break;
            default:
                return;
        }
    }

    public async Task DecreaseLights(CancellationToken ct)
    {
        switch (_basementGroup.Snapshot().State)
        {
            case OnOff.Off:
                await Task.CompletedTask;
                break;
            case OnOff.On:
                if (_basementGroup.Attributes?.Brightness <= Bytes._20pct)
                {
                    await TurnOff(ct);
                }
                else
                {
                    await _services.Api.LightTurnOn(new LightTurnOnModel(){EntityId = [Light.BasementLightGroup], BrightnessStepPct = -10});
                }
                break;
            default:
                return;
        }    
    }

    public Task TurnOff(CancellationToken ct)
    {
        return Task.WhenAll(
            _services.Api.TurnOff([Light.BasementLight2, Light.BasementLight3], ct),
            _services.Api.LightSetBrightness(Light.BasementLight1, Bytes._20pct)
        );    
    }
}
