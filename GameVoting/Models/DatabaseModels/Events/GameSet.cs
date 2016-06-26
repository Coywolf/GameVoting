using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GameVoting.Models.DatabaseModels
{
    public class GameSet
    {
        //Key
        [Key]
        public int GameSetId { get; set; }

        //Members
        [MaxLength(100)]
        [Required]
        public string Name { get; set; }

        //Navigation Properties
        public virtual ICollection<GameSetGame> Games { get; set; }
    }
}