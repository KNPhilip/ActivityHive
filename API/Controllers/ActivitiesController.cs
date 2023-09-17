using Application.Activities;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    public class ActivitiesController : ControllerTemplate
    {
        [HttpGet]
        public async Task<IActionResult> GetActivities([FromQuery] ActivityParams request) =>
            HandlePagedResult(await Mediator.Send(new List.Query{Params = request}));

        [HttpGet("{id}")]
        public async Task<IActionResult> GetActivity(Guid id) =>
            HandleResult(await Mediator.Send(new Details.Query{Id = id}));

        [HttpPost]
        public async Task<IActionResult> CreateActivity(Activity activity) =>
            HandleResult(await Mediator.Send(new Create.Command {Activity = activity }));

        [HttpPut("{id}"), Authorize(Policy = "IsActivityHost")]
        public async Task<IActionResult> EditActivity(Guid id, Activity activity)
        {
            activity.Id = id;
            return HandleResult(await Mediator.Send(new Edit.Command {Activity = activity }));
        }

        [HttpDelete("{id}"), Authorize(Policy = "IsActivityHost")]
        public async Task<IActionResult> DeleteActivity(Guid id) =>
            HandleResult(await Mediator.Send(new Delete.Command { Id = id }));

        [HttpPost("{id}/attend")]
        public async Task<IActionResult> Attend(Guid id) =>
            HandleResult(await Mediator.Send(new UpdateAttendance.Command { Id = id }));
    }
}