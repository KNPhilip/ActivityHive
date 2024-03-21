using API.Extensions;
using Application.Core;
using MediatR;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ControllerTemplate : ControllerBase
    {
        private IMediator? _mediator;

        protected IMediator Mediator => _mediator ??=
            HttpContext.RequestServices.GetService<IMediator>()!;

        protected ActionResult HandleResult<T>(ServiceResponse<T> response)
        {
            return response.Success
                ? response.Data is null
                    ? NotFound() 
                    : Ok(response.Data) 
                : BadRequest(response.Error);
        }

        protected ActionResult HandlePagedResult<T>(ServiceResponse<PagedList<T>> response)
        {
            if (response.Success)
            {
                if (response.Data is null) 
                {
                    return NotFound();
                }

                Response.AddPaginationHeader(response.Data.CurrentPage, response.Data.PageSize,
                    response.Data.TotalCount, response.Data.TotalPages);
                return Ok(response.Data);
            }
            else 
            {
                return BadRequest(response.Error);
            }
        }
    }
}
