using Microsoft.AspNetCore.Mvc.Filters;

namespace InnoGotchi.API.Filters.ActionFilters
{
    public class ExtractUserIdFilterAttribute : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var claimValue = context.HttpContext.User.FindFirst("Id").Value;
            var id = Guid.Parse(claimValue);

            context.ActionArguments["id"] = id;

            await next();
        }
    }
}
