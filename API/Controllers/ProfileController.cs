using Application.Profiles;

namespace API.Controllers
{
    public class ProfileController : ControllerTemplate
    {
        [HttpGet("{username}")]
        public async Task<IActionResult> GetProfile(string username)
        {
            return HandleResult(await Mediator.Send(new Details.Query{Username = username}));
        }
    }
}