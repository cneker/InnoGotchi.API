using InnoGotchi.Application.Exceptions;
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

        public async Task InvokeAsync(HttpContext context)
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
                    PetIsDeadException => StatusCodes.Status400BadRequest,
                    _ => StatusCodes.Status500InternalServerError
                };
                var message = JsonSerializer.Serialize(new { message = ex.Message });

                await response.WriteAsync(message);
            }
        }
    }
}
