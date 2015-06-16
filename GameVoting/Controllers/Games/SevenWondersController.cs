using GameVoting.Helpers;
using GameVoting.Models.DatabaseModels;
using GameVoting.Models.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;

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
                    var users = db.UserProfile.OrderBy(u => u.UserName).ToList().Select(u => new UserViewModel(u));
                    var boards = db.WondersBoards.OrderBy(b => b.Name).ToList().Select(b => new SevenWondersBoardViewModel(b));                    

                    return JsonHelpers.SuccessResponse("", new { games, users, boards });
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
            try
            {
                var model = JsonConvert.DeserializeObject<SevenWondersGameViewModel>(data);

                using (var db = new VotingContext())
                {
                    var newGame = new WondersGame()
                    {
                        CreatedDate = DateTime.Now,
                        CreatedBy = WebSecurity.CurrentUserId
                    };
                    db.WondersGame.Add(newGame);

                    for (var i = 0; i < model.Players.Count; i++ )
                    {
                        var player = model.Players[i];
                        var user = db.UserProfile.SingleOrDefault(u => u.UserId == player.UserId);

                        var newPlayer = new WondersPlayer()
                        {
                            User = user,
                            Name = user == null ? player.Name : user.UserName,

                            BoardId = player.BoardId,
                            Seat = i,

                            MilitaryScore = player.MilitaryScore,
                            CoinScore = player.CoinScore,
                            WonderScore = player.WonderScore,
                            CivicScore = player.CivicScore,
                            CommercialScore = player.CommercialScore,
                            GuildScore = player.GuildScore,
                            ScienceScore = player.ScienceScore,
                            LeaderScore = player.LeaderScore
                        };
                        db.WondersPlayer.Add(newPlayer);
                    }

                    db.SaveChanges();
                    // reload to get the game id
                    db.Entry<WondersGame>(newGame).Reload();

                    return JsonHelpers.SuccessResponse("", new SevenWondersGameViewModel(newGame));
                }
            }
            catch (DbEntityValidationException e)
            {
                return JsonHelpers.ErrorResponse(e.InnerException.Message);
            }
            catch (Exception e)
            {
                return JsonHelpers.ErrorResponse(e.Message);
            }
        }        

        [HttpPost]
        [Authorize]
        public string DeleteGame(int gameId)
        {
            throw new NotImplementedException();
        }

        public ActionResult Index()
        {
            return View("~/Views/Games/SevenWonders.cshtml");
        }

        [Authorize]
        public ActionResult New()
        {
            return RedirectToAction("Index");

            return View("~/Views/Games/SevenWonders_New.cshtml");
        }

        public ActionResult Game(string id)
        {
            return RedirectToAction("Index");

            if (id == null)
            {
                return RedirectToAction("Index");
            }

            return View("~/Views/Games/SevenWonders_View.cshtml");
        }
    }
}