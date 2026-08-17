using System.Globalization;

namespace Oscar.Blazor.Library.Services
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;

    public class SessionTimeoutMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly TimeSpan _sessionTimeout;
        private readonly PathString _redirectPath;

        public SessionTimeoutMiddleware(RequestDelegate next, TimeSpan sessionTimeout, PathString redirectPath)
        {
            _next = next;
            _sessionTimeout = sessionTimeout;
            _redirectPath = redirectPath;
        }

        public async Task Invoke(HttpContext context)
        {
            var lastActivity = context.Session.GetString("LastActivity");
            if (!string.IsNullOrEmpty(lastActivity))
            {
                var lastActivityTime = DateTime.Parse(lastActivity, CultureInfo.InvariantCulture);
                if (DateTime.Now - lastActivityTime > _sessionTimeout)
                {
                    // Session has expired, redirect user
                    await context.Response.WriteAsync("Your session has been expired");
                    //context.Response.Redirect(_redirectPath);
                    return;
                }
            }

            // Update last activity time
            context.Session.SetString("LastActivity", DateTime.Now.ToString(CultureInfo.InvariantCulture));

            await _next(context);
        }
    }

    public static class SessionTimeoutMiddlewareExtensions
    {
        public static IApplicationBuilder UseSessionTimeoutMiddleware(this IApplicationBuilder builder, TimeSpan sessionTimeout, string redirectPath)
        {
            return builder.UseMiddleware<SessionTimeoutMiddleware>(sessionTimeout, new PathString(redirectPath));
        }
    }

}
