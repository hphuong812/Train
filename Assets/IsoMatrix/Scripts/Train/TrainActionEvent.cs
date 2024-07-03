using ADN.Meta.Core;

namespace IsoMatrix.Scripts.Train
{
    public enum TrainActionEventType
    {
        Run,
        Reset,
        LocomotiveRun,
        Update,
    }
    public struct TrainActionEvent : IEvent
    {
        public TrainActionEventType type;

        public TrainActionEvent(TrainActionEventType type)
        {
            this.type = type;
        }

        public static void Trigger(TrainActionEventType type)
        {
            var eventInstance = new TrainActionEvent(type);
            EventManager.TriggerEvent(eventInstance);
        }
    }
}