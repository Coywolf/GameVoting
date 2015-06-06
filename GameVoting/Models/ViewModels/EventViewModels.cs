using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GameVoting.Models.DatabaseModels;
using WebMatrix.WebData;

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
        public bool IsCreator { get; set; }

        public List<EventOptionViewModel> Options { get; set; }
        public List<UserViewModel> Members { get; set; }

        public DetailEventViewModel(Event e) 
            : base(e)
        {
            MinScore = e.Type.MinScore;
            MaxScore = e.Type.MaxScore;
            HasVoted = false;
            IsCreator = WebSecurity.CurrentUserId == e.CreatedBy;

            var defaultScore = e.Type.Name == "Favorite" ? 0 : (int?)null;

            Options = e.Options.Select(o => new EventOptionViewModel(o, defaultScore)).OrderBy(o => o.Name).ToList();

            if (IsPrivate || EndDate != null)
            {
                //return members only if the event is private or closed
                Members = e.Members.Select(m => new UserViewModel(m)).OrderBy(u => u.UserName).ToList();
            }
            else
            {
                Members = new List<UserViewModel>();
            }
        }

        public DetailEventViewModel(Event e, int UserId)
            : this(e)
        {
            HasVoted = true;

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
        public int NumberOfVotes { get; set; }

        public EventResultsViewModel(Event e)
        {
            Options = new List<EventOptionResultViewModel>();
            NumberOfVotes = 0;

            var eventType = e.Type.Name;
            var weightZeroes = eventType == "Ok-Rank";
            ShowWeights = weightZeroes;

            foreach (var option in e.Options)
            {
                var optionResult = new EventOptionResultViewModel(option.Name);
                int curNumVotes = 0;

                foreach (var vote in option.Votes)
                {
                    optionResult.Score += vote.Score;
                    if (weightZeroes && vote.Score > 0)
                    {
                        optionResult.Weight++;
                    }

                    curNumVotes++;
                }

                Options.Add(optionResult);
                if (curNumVotes > NumberOfVotes)
                {
                    NumberOfVotes = curNumVotes;
                }
            }

            //shift all weights up so that all weights are at a minimum of zero
            //this is to display better on the chart
            //var minWeight = Options.Min(o => o.Weight);
            //if (minWeight < 0)
            //{
            //    var addWeight = Math.Abs(minWeight);
            //    foreach (var option in Options)
            //    {
            //        option.Weight += addWeight;
            //    }
            //}

            //sort by weight then by score
            Options.Sort();
            //descending
            Options.Reverse();
        }
    }
}