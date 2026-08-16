using SecuroAPI.Contracts.Events;

namespace SecuroAPI.BusinessLogic.Services.Publisher;

public interface IEventHandler<in TEvent> : IEventHandler where TEvent :  Event
{
    Task Handle(TEvent @event);
}

public interface IEventHandler
{
    
}