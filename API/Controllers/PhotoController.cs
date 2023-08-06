using Application.Photos;

namespace API.Controllers
{
    public class PhotoController : ControllerTemplate
    {
        [HttpPost]
        public async Task<IActionResult> Add([FromForm] Add.Command command)
        {
            return HandleResult(await Mediator.Send(command));
        }
    }
}