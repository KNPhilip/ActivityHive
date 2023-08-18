using Application.Followers;

namespace API.Controllers
{
    public class FollowController : ControllerTemplate
    {
        [HttpPost("{username}")]
        public async Task<IActionResult> Follow(string username)
        {
            return HandleResult(await Mediator.Send(new FollowToggle.Command{TargetUsername = username}));
        }
    }
}