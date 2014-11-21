using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GameVoting.Models.DatabaseModels
{
    public class EventVote
    {
        //Key
        [Key]
        public int VoteId { get; set; }

        //Foreign Keys
        [Index("IX_OptionMember", 1, IsUnique = true)]
        public int OptionId { get; set; }
        [Index("IX_OptionMember", 2, IsUnique = true)]
        public int MemberId { get; set; }

        //Members
        public int Score { get; set; }
        public DateTime LastUpdatedDate { get; set; }

        //Navigation Properties
        public virtual EventOption Option { get; set; }
        public virtual EventMember Member { get; set; }
    }
}