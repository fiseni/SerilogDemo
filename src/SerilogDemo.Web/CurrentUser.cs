using Serilog.Context;
using System.Security.Claims;

namespace SerilogDemo.Web;

// Refer to this article for CurrentUser implementation.
// https://fiseni.com/posts/current-user-aspnetcore/
public class CurrentUser : ICurrentUser, ICurrentUserInitializer
{
    public string? UserId { get; set; }
    public string? UserName { get; set; }
}

public interface ICurrentUser
{
    string? UserId { get; }
    string? UserName { get; }
}

public interface ICurrentUserInitializer
{
    string? UserId { get; set; }
    string? UserName { get; set; }
}

public static class CurrentUserExtensions
{
    public static IServiceCollection AddCurrentUser(this IServiceCollection services)
    {
        services.AddScoped<CurrentUser>();
        services.AddScoped<ICurrentUser, CurrentUser>(x => x.GetRequiredService<CurrentUser>());
        services.AddScoped<ICurrentUserInitializer, CurrentUser>(x => x.GetRequiredService<CurrentUser>());

        return services;
    }

    public static IApplicationBuilder UseCurrentUserFromToken(this IApplicationBuilder app, bool pushToSerilogContext = true)
    {
        app.Use(async (context, next) =>
        {
            var currentUser = context.RequestServices.GetService<ICurrentUserInitializer>();

            if (currentUser is not null)
            {
                //currentUser.UserId = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                //currentUser.UserName = context.User?.FindFirst(ClaimTypes.Name)?.Value;

                // For the demo we'll just set some values.
                currentUser.UserId = "1";
                currentUser.UserName = "admin";
            }

            if (pushToSerilogContext)
            {
                using (LogContext.PushProperty(nameof(ICurrentUserInitializer.UserId), currentUser?.UserId))
                using (LogContext.PushProperty(nameof(ICurrentUserInitializer.UserName), currentUser?.UserName))
                {
                    await next();
                }
            }
            else
            {
                await next();
            }

        });

        return app;
    }
}
