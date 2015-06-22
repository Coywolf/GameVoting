using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetOpenAuth.OpenId.Extensions.AttributeExchange;
using GameVoting.Models.DatabaseModels;
using GameVoting.Models.ViewModels;
using WebMatrix.WebData;

namespace GameVoting.Helpers
{
    public static class EventExtensions
    {
        public static string ValidateVotes(this Event e, List<EventOptionViewModel> votes)
        {
            //Check that all options have a score
            if (votes.Count() != e.Options.Count || votes.Any(v => !v.Score.HasValue))
            {
                return "A score must be chosen for all options";
            }

            //Check uniqueness of selections, depending on the vote type
            if (e.Type.Name == "Favorite")
            {
                if (votes.Count(v => v.Score.Value == 1) > 1)
                {
                    return "Only one option may be chosen as your favorite";
                }
            }
            else if (e.Type.Name == "Rank")
            {
                if (votes.Select(v => v.Score.Value).Distinct().Count() != votes.Count())
                {
                    return "All options must be assigned a unique rank";
                }
            }
            else
            {
                //No restriction on selections
            }

            var min = e.Type.MinScore ?? e.Options.Count;
            var max = e.Type.MaxScore ?? e.Options.Count;

            if (votes.Any(v => v.Score < min || v.Score > max))
            {
                //try
                //{
                //    System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();
                //    message.To.Add("michael.bush@inin.com");
                //    message.Subject = "Cheater";
                //    message.From = new System.Net.Mail.MailAddress("michael.bush@inin.com");
                //    message.Body = WebSecurity.CurrentUserName;
                //    System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient("smtp.inin.com");
                //    smtp.Send(message);
                //}
                //catch (Exception ex)
                //{
                    
                //}

                return "Stop hacking the values. Bush knows who you are.";
            }

            return "";
        }
    }
}