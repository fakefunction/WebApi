using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CalculatorLibWeb.Web.Attribute
{

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class CalculatorLibWebAuthorizeAttribute : AuthorizeAttribute
    {
        public enum Role
        {
            Administrator = 1,
            UserWithPrivileges = 2,
            User = 3,
        }

        public CalculatorLibWebAuthorizeAttribute(params object[] roles)
        {
            if (roles.Any(r => r.GetType().BaseType != typeof(Enum)))
                throw new ArgumentException("roles");

            this.Roles = string.Join(",", roles.Select(r => Enum.GetName(r.GetType(), r).ToLower()));
        }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            //call to see if logged in
            return true;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new HttpUnauthorizedResult();
        }
    }
    
}