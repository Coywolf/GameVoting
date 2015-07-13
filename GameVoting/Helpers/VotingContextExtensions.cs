using GameVoting.Models.DatabaseModels;
using GameVoting.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GameVoting.Helpers
{
    public static class VotingContextExtensions
    {
        public static IEnumerable<EventViewModel> GetEvents(this VotingContext context, int? userId)
        {
            var publicEvents = context.Event.Where(e => !e.IsPrivate);
            IEnumerable<Event> memberEvents;
            if (userId.HasValue)
            {
                memberEvents = context.EventMember.Where(m => m.UserId == userId.Value).Select(m => m.Event);
            }
            else
            {
                memberEvents = new List<Event>();
            }

            var events = publicEvents.Union(memberEvents).OrderByDescending(e => e.StartDate).ToList().Select(e => new EventViewModel(e));

            return events;
        }
    }
}