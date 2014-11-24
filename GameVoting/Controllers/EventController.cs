using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GameVoting.Models.DatabaseModels;
using Newtonsoft.Json;

namespace GameVoting.Controllers
{
    public class EventController : Controller
    {
        //Return list of events for main page
        public string GetEvents()
        {
            return JsonConvert.SerializeObject(new { test = "hello"});
        }

        //Return data for a single event, for voting
        public string GetEvent(int eventId)
<<<<<<< HEAD
        {
            return "";
        }

        [HttpPost]
        public string CreateEvent(string data)
=======
>>>>>>> 4b9142d8c6b68334d8826f466adcd2a35bcc2b07
        {
            return "";
        }

        #region Page ActionResults
        //Main page, the list of events
        public ActionResult Index()
        {
            using (var db = new VotingContext())
            {
                ViewBag.EventTypes = db.EventType.ToList();
            }

            return View();
        }

        //Event page, for voting
        public ActionResult Event()
        {
            return View();
        }
        #endregion
    }
}
