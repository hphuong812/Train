using UnityEngine;

namespace IsoMatrix.Scripts.Data
{
    public class GameConstant
    {
        public const string LEVEL_LOAD = "levelLoad";
        public const string LEVEL_FAIL = "levelFail";
        public const string LEVEL_WIN = "levelWin";
        public const string LEVEL_RELOAD = "levelWin";
        public const string CURRENT_LEVEL_ID_KEY = "currentLevelID";
        public const string TUTORIAL_COMPLETE = "tutorialComplete";
        public const string LEVEL_DATA_KEY = "levelData";

        public const string SCENE_NAME_BOOT = "scene_boot";
        public const string SCENE_NAME_MAIN_MENU = "scene_main_menu";
        public const string SCENE_NAME_PLAY = "scene_play";
        public const string WALLET_KEY = "wallet";
        public const string COIN_KEY = "coins";

        public const string MATERIAL_KEY = "materialKey";

        public const string ADS_CONFIG_KEY = "adsConfig";

        public const string IAP_ITEM_REMOVE_ADS_KEY = "remove_ads";

        // player prefabs key
        public const string PLAYER_PREFS_FIRST_TIME_OPEN_KEY = "firstTimeOpen";

        public static readonly Vector2Int TOP_LEFT_POINT = new Vector2Int(2, 3);
        public static readonly Vector2Int BOTTOM_LEFT_POINT = new Vector2Int(6, 7);
        public static readonly Vector2Int TOP_RIGHT_POINT = new Vector2Int(0, 1);
        public static readonly Vector2Int BOTTOM_RIGHT_POINT = new Vector2Int(4, 5);
        public static readonly Vector2Int RIGHT_POINT = new Vector2Int(1, 2);
        public static readonly Vector2Int TOP_POINT = new Vector2Int(3, 4);

        public const int MAX_LEVEL = 3;
    }
}