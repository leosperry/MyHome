using System;
using HaKafkaNet;
using Microsoft.Extensions.Logging;
using MyHome.Models;

namespace MyHome.People;

public class AsherService
{
    private readonly IHaServices _services;
    private readonly ILogger<AsherService> _logger;

    private readonly IHaEntity<MediaPlayerState, SonosAttributes> _asherMediaPlayer;
    private readonly NotificationSender _diningRoomChannel;

    public AsherService(INotificationService notificationService, IHaServices services, IUpdatingEntityProvider updatingEntityProvider, ILogger<AsherService> logger)
    {
        this._services = services;
        this._logger = logger;

        _asherMediaPlayer = updatingEntityProvider.GetMediaPlayer<SonosAttributes>(MediaPlayers.Asher);

        this._diningRoomChannel = notificationService.CreateNotificationSender(
            [notificationService.CreateAudibleChannel([MediaPlayers.DiningRoom])]);
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
                await _services.Api.MediaPlayerSetVolume(MediaPlayers.Asher, targetVolume);
                await Task.Delay(TimeSpan.FromSeconds(2));
            }

            await _services.Api.SpeakPiper(MediaPlayers.Asher, message, true, voice);
            await Task.Delay(TimeSpan.FromSeconds(10));

            if (previousVolume is not null && previousVolume != targetVolume)
            {
                await _services.Api.MediaPlayerSetVolume(MediaPlayers.Asher, previousVolume.Value);
            }
        }
        else
        {
            _logger.LogWarning("Asher speaker state is {AsherState}", _asherMediaPlayer?.State.ToString() ?? "null" );
            await _diningRoomChannel("Something is wrong with Asher's speaker");
        }
    }
}
