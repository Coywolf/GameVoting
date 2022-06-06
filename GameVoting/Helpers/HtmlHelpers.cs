using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace GameVoting.Helpers
{
    public static class HtmlHelpers
    {
        //from http://chrisondotnet.com/2012/08/setting-active-link-twitter-bootstrap-navbar-aspnet-mvc/
        public static MvcHtmlString MenuLink(this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName)
        {
            var currentAction = htmlHelper.ViewContext.RouteData.GetRequiredString("action");
            var currentController = htmlHelper.ViewContext.RouteData.GetRequiredString("controller");

            var builder = new TagBuilder("li")
            {
                InnerHtml = htmlHelper.ActionLink(linkText, actionName, controllerName).ToHtmlString()
            };

            if (controllerName == currentController && actionName == currentAction)
                builder.AddCssClass("active");

            return new MvcHtmlString(builder.ToString());
        }
    }
}