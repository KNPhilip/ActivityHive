using System.Reflection.Metadata;
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

        [HttpPut]
        public async Task<IActionResult> Edit(Edit.Command command)
        {
            return HandleResult(await Mediator.Send(command));
        }

        [HttpGet("{username}/activities")]
        public async Task<IActionResult> GetProfileActivities(string username, [FromQuery] string predicate)
        {
            return HandleResult(await Mediator.Send(new ListActivities.Query{Username = username, Predicate = predicate}));
        }
    }
}