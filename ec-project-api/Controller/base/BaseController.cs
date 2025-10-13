using Microsoft.AspNetCore.Mvc;
using ec_project_api.Dtos.response;

namespace ec_project_api.Controllers.Base
{
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        protected async Task<ActionResult<T>> ExecuteAsync<T>(
            Func<Task<T>> action,
            int? successStatusCode = null)
        {
            try
            {
                var result = await action();
                if (successStatusCode.HasValue)
                    return StatusCode(successStatusCode.Value, result);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ResponseData<T>.Error(StatusCodes.Status404NotFound, ex.Message));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ResponseData<T>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ResponseData<T>.Error(StatusCodes.Status409Conflict, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ResponseData<T>.Error(StatusCodes.Status500InternalServerError,
                        ex.InnerException?.Message ?? ex.Message));
            }
        }

        protected string GetModelErrors()
        {
            return string.Join("; ", ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage));
        }
    }
}
