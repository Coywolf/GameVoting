using GameVoting.Models.DatabaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GameVoting.Models.ViewModels
{
    public class SevenWondersGameViewModel
    {
        public int GameId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Creator { get; set; }

        public List<SevenWondersPlayerViewModel> Players { get; set; }
        
        public SevenWondersGameViewModel(WondersGame game)
        {
            GameId = game.GameId;
            CreatedDate = game.CreatedDate;
            Creator = game.Creator.UserName;

            Players = game.Players.OrderBy(p => p.Seat).Select(p => new SevenWondersPlayerViewModel(p)).ToList();
        }

        public SevenWondersGameViewModel()
        {
        }
    }

    public class SevenWondersPlayerViewModel
    {
        public int? UserId { get; set; }
        public int? BoardId { get; set; }

        public string Name { get; set; }
        public string BoardName { get; set; }

        public int MilitaryScore { get; set; }
        public int CoinScore { get; set; }
        public int WonderScore { get; set; }
        public int CivicScore { get; set; }
        public int CommercialScore { get; set; }
        public int GuildScore { get; set; }
        public int ScienceScore { get; set; }
        public int LeaderScore { get; set; }

        public SevenWondersPlayerViewModel(WondersPlayer player) 
        {
            UserId = player.UserId;
            BoardId = player.BoardId;

            Name = player.Name;
            BoardName = player.Board == null ? "--" : player.Board.Name;

            MilitaryScore = player.MilitaryScore;
            CoinScore = player.CoinScore;
            WonderScore = player.WonderScore;
            CivicScore = player.CivicScore;
            CommercialScore = player.CommercialScore;
            GuildScore = player.GuildScore;
            ScienceScore = player.ScienceScore;
            LeaderScore = player.LeaderScore;
        }

        public SevenWondersPlayerViewModel()
        {
        }
    }

    public class SevenWondersBoardViewModel
    {
        public int BoardId { get; set; }
        public string Name { get; set; }

        public SevenWondersBoardViewModel(WondersBoards board)
        {
            BoardId = board.BoardId;
            Name = board.Name;
        }
    }
}