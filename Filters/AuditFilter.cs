using Microsoft.AspNetCore.Mvc.Filters;
using st_xaviers_website_api.Models;

namespace st_xaviers_website_api.Filters
{
    public class AuditFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            // Token se RoleId, na mile (bina login) to 0
            var claim = context.HttpContext.User.FindFirst("RoleId")?.Value;
            int roleId = int.TryParse(claim, out var r) ? r : 0;

            // Jis bhi API ke parameter mein BaseModel wala model ho, usme bhar do
            foreach (var arg in context.ActionArguments.Values)
            {
                if (arg is BaseModel m)
                {
                    m.AddedBy = roleId;
                    m.UpdatedBy = roleId;
                }
            }
        }

        public void OnActionExecuted(ActionExecutedContext context) { }
    }
}