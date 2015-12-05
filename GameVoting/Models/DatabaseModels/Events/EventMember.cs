using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GameVoting.Models.DatabaseModels
{
    public class EventMember
    {
        // Key
        [Key]
        public int MemberId { get; set; }

        // Foreign Keys
        [Index("IX_EventUser", 1, IsUnique = true)]
        public int EventId { get; set; }
        [Index("IX_EventUser", 2, IsUnique = true)]
        public int UserId { get; set; }

        // Members
        public DateTime JoinedDate { get; set; }

        private bool _hasDeferred = false;
        [DefaultValue(false)]
        public bool HasDeferred
        {
            get { return _hasDeferred; }
            set { _hasDeferred = value; }
        }

        // Navigation Properties
        public virtual Event Event { get; set; }
        public virtual UserProfile User { get; set; }
        public virtual ICollection<EventVote> Votes { get; set; }
    }
}