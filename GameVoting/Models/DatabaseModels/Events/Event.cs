using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GameVoting.Models.DatabaseModels
{
    public class Event
    {
        // Key
        public int EventId { get; set; }

        // Foreign Keys
        public int CreatedBy { get; set; }
        public int TypeId { get; set; }

        // Members
        [MaxLength(250)]
        [Required]
        public string Name { get; set; }

        public bool IsPrivate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        // Navigation Properties
        [ForeignKey("CreatedBy")]
        public virtual UserProfile Creator { get; set; }

        public virtual EventType Type { get; set; }
        public virtual ICollection<EventOption> Options { get; set; }
        public virtual ICollection<EventMember> Members { get; set; }
        public virtual ICollection<WondersGame> Games { get; set; }
    }
}