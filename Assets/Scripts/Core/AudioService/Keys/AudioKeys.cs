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
        public static readonly string KEY_POWERUP5 = "PowerUp5";
        public static readonly string KEY_POWERUP4 = "PowerUp4";
        public static readonly string KEY_AUDIO_WATER_DEFLECT = "WaterDeflect";
        public static readonly string KEY_AUDIO_NATURE_DEFLECT = "NatureDeflect";
        public static readonly string KEY_AUDIO_DARK_DEFLECT = "DarkDeflect";
        public static readonly string KEY_AUDIO_FIRE_DEFLECT = "FireDeflect";
        public static readonly string KEY_AUDIO_HOLY_DEFLECT = "HolyDeflect";
        public static readonly string KEY_AUDIO_ELEMENT_TURN = "ElementTurn2";
        public static readonly string KEY_AUDIO_ENEMY_DIE = "EnemyDie";
        public static readonly string KEY_AUDIO_ELEMENT_MOVE = "ElementMove3";
        public static readonly string KEY_AUDIO_ELEMENT_MOVE_CANCEL = "ElementMoveCancel";
        public static readonly string KEY_MAIN_MUSIC = "MainMusic";
        public static readonly string KEY_BATTLE_MUSIC = "BattleMusic";

        
        public static readonly string KEY_WIN_GAME = "WinGameMusic";
        public static readonly string KEY_LOOSE_GAME = "LooseGameMusic";
        public static readonly string KEY_CLICK_SOUND = "ClickSound";
        
        public static readonly string KEY_CONFIRM_SOUND = "Confirm";
        public static readonly string KEY_MENU_OPEN_SOUND = "MenuOpen";
        public static readonly string KEY_MENU_CLOSE_SOUND = "MenuClose";
        public static readonly string KEY_PURCHASE_SOUND = "Purchase";
        public static readonly string KEY_SELECT_SOUND = "Select";

        public static readonly Dictionary<string, AudioData> KEY_ALL_AUDIO = new ()
        {
            { KEY_AUDIO_WATER_DEFLECT , new AudioData( true, true)},
            { KEY_AUDIO_NATURE_DEFLECT , new AudioData( true, true)},
            { KEY_AUDIO_DARK_DEFLECT , new AudioData(true, true)},
            { KEY_AUDIO_FIRE_DEFLECT , new AudioData(true, true)},
            { KEY_AUDIO_HOLY_DEFLECT , new AudioData(true, true)},
            { KEY_AUDIO_ELEMENT_TURN , new AudioData( true, false)},
            { KEY_AUDIO_ENEMY_DIE , new AudioData( true, false)},
            { KEY_AUDIO_ELEMENT_MOVE , new AudioData( true, false,0.3f)},
            { KEY_AUDIO_ELEMENT_MOVE_CANCEL , new AudioData( true, false,0.75f)},
            { KEY_WIN_GAME , new AudioData( true, false,0.75f)},
            { KEY_BATTLE_MUSIC , new AudioData( true, false,0.3f)},
            { KEY_CLICK_SOUND , new AudioData( true, false,0.8f)},
            { KEY_LOOSE_GAME , new AudioData( true, false,0.75f)},
            { KEY_POWERUP4 , new AudioData( true, false,0.8f)},
            { KEY_POWERUP5 , new AudioData( true, false,0.8f)},
            { KEY_CONFIRM_SOUND , new AudioData( true, false)},
            { KEY_MENU_OPEN_SOUND , new AudioData( true, false)},
            { KEY_MENU_CLOSE_SOUND , new AudioData( true, false)},
            { KEY_PURCHASE_SOUND , new AudioData( true, false)},
            { KEY_SELECT_SOUND , new AudioData( true, false)},

        };
    }
}