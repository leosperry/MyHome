using HaKafkaNet;

namespace MyHome;

public class Voices
{
    public static readonly PiperSettings Buttler = new(){
        Voice = "en_GB-semaine-medium",
        Speaker = 1 //spike
    };

    public static readonly PiperSettings Mundane = new(){
        Voice = "en_US-libritts_r-medium",
        Speaker = 7
    };

    public static readonly PiperSettings Female = new(){
        Voice = "en_US-libritts_r-medium",
        Speaker = 10
    };

    
}
