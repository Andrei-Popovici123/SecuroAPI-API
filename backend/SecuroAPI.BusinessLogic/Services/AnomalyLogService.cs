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

    public AnomalyLogService(IAnomalyLogRepository repository)
    {
        _repository = repository;
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

        if (log == null)
            return Result<AnomalyLogDto>
                .Failure(new Error(ErrorCodes.NotFound, $"Log with the Id' {id} ' was not found"));

        return Result<AnomalyLogDto>.Success(new AnomalyLogDto
        {
            AnomalyId = log.AnomalyId,
            AnomalyType = log.AnomalyType,
            Severity = log.Severity,
            NotificationSent = log.NotificationSent,
            APIID = log.APIID,
            TimeStamp = log.TimeStamp
        });
    }

    public async Task<Result<AnomalyLogDto>> UpdateAnomalyLogAsync(Guid id, UpdateAnomalyLogDto? anomalyLogDto)
    {
        try
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
        catch (Exception)
        {
            return Result<AnomalyLogDto>.Failure();
        }
    }

    public async Task<Result<AnomalyLogDto>> CreateAnomalyLogAsync(CreateAnomalyLogDto? anomalyLogDto)
    {
        try
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
        catch (Exception)
        {
            return Result<AnomalyLogDto>.Failure();
        }
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
        var logs = await _repository.GetAllByAPIID(id);
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
}