namespace SecuroAPI.Contracts.Events;

public class Event
{
    public DateTime Timestamp { get; protected set; }

    protected Event()
    {
        Timestamp = DateTime.UtcNow;
    }
}