using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GameVoting.Models.DatabaseModels
{
    [Table("UserProfile")]
    public class UserProfile
    {
        //Key
        [Key]
        public int UserId { get; set; }

        //Members
        [MaxLength(30)]
        [Required]
        public string UserName { get; set; }

        public DateTime CreatedDate { get; set; }

        //Navigation Properties
        public virtual ICollection<Event> OwnedEvents { get; set; }
        public virtual ICollection<EventMember> EventMemberships { get; set; } 
    }
}