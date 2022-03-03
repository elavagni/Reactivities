using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using MediatR;
using Application.Comments;

namespace API.SignalR
{
    public class ChatHub : Hub
    {
        private readonly IMediator _mediator;
        public ChatHub(IMediator mediator)
        {
            this._mediator = mediator;
        }

        public async Task SendComment(Create.Command command)
        {
            var comment = await _mediator.Send(command);

            await Clients.Group(command.ActivityId.ToString())
                .SendAsync("ReceiveComment", comment.Value);

        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            //get activity id from query string (url)
            var activityId = httpContext.Request.Query["activityId"];

            await Groups.AddToGroupAsync(Context.ConnectionId, activityId);

            var result = await _mediator.Send(new List.Query { ActivityId = Guid.Parse(activityId) });

            //When the client connects, send the list of comments for the given activity
            await Clients.Caller.SendAsync("LoadComments", result.Value);

        }

    }
}