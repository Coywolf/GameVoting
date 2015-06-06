using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GameVoting.Models.DatabaseModels
{
    public class WondersGame
    {
        //Key
        [Key]
        public int GameId { get; set; }

        // Foreign Key
        public int? EventId { get; set; }

        // Members
        public DateTime CreatedDate { get; set; }

        // Navigation Properties
        public virtual Event Event { get; set; }
        public virtual ICollection<WondersPlayer> Players { get; set; }
    }
}