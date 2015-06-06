using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GameVoting.Models.DatabaseModels
{
    public class OptionSet
    {
        //Key
        [Key]
        public int OptionSetId { get; set; }

        //Members
        [MaxLength(100)]
        [Required]
        public string Name { get; set; }

        //Navigation Properties
        public virtual ICollection<Option> Options { get; set; }
    }
}