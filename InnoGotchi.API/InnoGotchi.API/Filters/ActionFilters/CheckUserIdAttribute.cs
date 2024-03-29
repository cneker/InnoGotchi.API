﻿using InnoGotchi.Application.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace InnoGotchi.API.Filters.ActionFilters
{
    public class CheckUserIdAttribute : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var claimValue = context.HttpContext.User.FindFirst("Id").Value;
            var routeId = context.RouteData.Values["userId"] as string;
            if (claimValue != routeId)
            {
                throw new AccessDeniedException("You are not the owner of this account");
            }

            await next();
        }
    }
}
