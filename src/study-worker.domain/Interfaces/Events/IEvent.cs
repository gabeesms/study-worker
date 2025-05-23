namespace study_worker.domain.Interfaces.Events
{
    public interface IEvent
    {
        string EventType { get; }
        DateTime Timestamp { get; }
    }
}
