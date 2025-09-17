using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class AuthorizeRoleAttribute : ActionFilterAttribute
{
    private readonly string[] _roles;
    public AuthorizeRoleAttribute(params string[] roles) => _roles = roles;

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var role = context.HttpContext.Session.GetString("UserRole");
        if (!_roles.Contains(role))
        {
            context.Result = new RedirectToActionResult("Index", "Dashboard", null);
        }
        base.OnActionExecuting(context);
    }
}
