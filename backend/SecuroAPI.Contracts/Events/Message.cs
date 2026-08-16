using MediatR;

namespace SecuroAPI.Contracts.Events;

public record Message : IRequest<bool>
{
    public string MessageType {get; protected set;}

    protected Message()
    {
        MessageType = GetType().Name;
    }
}