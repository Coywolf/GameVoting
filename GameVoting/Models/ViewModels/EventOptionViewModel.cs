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
        public int? Score { get; set; }

        public EventOptionViewModel(EventOption o, int? defaultScore)
        {
            OptionId = o.OptionId;
            Name = o.Name;
            Score = defaultScore;
        }
        public EventOptionViewModel(){ }
    }

    public class EventOptionResultViewModel : IComparable<EventOptionResultViewModel>
    {
        public string Name { get; set; }
        public int Score { get; set; }
        public int Weight { get; set; }

        public EventOptionResultViewModel(string name)
        {
            Name = name;
            Score = 0;
            Weight = 1;
        }

        public EventOptionResultViewModel() { }

        public int CompareTo(EventOptionResultViewModel other)
        {
            var weightCompare = this.Weight.CompareTo(other.Weight);
            if (weightCompare == 0)
            {
                return this.Score.CompareTo(other.Score);
            }

            return weightCompare;
        }
    }
}