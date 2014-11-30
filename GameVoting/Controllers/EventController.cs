using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GameVoting.Models.DatabaseModels;
using GameVoting.Models.ViewModels;
using Newtonsoft.Json;
using WebMatrix.WebData;
using GameVoting.Helpers;

namespace GameVoting.Controllers
{
    public class EventController : Controller
    {
        //Return list of events for main page
        public string GetEventData()
        {
            var userId = WebSecurity.CurrentUserId;

            //todo paginate or otherwise limit results

            using (var db = new VotingContext())
            {
                var publicEvents = db.Event.Where(e => !e.IsPrivate);
                var memberEvents = db.EventMember.Where(m => m.UserId == userId).Select(m => m.Event);

                var events = publicEvents.Union(memberEvents).OrderByDescending(e => e.StartDate).ToList().Select(e => new EventViewModel(e));

                //static data
                var eventTypes = db.EventType.OrderBy(t => t.Name).ToList().Select(t => new EventTypeViewModel(t));
                var users = db.UserProfile.OrderBy(u => u.UserName).ToList().Select(u => new UserViewModel(u));
                var optionSets = db.OptionSet.OrderBy(o => o.Name).ToList().Select(o => new OptionSetViewModel(o));
                
                return JsonConvert.SerializeObject(new { events, eventTypes, users, optionSets });
            }
        }

        //Return data for a single event, for voting
        [Authorize]
        public string GetEvent(int eventId)
        {
            var userId = WebSecurity.CurrentUserId;

            using (var db = new VotingContext())
            {
                var eventRow = db.Event.SingleOrDefault(e => e.EventId == eventId);
                var member = eventRow.Members.SingleOrDefault(m => m.UserId == userId);
                if (eventRow != null && (!eventRow.IsPrivate || member != null))
                {
                    if (member != null && member.Votes.Any())
                    {
                        //user has votes, return with their vote data
                        return JsonConvert.SerializeObject(new DetailEventViewModel(eventRow, userId));
                    }
                    else
                    {
                        return JsonConvert.SerializeObject(new DetailEventViewModel(eventRow));
                    }
                }
            }

            return "";
        }

        [HttpPost]
        [Authorize]
        public string SubmitVote(int eventId, string voteData)
        {
            try
            {
                var votes = JsonConvert.DeserializeObject<List<EventOptionViewModel>>(voteData);

                using (var db = new VotingContext())
                {
                    var eventRow = db.Event.Single(e => e.EventId == eventId);
                    EventMember member;

                    if (eventRow.IsPrivate)
                    {
                        member = eventRow.Members.SingleOrDefault(m => m.UserId == WebSecurity.CurrentUserId);
                        if (member == null)
                        {
                            return JsonHelpers.ErrorResponse("User is not a member of this event");
                        }
                    }
                    else
                    {
                        member = new EventMember
                        {
                            Event = eventRow,
                            UserId = WebSecurity.CurrentUserId,
                            JoinedDate = DateTime.Now
                        };
                        db.EventMember.Add(member);
                    }

                    if (member.Votes != null && member.Votes.Any())
                    {
                        //member has voted already
                        return JsonHelpers.ErrorResponse("Member has already voted");
                    }
                    //todo vote data must be valid for the event type and options

                    foreach (var vote in votes)
                    {
                        var newVote = new EventVote
                        {
                            OptionId = vote.OptionId,
                            Member = member,
                            Score = vote.Score.Value,
                            LastUpdatedDate = DateTime.Now
                        };
                        db.EventVote.Add(newVote);
                    }

                    db.SaveChanges();
                    return JsonHelpers.SuccessResponse("Votes saved");
                }
            }
            catch (Exception e)
            {
                return JsonHelpers.ErrorResponse(e.Message);
            }
        }

        [HttpPost]
        [Authorize]
        public string CreateEvent(string data)
        {
            try
            {
                var model = JsonConvert.DeserializeObject<CreateEventViewModel>(data);

                using (var db = new VotingContext())
                {
                    var newEvent = new Event
                        {
                            Name = model.Name,
                            TypeId = model.TypeId,
                            CreatedBy = WebSecurity.CurrentUserId,
                            IsPrivate = model.IsPrivate,
                            StartDate = DateTime.Now
                        };
                    db.Event.Add(newEvent);

                    foreach (var option in model.Options)
                    {
                        var newOption = new EventOption
                            {
                                Event = newEvent,
                                Name = option
                            };
                        db.EventOption.Add(newOption);
                    }

                    foreach (var userId in model.Members)
                    {
                        var newMember = new EventMember
                            {
                                Event = newEvent,
                                UserId = userId,
                                JoinedDate = DateTime.Now
                            };
                        db.EventMember.Add(newMember);
                    }

                    //make sure the creator is added on a private event
                    if (model.IsPrivate && !model.Members.Any(m => m == WebSecurity.CurrentUserId))
                    {
                        var creatorMember = new EventMember
                        {
                            Event = newEvent,
                            UserId = WebSecurity.CurrentUserId,
                            JoinedDate = DateTime.Now
                        };
                        db.EventMember.Add(creatorMember);
                    }
                    
                    db.SaveChanges();
                    db.Entry<Event>(newEvent).Reload();
                    
                    return JsonConvert.SerializeObject(new { newEvent.EventId });
                }
            }
            catch (Exception e)
            {
                return JsonConvert.SerializeObject(new { EventId = 0 });
            }
        }

        #region Page ActionResults
        //Main page, the list of events
        public ActionResult Index()
        {
            return View();
        }

        //Event page, for voting
        [Authorize]
        public ActionResult View(int? id)
        {
            if (id == null)
            {
                return new RedirectResult("/");
            }
            return View();
        }
        #endregion
    }
}
