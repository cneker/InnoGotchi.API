using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace InnoGotchi.API.Filters.ActionFilters
{
    public class ValidationFilterAttribute : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var action = context.RouteData.Values["action"];
            var controller = context.RouteData.Values["controller"];

            var param = context.ActionArguments
                .SingleOrDefault(p => p.Value.ToString().Contains("Dto")).Value;

            if (param == null)
            {
                context.Result =
                    new BadRequestObjectResult($"Object is null. Controller: {controller}, action: {action}");
            }

            if (!context.ModelState.IsValid)
            {
                context.Result =
                    new UnprocessableEntityObjectResult(context.ModelState);
            }
            else
            {
                await next();
            }
        }
    }
}
