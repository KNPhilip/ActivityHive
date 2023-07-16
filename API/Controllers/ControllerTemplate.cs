using MediatR;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ControllerTemplate : ControllerBase
    {
        private IMediator? _mediator;

        protected IMediator Mediator => _mediator ??=
            HttpContext.RequestServices.GetService<IMediator>();
    }
}