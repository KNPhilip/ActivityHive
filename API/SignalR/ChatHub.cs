using Application.Comments;
using Application.Core;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Primitives;

namespace API.SignalR;

public sealed class ChatHub(IMediator mediator) : Hub
{
    public IMediator _mediator = mediator;

    public async Task SendComment(Create.Command command)
    {
        ServiceResponse<CommentDto>? response = await _mediator.Send(command);
        CommentDto? comment = response!.Data;

        await Clients.Group(command.ActivityId.ToString())
            .SendAsync("ReceiveComment", comment);
    }

    public override async Task OnConnectedAsync()
    {
        HttpContext _context = Context.GetHttpContext()!;
        StringValues activityId = _context!.Request.Query["activityId"];
        await Groups.AddToGroupAsync(Context.ConnectionId, activityId!);
        ServiceResponse<List<CommentDto>>? result = await _mediator
            .Send(new List.Query{ActivityId = Guid.Parse(activityId!)});
        await Clients.Caller.SendAsync("LoadComments", result!.Data);
    }
}
