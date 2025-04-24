using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Urb.Domain.Urb.Models;


namespace Urb.Plan.v2.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class UserAuthorizeAttribute: Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var anonym = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
            if(anonym == true)
            {
                return;
            }

            User user = (User)context.HttpContext.Items.OfType<User>();

            if(user == null)
            {
                context.Result = new JsonResult((new { message = "Unauthorized" })) 
                { 
                    StatusCode = StatusCodes.Status401Unauthorized 
                };
            }
            
        }
    }
}
