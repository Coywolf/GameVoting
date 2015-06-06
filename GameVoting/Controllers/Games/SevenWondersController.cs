using GameVoting.Helpers;
using GameVoting.Models.DatabaseModels;
using GameVoting.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GameVoting.Controllers
{
    public class SevenWondersController : Controller
    {
        // Get the list of games for the main view
        public string GetGameData()
        {
            //todo paginate or otherwise limit results
            try
            {
                using (var db = new VotingContext())
                {
                    var games = db.WondersGame.OrderByDescending(g => g.CreatedDate).ToList().Select(g => new SevenWondersGameViewModel(g));                    

                    return JsonHelpers.SuccessResponse("", new { games });
                }
            }
            catch (Exception e)
            {
                return JsonHelpers.ErrorResponse(e.Message);
            }
        }

        [Authorize]
        public string GetNewGameData()
        {
            try
            {
                using (var db = new VotingContext())
                {
                    var boards = db.WondersBoards.OrderBy(b => b.Name).ToList().Select(b => new SevenWondersBoardViewModel(b));
                    var users = db.UserProfile.OrderBy(u => u.UserName).ToList().Select(u => new UserViewModel(u));

                    return JsonHelpers.SuccessResponse("", new { boards, users });
                }
            }
            catch (Exception e)
            {
                return JsonHelpers.ErrorResponse(e.Message);
            }
        }

        [HttpPost]
        [Authorize]
        public string CreateGame(string data)
        {

        }

        public string GetGame(int gameId)
        {

        }

        [HttpPost]
        [Authorize]
        public string DeleteGame(int gameId)
        {

        }

        public ActionResult Index()
        {
            return View("~/Views/Games/SevenWonders.cshtml");
        }

        [Authorize]
        public ActionResult New()
        {
            return View("~/Views/Games/SevenWonders_New.cshtml");
        }

        public ActionResult View(string id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            return View("~/Views/Games/SevenWonders_View.cshtml");
        }
    }
}