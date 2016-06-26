using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GameVoting.Models.DatabaseModels
{
    public class GameSetGame
    {
        // Key
        [Key]
        public int GameSetGameId { get; set; }

        // Foreign Keys
        [Index("IX_GameSetGame", 1, IsUnique = true)]
        public int GameSetId { get; set; }
        [Index("IX_GameSetGame", 2, IsUnique = true)]
        public int GameId { get; set; }

        // Navigation Properties
        public virtual GameSet GameSet { get; set; }
        public virtual Game Game { get; set; }
    }
}