﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GameVoting.Models.DatabaseModels
{
    public class EventType
    {
        //Key
        [Key]
        public int TypeId { get; set; }

        //Members
        [MaxLength(100)]
        [Required]
        public string Name { get; set; }

        public int? MinScore { get; set; }  //Null = N
        public int? MaxScore { get; set; }  //Null = N

        [MaxLength(250)]
        public string Description { get; set; }

        public virtual ICollection<Event> Events { get; set; }
    }
}