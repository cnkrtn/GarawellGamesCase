using System.Collections.Generic;

namespace Core.AudioService.Keys
{
    public class AudioData
    {
        public bool RandomPitch { get; }
        public bool Countable { get; }
        public float VolumeOverride { get; }

        public AudioData(bool randomPitch,bool countable)
        {
            RandomPitch = randomPitch;
            Countable = countable;
            VolumeOverride = 0;
        }
        public AudioData(bool randomPitch,bool countable,float volumeOverride)
        {
            RandomPitch = randomPitch;
            Countable = countable;
            VolumeOverride = volumeOverride;
        }
    }

    public static class AudioKeys
    {
        public static readonly string KEY_TILE_SNAP = "TileSnap";
        public static readonly string KEY_TILE_DRAG = "TileDrag";
        public static readonly string KEY_SQUARE_CREATED = "SquareCreated";
        public static readonly string KEY_LINE_CLEARED = "LineClear";
        public static readonly string KEY_CoMBO = "Combo";
        public static readonly string KEY_EXCELLENT = "Excellent";
        public static readonly string KEY_WIN = "Win";
      
        public static readonly string KEY_MAIN_MUSIC = "MainMusic";
        public static readonly string KEY_CLICK_SOUND = "Click";
        
      

        public static readonly Dictionary<string, AudioData> KEY_ALL_AUDIO = new ()
        {
            { KEY_SQUARE_CREATED , new AudioData( true, true)},
            { KEY_LINE_CLEARED , new AudioData( true, true)},
            { KEY_CLICK_SOUND , new AudioData( true, true,0.8f)},
            { KEY_CoMBO , new AudioData( true, false,0.8f)},
            { KEY_EXCELLENT , new AudioData( true, false,0.8f)},
            { KEY_WIN , new AudioData( true, false,0.8f)},
            { KEY_TILE_DRAG , new AudioData( true, false,0.8f)},
            { KEY_TILE_SNAP , new AudioData( true, false,0.8f)},
         

        };
    }
}