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
            if (response is null) return NotFound();
            if (response.Success)
                return response.Data is null ? NotFound() : Ok(response.Data);
            return BadRequest(response.Error);
        }
    }
}