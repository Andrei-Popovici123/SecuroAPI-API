using SecuroAPI.BusinessLogic.DTO_s.Rating;
using SecuroAPI.Common.Results;
using SecuroAPI.Contracts.Events;

namespace SecuroAPI.BusinessLogic.Services.Publisher;

public interface IMonitoringRegisterPublisher
{

    Task PublishAsync(RegisterMonitoringMessage message, CancellationToken cancellationToken = default);
}