using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GameVoting.Models.DatabaseModels;
using GameVoting.Models.ViewModels;
using Newtonsoft.Json;
using WebMatrix.WebData;

namespace GameVoting.Controllers
{
    public class EventController : Controller
    {
        //Return list of events for main page
        public string GetEvents()
        {
            var userId = WebSecurity.CurrentUserId;

            //todo paginate or otherwise limit results

            using (var db = new VotingContext())
            {
                var publicEvents = db.Event.Where(e => !e.IsPrivate);
                var memberEvents = db.EventMember.Where(m => m.UserId == userId).Select(m => m.Event);

                var events = publicEvents.Union(memberEvents).OrderByDescending(e => e.StartDate).ToList().Select(e => new EventViewModel(e));
                
                return JsonConvert.SerializeObject(new { events });
            }
        }

        //Return data for a single event, for voting
        [Authorize]
        public string GetEvent(int eventId)
        {
            return "";
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
            using (var db = new VotingContext())
            {
                ViewBag.EventTypes = db.EventType.ToList();
                ViewBag.Users = db.UserProfile.OrderBy(u => u.UserName).ToList();
            }

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
