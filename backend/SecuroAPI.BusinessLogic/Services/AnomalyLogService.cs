using SecuroAPI.BusinessLogic.DTO_s.AnomalyLog;
using SecuroAPI.BusinessLogic.Services.Interfaces;
using SecuroAPI.Common.Constants;
using SecuroAPI.Common.Results;
using SecuroAPI.DataAccess.Entities;
using SecuroAPI.DataAccess.Repositories.Interfaces;

namespace SecuroAPI.BusinessLogic.Services;

public class AnomalyLogService : IAnomalyLogService
{
    private readonly IAnomalyLogRepository _repository;
    private readonly IAPIRegistryRepository _apiRepository;
    private readonly IUserService _userService;

    public AnomalyLogService(IAnomalyLogRepository repository, IAPIRegistryRepository apiRepository, IUserService userService)
    {
        _repository = repository;
        _apiRepository = apiRepository;
        _userService = userService;
    }

    public async Task<Result<IEnumerable<AnomalyLogDto>>> GetAllAnomalyLogAsync()
    {
        var logs = await _repository.GetAllAsync();
        var mappedLogs = logs.Select(l => new AnomalyLogDto()
        {
            AnomalyId = l.AnomalyId,
            AnomalyType = l.AnomalyType,
            Severity = l.Severity,
            NotificationSent = l.NotificationSent,
            APIID = l.APIID,
            TimeStamp = l.TimeStamp
        });

        return Result<IEnumerable<AnomalyLogDto>>.Success(mappedLogs);
    }

    public async Task<Result<AnomalyLogDto>> GetAnomalyLogByIdAsync(Guid id)
    {
        var log = await _repository.GetByIdAsync(id);

        if (log == null || !await OwnsApi(log.APIID))
            return Result<AnomalyLogDto>
                .Failure(new Error(ErrorCodes.NotFound, $"Log with the Id' {id} ' was not found"));

        return Result<AnomalyLogDto>.Success(MapToDto(log));
    }

    public async Task<Result<AnomalyLogDto>> UpdateAnomalyLogAsync(Guid id, UpdateAnomalyLogDto? anomalyLogDto)
    {
            if (anomalyLogDto == null) return Result<AnomalyLogDto>.BadRequest();
            var log = await _repository.GetByIdAsync(id);

            if (log == null)
                return Result<AnomalyLogDto>
                    .Failure(new Error(ErrorCodes.NotFound, $"Log with the Id' {id} ' was not found"));


            log.AnomalyType = anomalyLogDto.AnomalyType;
            log.Severity = anomalyLogDto.Severity;
            log.NotificationSent = anomalyLogDto.NotificationSent;
            log.TimeStamp = DateTime.UtcNow;
            var anomalyLog = await _repository.UpdateAsync(log);

            return Result<AnomalyLogDto>.Success(new AnomalyLogDto
            {
                AnomalyId = anomalyLog.AnomalyId,
                AnomalyType = anomalyLog.AnomalyType,
                Severity = anomalyLog.Severity,
                NotificationSent = anomalyLog.NotificationSent,
                APIID = anomalyLog.APIID,
                TimeStamp = anomalyLog.TimeStamp,
            });
    }

    public async Task<Result<AnomalyLogDto>> CreateAnomalyLogAsync(CreateAnomalyLogDto? anomalyLogDto)
    {
            if (anomalyLogDto == null) return Result<AnomalyLogDto>.BadRequest();

            var log = new AnomalyLog
            {
                AnomalyType = anomalyLogDto.AnomalyType,
                Severity = anomalyLogDto.Severity,
                NotificationSent = anomalyLogDto.NotificationSent,
                APIID = anomalyLogDto.APIID,
                TimeStamp = DateTime.UtcNow,
            };
            
            var anomalyLog = await _repository.AddAsync(log);
            
                
            return Result<AnomalyLogDto>.Success(new AnomalyLogDto
            {
                AnomalyId = anomalyLog.AnomalyId,
                AnomalyType = anomalyLog.AnomalyType,
                Severity = anomalyLog.Severity,
                NotificationSent = anomalyLog.NotificationSent,
                APIID = anomalyLog.APIID,
                TimeStamp = anomalyLog.TimeStamp,
            });
    }

    public async Task<Result> DeleteAnomalyLogAsync(Guid id)
    {
            var log = await _repository.GetByIdAsync(id);
            if (log == null)
                return Result.NotFound(new Error(ErrorCodes.NotFound, $"Log with ID '{id}' does not exist"));

            await _repository.DeleteAsync(id);
            return Result.Success();

    }

    public async Task<Result<IEnumerable<AnomalyLogDto>>> GetAllAnomalyLogByAPIID(Guid id)
    {
        if (!await OwnsApi(id))
            return Result<IEnumerable<AnomalyLogDto>>.Failure(
                new Error(ErrorCodes.NotFound, $"API with the Id '{id}' was not found"));
        
        var logs = await _repository.GetAllByAPIID(id);

        return Result<IEnumerable<AnomalyLogDto>>.Success(logs.Select(MapToDto));
    }
    
    public async Task<Result<IEnumerable<AnomalyLogDto>>> GetMyAnomalyLogsAsync()
    {
        var userId = _userService.UserId;
        var since = DateTime.UtcNow.AddMonths(-3);

        var logs = await _repository.GetAllAsync(
            l => l.ApiRegistry.UserID == userId && l.TimeStamp >= since);

        return Result<IEnumerable<AnomalyLogDto>>.Success(
            logs.OrderByDescending(l => l.TimeStamp).Select(MapToDto));
    } 
    
    private async Task<bool> OwnsApi(Guid apiId)
        => await _apiRepository.CheckExistsAsync(a => a.APIID == apiId && a.UserID == _userService.UserId);
    private static AnomalyLogDto MapToDto(AnomalyLog l) => new()
    {
        AnomalyId        = l.AnomalyId,
        AnomalyType      = l.AnomalyType,
        Severity         = l.Severity,
        NotificationSent = l.NotificationSent,
        APIID            = l.APIID,
        TimeStamp        = l.TimeStamp
    };
}