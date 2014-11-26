using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GameVoting.Models.DatabaseModels;

namespace GameVoting.Models.ViewModels
{
    public class EventOptionViewModel
    {
        public int OptionId { get; set; }
        public string Name { get; set; }

        public EventOptionViewModel(EventOption o)
        {
            OptionId = o.OptionId;
            Name = o.Name;
        }
        public EventOptionViewModel(){ }
    }
}