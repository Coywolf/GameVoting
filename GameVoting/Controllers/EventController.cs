using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GameVoting.Models.DatabaseModels;

namespace GameVoting.Controllers
{
    public class EventController : Controller
    {
        //Main page, the list of events
        public ActionResult Index()
        {
            return View();
        }

        //View and vote in a specific event
        public ActionResult Event(int eventId)
        {
            return View();
        }
    }
}
