using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GameVoting.Models.DatabaseModels;

namespace GameVoting.Models.ViewModels
{
    public class EventViewModel
    {
        public int EventId { get; set; }
        public int CreatedBy { get; set; }
        public int TypeId { get; set; }
        public string Name { get; set; }
        public bool IsPrivate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public string Creator { get; set; }
        public string EventType { get; set; }
                
        public EventViewModel(Event e)
        {
            EventId = e.EventId;
            CreatedBy = e.CreatedBy;
            TypeId = e.TypeId;
            Name = e.Name;
            IsPrivate = e.IsPrivate;
            StartDate = e.StartDate;
            EndDate = e.EndDate;

            Creator = e.Creator.UserName;
            EventType = e.Type.Name;
        }
        public EventViewModel() { }
    }

    public class CreateEventViewModel : EventViewModel
    {
        public List<string> Options { get; set; }
        public List<int> Members { get; set; }
    }

    public class DetailEventViewModel : EventViewModel
    {
        public int? MinScore { get; set; }
        public int? MaxScore { get; set; }

        public List<EventOptionViewModel> Options { get; set; }
        public List<UserViewModel> Members { get; set; }

        public DetailEventViewModel(Event e) : base(e)
        {
            MinScore = e.Type.MinScore;
            MaxScore = e.Type.MaxScore;

            Options = e.Options.Select(o => new EventOptionViewModel(o)).ToList();
            Members = e.Members.Select(m => new UserViewModel(m)).ToList();
        }
        public DetailEventViewModel(){ }
    }
}