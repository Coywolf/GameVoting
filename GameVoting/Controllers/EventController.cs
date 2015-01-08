﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GameVoting.Hubs;
using GameVoting.Models.DatabaseModels;
using GameVoting.Models.ViewModels;
using Microsoft.AspNet.SignalR;
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
            //todo paginate or otherwise limit results
            try
            {
                using (var db = new VotingContext())
                {
                    var publicEvents = db.Event.Where(e => !e.IsPrivate);
                    IEnumerable<Event> memberEvents;
                    if (WebSecurity.IsAuthenticated)
                    {
                        memberEvents = db.EventMember.Where(m => m.UserId == WebSecurity.CurrentUserId).Select(m => m.Event);
                    }
                    else
                    {
                        memberEvents = new List<Event>();
                    }

                    var events = publicEvents.Union(memberEvents).OrderByDescending(e => e.StartDate).ToList().Select(e => new EventViewModel(e));

                    //static data
                    var eventTypes = db.EventType.OrderBy(t => t.Name).ToList().Select(t => new EventTypeViewModel(t));
                    var users = db.UserProfile.OrderBy(u => u.UserName).ToList().Select(u => new UserViewModel(u));
                    var optionSets = db.OptionSet.OrderBy(o => o.Name).ToList().Select(o => new OptionSetViewModel(o));

                    return JsonHelpers.SuccessResponse("", new { events, eventTypes, users, optionSets });
                }
            }
            catch (Exception e)
            {
                return JsonHelpers.ErrorResponse(e.Message);
            }
        }
        
        //Return data for a single event, for voting
        [System.Web.Mvc.Authorize]
        public string GetEvent(int eventId)
        {
            var userId = WebSecurity.CurrentUserId;

            using (var db = new VotingContext())
            {
                var eventRow = db.Event.SingleOrDefault(e => e.EventId == eventId);

                if (eventRow != null)
                {
                    var member = eventRow.Members.SingleOrDefault(m => m.UserId == userId);

                    DetailEventViewModel responseEvent = null;      //event data and user votes
                    EventResultsViewModel responseResults = null;   //result data - anonymous

                    if (eventRow.IsPrivate && member == null)
                    {
                        return JsonHelpers.ErrorResponse("User is not a member of a private event.");
                    }
                    
                    if (member != null && member.Votes.Any())
                    {
                        //current user has voted, get their votes
                        responseEvent = new DetailEventViewModel(eventRow, userId);
                        responseResults = new EventResultsViewModel(eventRow);
                    }
                    else
                    {
                        responseEvent = new DetailEventViewModel(eventRow);
                    }

                    //checking for null because results may have been created above
                    if (responseResults == null && !eventRow.IsPrivate && eventRow.EndDate != null)
                    {
                        //closed public event
                        responseResults = new EventResultsViewModel(eventRow);
                    }

                    return JsonHelpers.SuccessResponse("", new
                    {
                        @event = responseEvent,
                        results = responseResults
                    });
                }
                else
                {
                    return JsonHelpers.ErrorResponse("Event not found.");
                }
            }
        }

        [System.Web.Mvc.Authorize]
        public string GetEventResults(int eventId)
        {
            using (var db = new VotingContext())
            {
                var eventRow = db.Event.SingleOrDefault(e => e.EventId == eventId);
                if (eventRow != null)
                {
                    var member = eventRow.Members.SingleOrDefault(m => m.UserId == WebSecurity.CurrentUserId);
                    if (!eventRow.IsPrivate || member != null)
                    {
                        return JsonHelpers.SuccessResponse("", new EventResultsViewModel(eventRow));
                    }
                    else
                    {
                        return JsonHelpers.ErrorResponse("User is not a member of a private event.");
                    }
                }
                else
                {
                    return JsonHelpers.ErrorResponse("Event not found.");
                }
            }
        }

        [HttpPost]
        [System.Web.Mvc.Authorize]
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

                    if (eventRow.EndDate != null)
                    {
                        return JsonHelpers.ErrorResponse("This event has closed");
                    }

                    if (member.Votes != null && member.Votes.Any())
                    {
                        //member has voted already
                        return JsonHelpers.ErrorResponse("Member has already voted");
                    }

                    var validateError = eventRow.ValidateVotes(votes);
                    if (!String.IsNullOrEmpty(validateError))
                    {
                        return JsonHelpers.ErrorResponse(validateError);
                    }
                    
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

                    var eventHubContext = GlobalHost.ConnectionManager.GetHubContext<EventHub>();
                    eventHubContext.Clients.Group(eventId.ToString()).UpdateResults(new EventResultsViewModel(eventRow));
                    return JsonHelpers.SuccessResponse("Votes saved");
                }
            }
            catch (Exception e)
            {
                return JsonHelpers.ErrorResponse(e.Message);
            }
        }

        [HttpPost]
        [System.Web.Mvc.Authorize]
        public string CloseEvent(int eventId)
        {
            try
            {
                using (var db = new VotingContext())
                {
                    var eventRow = db.Event.Single(e => e.EventId == eventId);

                    if (eventRow.EndDate != null)
                    {
                        return JsonHelpers.ErrorResponse("Event is already closed");
                    }
                    if (eventRow.CreatedBy != WebSecurity.CurrentUserId)
                    {
                        return JsonHelpers.ErrorResponse("Only the creator of an event may close it.");
                    }

                    eventRow.EndDate = DateTime.Now;
                    db.SaveChanges();

                    return JsonHelpers.SuccessResponse("", eventRow.EndDate);
                }
            }
            catch (Exception e)
            {
                return JsonHelpers.ErrorResponse(e.Message);
            }
        }

        [HttpPost]
        [System.Web.Mvc.Authorize]
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
                    if (model.IsPrivate && model.Members.All(m => m != WebSecurity.CurrentUserId))
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
                    
                    return JsonHelpers.SuccessResponse("", new { newEvent.EventId });
                }
            }
            catch (Exception e)
            {
                return JsonHelpers.ErrorResponse(e.Message);
            }
        }

        public string AddMember(int eventId, int userId)
        {
            try
            {
                using (var db = new VotingContext())
                {
                    var eventRow = db.Event.Single(e => e.EventId == eventId);

                    if(WebSecurity.CurrentUserId != eventRow.CreatedBy)
                    {
                        return JsonHelpers.ErrorResponse("Only the event creator may add members to an event.");
                    }
                    if (!eventRow.IsPrivate)
                    {
                        return
                            JsonHelpers.ErrorResponse(
                                "Cannot add member to a public event. The target user will be added when they submit a vote.");
                    }
                    if (eventRow.EndDate != null)
                    {
                        return JsonHelpers.ErrorResponse("Event has already closed, cannot add new members.");
                    }

                    var newMember = new EventMember
                    {
                        Event = eventRow,
                        UserId = userId,
                        JoinedDate = DateTime.Now
                    };
                    db.EventMember.Add(newMember);

                    db.SaveChanges();

                    return JsonHelpers.SuccessResponse("New member has been added");
                }
            }
            catch (Exception e)
            {
                return JsonHelpers.ErrorResponse(e.Message);
            }
        }

        #region Page ActionResults
        //Main page, the list of events
        public ActionResult Index()
        {
            return View();
        }

        //Event page, for voting
        [System.Web.Mvc.Authorize]
        public new ActionResult View(string id)
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
