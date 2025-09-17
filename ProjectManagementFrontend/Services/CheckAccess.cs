using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ProjectManagementFrontend.Services
{
    /// <summary>
    /// Custom authorization filter to check if a user is authenticated based on JWT token stored in session.
    /// If the token is missing, it redirects the user to the login page.
    /// </summary>
    public class CheckAccess : ActionFilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            // If JWT token is not found in session, redirect to login page
            if (filterContext.HttpContext.Session.GetString("JWTToken") == null)
            {
                filterContext.Result = new RedirectResult("~/User/Login");
            }
        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            // If JWT token is not found, add headers to prevent browser from caching the previous page
            if (context.HttpContext.Session.GetString("JWTToken") == null)
            {
                context.HttpContext.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
                context.HttpContext.Response.Headers["Expires"] = "-1";
                context.HttpContext.Response.Headers["Pragma"] = "no-cache";
            }

            base.OnResultExecuting(context);
        }
    }
}
