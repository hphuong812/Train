using ADN.Meta.Core;

namespace IsoMatrix.Scripts.Level
{
    public enum LevelEventType
    {
        Unknown,
        Loaded,
        Completed,
        NextLevel,
        Failed,
        ReloadCurLevel,
    }
    public struct LevelEvent : IEvent
    {
        public int currentLevel { set; get; }
        public LevelEventType type;

        public LevelEvent(LevelEventType type, int currentLevel)
        {
            this.currentLevel = currentLevel;
            this.type = type;
        }

        public static void Trigger(LevelEventType type, int currentLevel)
        {
            var eventInstance = new LevelEvent(type, currentLevel);
            EventManager.TriggerEvent(eventInstance);
        }
    }
}

