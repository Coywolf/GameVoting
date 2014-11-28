using GameVoting.Models.DatabaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GameVoting.Models.ViewModels
{
    public class OptionSetViewModel
    {
        public int OptionSetId { get; set; }
        public string Name { get; set; }
        public List<OptionViewModel> Options { get; set; }

        public OptionSetViewModel(OptionSet os)
        {
            OptionSetId = os.OptionSetId;
            Name = os.Name;
            Options = os.Options.Select(o => new OptionViewModel(o)).ToList();
        }
        public OptionSetViewModel(){ }
    }

    public class OptionViewModel
    {
        public int OptionId { get; set; }
        public string Name { get; set; }

        public OptionViewModel(Option o)
        {
            OptionId = o.OptionId;
            Name = o.Name;
        }
        public OptionViewModel() { }
    }
}