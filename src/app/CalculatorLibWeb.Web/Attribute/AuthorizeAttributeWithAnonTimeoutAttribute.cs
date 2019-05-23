using System;
using System.Globalization;
using System.Web;
using System.Web.Mvc;

namespace CalculatorLibWeb.Web.Attribute
{
    public class AuthorizeAttributeWithAnonTimeoutAttribute : AuthorizeAttribute
    {
        public int? AnonymousMinutesTimeout { get; set; }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            /*
             Then, use it as a global filter if it suits you, like this:

public static void RegisterGlobalFilters(GlobalFilterCollection filters)
{
    filters.Add(
        new AuthorizeAttributeWithAnonTimeoutAttribute
        {
            // By example, 10 minutes
            AnonymousMinutesTimeout = 10
        });
    // Your other filters
    ...
}
And do not forget to decorate your controllers or actions with AllowAnonymousAttribute:

[AllowAnonymous]
public class YourController
{
}
             */
            // Let default handling occurs
            base.OnAuthorization(filterContext);

            // If result is set, authorization has already been denied,
            // nothing more to do.
            if (filterContext.Result as HttpUnauthorizedResult != null)
                return;

            var isAnonAllowed = filterContext.ActionDescriptor.IsDefined(
                                    typeof(AllowAnonymousAttribute), true) ||
                                filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(
                                    typeof(AllowAnonymousAttribute), true);
            if (isAnonAllowed && AnonymousMinutesTimeout.HasValue &&
                // Not authorized
                !AuthorizeCore(filterContext.HttpContext))
            {
                const string visitStartCookieName = "visitStartCookie";
                const string visitStartDateFormat = "yyyyMMddhhmmss";
                var visitStartCookie = filterContext.HttpContext.Request
                    .Cookies[visitStartCookieName];
                var now = DateTime.UtcNow;
                DateTime visitStartDate=DateTime.UtcNow;
                var visitStartCookieValid = visitStartCookie != null &&
                                            DateTime.TryParseExact(visitStartCookie.Value,
                                                visitStartDateFormat, null, DateTimeStyles.AssumeUniversal,
                                                out visitStartDate);
                if (!visitStartCookieValid)
                {
                    visitStartDate = now;
                    filterContext.HttpContext.Response.Cookies.Add(
                        // Session cookie. (Need to set an expiry date if
                        // a "permanent" cookie is wished instead.)
                        new HttpCookie("CalculatorLibWeb.visitStartCookie")
                        {
                            //Name = "CalculatorLibWeb.visitStartCookie",
                            Value = now.ToString(visitStartDateFormat)
                        });
                }
                if (visitStartDate.AddMinutes(AnonymousMinutesTimeout.Value) < now)
                {
                    // Anonymous visit timed out
                    HandleUnauthorizedRequest(filterContext);
                    return;
                }
            }
        }
    }
}