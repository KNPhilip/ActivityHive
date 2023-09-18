using Application.Comments;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    public class ChatHub : Hub
    {
        public IMediator _mediator;

        public ChatHub(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task SendComment(Create.Command command)
        {
            var response = await _mediator.Send(command);
            CommentDto? comment = response!.Data;

            await Clients.Group(command.ActivityId.ToString())
                .SendAsync("ReceiveComment", comment);
        }

        public override async Task OnConnectedAsync()
        {
            HttpContext _context = Context.GetHttpContext()!;
            var activityId = _context!.Request.Query["activityId"];
            await Groups.AddToGroupAsync(Context.ConnectionId, activityId!);
            var result = await _mediator.Send(new List.Query{ActivityId = Guid.Parse(activityId!)});
            await Clients.Caller.SendAsync("LoadComments", result!.Data);
        }
    }
}