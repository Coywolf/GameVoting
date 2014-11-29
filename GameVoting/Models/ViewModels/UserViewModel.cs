using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GameVoting.Models.DatabaseModels;

namespace GameVoting.Models.ViewModels
{
    public class UserViewModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }

        public UserViewModel(UserProfile u)
        {
            UserId = u.UserId;
            UserName = u.UserName;
        }

        public UserViewModel(EventMember m)
        {
            UserId = m.UserId;
            UserName = m.User.UserName;
        }
        public UserViewModel() { }
    }
}