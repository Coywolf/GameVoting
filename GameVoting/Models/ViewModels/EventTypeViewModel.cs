using GameVoting.Models.DatabaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GameVoting.Models.ViewModels
{
    public class EventTypeViewModel
    {
        public int TypeId { get; set; }
        public string Name { get; set; }

        public EventTypeViewModel(EventType t)
        {
            TypeId = t.TypeId;
            Name = t.Name;
        }
    }
}