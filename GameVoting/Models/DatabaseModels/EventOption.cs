using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GameVoting.Models.DatabaseModels
{
    public class EventOption
    {
        //Key
        [Key]
        public int OptionId { get; set; }

        //Foreign Keys
        public int EventId { get; set; }

        //Members
        [MaxLength(100)]
        [Required]
        public string Name { get; set; }

        //Navigation Properties
        public virtual Event Event { get; set; }
        public virtual ICollection<EventVote> Votes { get; set; } 
    }
}