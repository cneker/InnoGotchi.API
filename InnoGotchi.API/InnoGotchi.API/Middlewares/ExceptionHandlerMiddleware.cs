﻿using InnoGotchi.Application.Exceptions;
using System.Text.Json;

namespace InnoGotchi.API.Middlewares
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        public ExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ILogger<ExceptionHandlerMiddleware> logger)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var response = context.Response;
                response.ContentType = "application/json";

                response.StatusCode = ex switch
                {
                    NotFoundException => StatusCodes.Status404NotFound,
                    AlreadyExistsException => StatusCodes.Status400BadRequest,
                    PetIsDeadException => StatusCodes.Status423Locked,
                    IncorrectRequestException => StatusCodes.Status400BadRequest,
                    AccessDeniedException => StatusCodes.Status403Forbidden,
                    _ => StatusCodes.Status500InternalServerError
                };
                var message = JsonSerializer.Serialize(new { message = ex.Message });
                logger.LogError(message);
                await response.WriteAsync(message);
            }
        }
    }
}
