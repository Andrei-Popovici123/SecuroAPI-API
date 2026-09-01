using SecuroAPI.BusinessLogic.DTO_s.APIRegistry;
using SecuroAPI.Common.Results;

namespace SecuroAPI.BusinessLogic.Services;

public interface IVerificationService
{
    Task<Result<VerificationStatusDTO>> VerifyAsync(Guid apiId, CancellationToken ct = default);
}