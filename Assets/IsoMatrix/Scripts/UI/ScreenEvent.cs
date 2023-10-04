using ADN.Meta.Core;

namespace IsoMatrix.Scripts.UI
{
    public enum ScreenEventType
    {
        ScreenIn,
        ScreenOut
    }
    public struct ScreenEvent : IEvent
    {
        public ScreenEventType type;

        public ScreenEvent(ScreenEventType type)
        {
            this.type = type;
        }

        public static void Trigger(ScreenEventType type)
        {
            var eventInstance = new ScreenEvent(type);
            EventManager.TriggerEvent(eventInstance);
        }
    }
}