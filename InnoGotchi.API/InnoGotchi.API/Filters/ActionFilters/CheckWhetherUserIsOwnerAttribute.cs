using InnoGotchi.Application.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace InnoGotchi.API.Filters.ActionFilters
{
    public class CheckWhetherUserIsOwnerAttribute : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var claimValue = context.HttpContext.User.FindFirst("Id").Value;
            var routeId = context.RouteData.Values["id"] as string;
            if(claimValue != routeId)
            {
                throw new AccessDeniedException("You cannot see other user profiles");
            }

            await next();
        }
    }
}
