﻿using System;
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
            //todo paginate or otherwise limit results

            using (var db = new VotingContext())
            {
                //todo only private if current user is a member
                var events = db.Event.OrderByDescending(e => e.StartDate).ToList().Select(e => new EventViewModel(e));
                
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
                var model = JsonConvert.DeserializeObject<EventViewModel>(data);

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
                    //todo options
                    db.Event.Add(newEvent);
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
