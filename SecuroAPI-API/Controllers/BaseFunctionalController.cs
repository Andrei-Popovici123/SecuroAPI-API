using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SecuroAPI.Common.Constants;
using SecuroAPI.Common.Results;

namespace SecuroAPI_API.Controllers;

public abstract class BaseFunctionalController : ControllerBase
{
    protected ActionResult<T> ToActionResult<T>(Result<T> result)
    {
        return result.IsSuccess ? Ok(result.Value) : MapErrorToResponse(result.Errors);
    }

    protected ActionResult ToActionResult(Result result)
    {
        return result.IsSuccess ? Ok() : MapErrorToResponse(result.Errors);
    }

    protected ActionResult MapErrorToResponse(Error[]? errors)
    {
        if (errors is null || errors.Length == 0) return Problem();
        
        var e = errors[0];
        return e.Code switch
        {
            ErrorCodes.NotFound => NotFound(e.Description),
            ErrorCodes.Validation => BadRequest(e.Description),
            ErrorCodes.BadRequest => BadRequest(e.Description),
            ErrorCodes.Conflict => Conflict(e.Description),
            ErrorCodes.Forbidden => Conflict(e.Description),
            _ => Problem(detail: string.Join(";", errors.Select(x=> x.Description)),title: e.Code)
        };
    }
}