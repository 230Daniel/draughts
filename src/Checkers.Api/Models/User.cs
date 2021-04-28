using System;
using Microsoft.AspNetCore.SignalR;

namespace Checkers.Api.Models
{
    public class User
    {
        public string ConnectionId { get; }
        public event EventHandler OnDisconnected;

        public User(HubCallerContext context)
        {
            ConnectionId = context.ConnectionId;
            context.ConnectionAborted.Register(() => OnDisconnected.Invoke(this, new EventArgs()));
        }
    }
}
