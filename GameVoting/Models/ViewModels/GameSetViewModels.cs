using GameVoting.Models.DatabaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GameVoting.Models.ViewModels
{
    public class GameSetViewModel
    {
        public int GameSetId { get; set; }
        public string Name { get; set; }
        public List<GameViewModel> Games { get; set; }

        public GameSetViewModel(GameSet os)
        {
            GameSetId = os.GameSetId;
            Name = os.Name;
            Games = os.Games.Select(o => new GameViewModel(o.Game)).ToList();
        }
        public GameSetViewModel(){ }
    }

    public class GameViewModel
    {
        public int GameId { get; set; }
        public string Name { get; set; }

        public GameViewModel(Game o)
        {
            GameId = o.GameId;
            Name = o.Name;
        }
        public GameViewModel() { }
    }
}