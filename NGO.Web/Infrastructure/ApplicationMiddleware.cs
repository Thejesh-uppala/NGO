using System.Net;
using System.Text.Json;

namespace NGO.Web.Infrastructure
{
    public static class ApplicationMiddlewareExtensions
    {
        public static IApplicationBuilder UseApplicationMiddleware(this IApplicationBuilder applicationBuilder)
        {
            return applicationBuilder.UseMiddleware<ApplicationMiddleware>();
        }
    }

    public class ApplicationMiddleware
    {

        private readonly RequestDelegate _next;
        //private readonly ILogger<ApplicationMiddleware> _logger;

        public ApplicationMiddleware(RequestDelegate next)
        {
            _next = next;
            //_logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unhandled exception occurred.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = exception switch
            {
                KeyNotFoundException => (int)HttpStatusCode.NotFound,
                UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
                _ => (int)HttpStatusCode.InternalServerError,
            };

            var response = new
            {
                StatusCode = context.Response.StatusCode,
                Message = exception.Message,
                Timestamp = DateTime.UtcNow,
                Path = context.Request.Path,
                TraceId = context.TraceIdentifier // Useful for correlating logs
            };

            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }



    //public async Task Invoke(HttpContext context, ApplicationContext applicationContext)
    //        {
    //            if (context.User.Identity.IsAuthenticated)
    //            {
    //                if (int.TryParse(context.User.Claims.Single(x => x.Type == ClaimTypes.NameIdentifier).Value, out int userId))
    //                {
    //                    applicationContext.UserId = userId;
    //                }

    //                //if (context.User.Claims.Any(x => x.Type == Constants.DivisionId))
    //                //{
    //                //    applicationContext.DivisionId = Convert.ToInt32(context.User.Claims.First(x => x.Type == Constants.DivisionId).Value);
    //                //}

    //                //TODO: Populate tenantId from request header
    //                //if (context.User.Claims.Any(x => x.Type == Constants.TenantId))
    //                //{
    //                //    applicationContext.TenantId = Convert.ToInt32(context.User.Claims.First(x => x.Type == Constants.TenantId).Value);
    //                //}

    //                applicationContext.UserEmail = context.User.Claims.SingleOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
    //            }

    //            await this.next.Invoke(context);
    //        }
    //

}
