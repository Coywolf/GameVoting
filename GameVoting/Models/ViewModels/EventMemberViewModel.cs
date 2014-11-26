using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GameVoting.Models.DatabaseModels;

namespace GameVoting.Models.ViewModels
{
    public class EventMemberViewModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }

        public EventMemberViewModel(EventMember m)
        {
            UserId = m.UserId;
            UserName = m.User.UserName;
        }
        public EventMemberViewModel(){ }
    }
}