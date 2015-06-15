using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GameVoting.Models.DatabaseModels
{
    public class WondersPlayer
    {
        // Key
        [Key]
        public int PlayerId { get; set; }

        // Foreign Keys
        public int GameId { get; set; }
        public int? UserId { get; set; }
        public int? BoardId { get; set; }

        // Members
        [MaxLength(30)]
        [Required]
        public string Name { get; set; }

        public int Seat { get; set; }

        public int MilitaryScore { get; set; }
        public int CoinScore { get; set; }
        public int WonderScore { get; set; }
        public int CivicScore { get; set; }
        public int CommercialScore { get; set; }
        public int GuildScore { get; set; }
        public int ScienceScore { get; set; }
        public int LeaderScore { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public int TotalScore   // todo: not sure what this will do
        {
            get
            {
                return MilitaryScore +
                    CoinScore +
                    WonderScore +
                    CivicScore +
                    CommercialScore +
                    GuildScore +
                    ScienceScore +
                    LeaderScore;
            }
            private set { }
        }

        // Navigation Properties
        public virtual WondersGame Game { get; set; }
        public virtual UserProfile User { get; set; }
        public virtual WondersBoards Board { get; set; }
    }
}