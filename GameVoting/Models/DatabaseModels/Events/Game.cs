using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GameVoting.Models.DatabaseModels
{
    public class Game
    {
        //Key
        [Key]
        public int GameId { get; set; }

        //Members
        [MaxLength(100)]
        [Required]
        public string Name { get; set; }
        [MaxLength(10)]
        public string BggId { get; set; }
        public int? MinimumPlayers { get; set; }
        public int? MaximumPlayers { get; set; }        
        public string Description { get; set; }
        public int? CreatedBy { get; set; }

        //Navigation Properties
        [ForeignKey("CreatedBy")]
        public virtual UserProfile Creator { get; set; }

        public virtual ICollection<GameSetGame> GameSets { get; set; }
    }
}