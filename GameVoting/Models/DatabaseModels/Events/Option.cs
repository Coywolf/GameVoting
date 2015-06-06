using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GameVoting.Models.DatabaseModels
{
    public class Option
    {
        //Key
        [Key]
        public int OptionId { get; set; }

        //Foreign Keys
        public int OptionSetId { get; set; }

        //Members
        [MaxLength(100)]
        [Required]
        public string Name { get; set; }

        //Navigation Properties
        public virtual OptionSet OptionSet { get; set; }
    }
}