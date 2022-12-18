using Api.Exceptions;

namespace Api.Middlewares
{
    public class ErrorMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (NotFoundException ex)
            {
                context.Response.StatusCode = 404;
                await context.Response.WriteAsJsonAsync(ex.Message);
                //await context.Response.CompleteAsync()
            }

            catch (IsExistException ex)
            {
                context.Response.StatusCode = 409;
                await context.Response.WriteAsJsonAsync(ex.Message);
                //await context.Response.CompleteAsync()
            }

            catch (AuthorizationException ex)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsJsonAsync(ex.Message);
               
            }

            catch (PasswordException ex)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsJsonAsync(ex.Message);

            }


        }
    }
    public static class ErrorMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalErrorWrapper(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorMiddleware>();
        }
    }
}
