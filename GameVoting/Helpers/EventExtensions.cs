using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GameVoting.Models.DatabaseModels;
using GameVoting.Models.ViewModels;

namespace GameVoting.Helpers
{
    public static class EventExtensions
    {
        public static string ValidateVotes(this Event e, List<EventOptionViewModel> votes)
        {
            var errorMessage = "";

            if (votes.Count() != e.Options.Count || votes.Any(v => !v.Score.HasValue))
            {
                errorMessage = "A score must be chosen for all options";
            }
            else
            {
                if (e.Type.Name == "Favorite")
                {
                    if (votes.Count(v => v.Score.Value == 1) > 1)
                    {
                        errorMessage = "Only one option may be chosen as your favorite";
                    }
                }
                else if (e.Type.Name == "Rank")
                {
                    if (votes.Select(v => v.Score.Value).Distinct().Count() != votes.Count())
                    {
                        errorMessage = "All options must be assigned a unique rank";
                    }
                }
                else
                {
                    //No restriction on selections
                }
            }

            return errorMessage;
        }
    }
}