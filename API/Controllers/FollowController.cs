using Application.Followers;

namespace API.Controllers
{
    public class FollowController : ControllerTemplate
    {
        [HttpPost("{username}")]
        public async Task<IActionResult> Follow(string username) =>
            HandleResult(await Mediator.Send(new FollowToggle.Command{TargetUsername = username}));

        [HttpGet("{username}")]
        public async Task<IActionResult> GetFollowings(string username, string predicate) =>
            HandleResult(await Mediator.Send(new List.Query{Username = username, Predicate = predicate}));
    }
}
