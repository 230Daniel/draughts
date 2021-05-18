using System;
using System.Collections.Generic;
using System.Linq;
using Draughts.Api.Models;
using Microsoft.AspNetCore.SignalR;

namespace Draughts.Api.Services
{
    public interface IUserService
    {
        User GetOrCreateUser(HubCallerContext context);
    }

    public class UserService : IUserService
    {
        List<User> _users;

        public UserService()
        {
            _users = new List<User>();
        }

        public User GetOrCreateUser(HubCallerContext context)
        {
            User user = _users.FirstOrDefault(x => x.ConnectionId == context.ConnectionId);
            if (user is null)
            {
                user = new User(context);
                user.OnDisconnected += UserDisconnected;
                _users.Add(user);
            }
            return user;
        }

        void UserDisconnected(object sender, EventArgs e)
        {
            User user = (User)sender;
            _users.RemoveAll(x => x.ConnectionId == user.ConnectionId);
        }
    }
}
