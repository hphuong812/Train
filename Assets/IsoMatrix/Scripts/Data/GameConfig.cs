using System.Collections.Generic;

namespace IsoMatrix.Scripts.Data
{
    public class GameConfig
    {
        private static GameConfig instance;
        public static GameConfig Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameConfig();
                }
                return instance;
            }
        }
        public List<LevelItemData> LevelItemList { get; set; } = new();
        public int CurrentLevel = 2;
    }
}
