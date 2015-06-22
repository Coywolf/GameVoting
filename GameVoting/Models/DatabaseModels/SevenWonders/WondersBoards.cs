using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GameVoting.Models.DatabaseModels
{
    public class WondersBoards
    {
        // Key
        [Key]
        public int BoardId { get; set; }

        // Members
        [MaxLength(250)]
        [Required]
        public string Name { get; set; }

        // Navigation Properties
        public virtual ICollection<WondersPlayer> Players { get; set; }
    }
}