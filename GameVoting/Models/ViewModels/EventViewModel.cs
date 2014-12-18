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
        public string TypeDescription { get; set; }
                
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
            TypeDescription = e.Type.Description;
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
        public bool HasVoted { get; set; }
        public bool CanClose { get; set; }

        public List<EventOptionViewModel> Options { get; set; }
        public List<UserViewModel> Members { get; set; }

        public DetailEventViewModel(Event e) 
            : base(e)
        {
            MinScore = e.Type.MinScore;
            MaxScore = e.Type.MaxScore;
            HasVoted = false;
            CanClose = false;

            var defaultScore = e.Type.Name == "Favorite" ? 0 : (int?)null;

            Options = e.Options.Select(o => new EventOptionViewModel(o, defaultScore)).OrderBy(o => o.Name).ToList();
            Members = e.Members.Select(m => new UserViewModel(m)).OrderBy(u => u.UserName).ToList();
        }

        public DetailEventViewModel(Event e, int UserId)
            : this(e)
        {
            HasVoted = true;
            CanClose = UserId == CreatedBy;

            var options = Options.OrderBy(o => o.OptionId).ToList();
            var votes = e.Members.Single(m => m.UserId == UserId).Votes.OrderBy(v => v.Option.OptionId).ToList();

            int optionPointer = 0, votePointer = 0;
            while (optionPointer < Options.Count)
            {
                if (options[optionPointer].OptionId == votes[votePointer].OptionId)
                {
                    options[optionPointer].Score = votes[votePointer].Score;
                    optionPointer++;
                    votePointer++;
                }
                else if (options[optionPointer].OptionId < votes[votePointer].OptionId)
                {
                    optionPointer++;
                }
                else
                {
                    votePointer++;
                }
            }
        }
        public DetailEventViewModel(){ }
    }

    public class EventResultsViewModel
    {
        public List<EventOptionResultViewModel> Options { get; set; }
        public bool ShowWeights { get; set; }

        public EventResultsViewModel(Event e)
        {
            Options = new List<EventOptionResultViewModel>();

            var eventType = e.Type.Name;
            var ignoreZeroes = eventType == "Ok" || eventType == "Ok-Rank";
            ShowWeights = ignoreZeroes;

            foreach (var option in e.Options)
            {
                var optionResult = new EventOptionResultViewModel(option.Name);

                foreach (var vote in option.Votes)
                {
                    optionResult.Score += vote.Score;
                    if (ignoreZeroes && optionResult.Score == 0)
                    {
                        optionResult.Weight--;
                    }
                }

                Options.Add(optionResult);
            }

            Options.Sort();
            Options.Reverse();
        }
    }
}