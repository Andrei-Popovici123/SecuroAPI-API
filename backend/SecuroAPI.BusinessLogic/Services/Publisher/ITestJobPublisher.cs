

using SecuroAPI.Contracts.Events;

namespace SecuroAPI.BusinessLogic.Services.Publisher;

public interface ITestJobPublisher
{

    Task PublishAsync(TestJobMessage message, CancellationToken cancellationToken = default);
}


