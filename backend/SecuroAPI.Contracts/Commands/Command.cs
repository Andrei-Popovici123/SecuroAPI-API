using SecuroAPI.Contracts.Events;

namespace SecuroAPI.Contracts.Commands;

public abstract record Command : Message
{
    public DateTime Timestamp { get; protected set; } = DateTime.UtcNow;
}