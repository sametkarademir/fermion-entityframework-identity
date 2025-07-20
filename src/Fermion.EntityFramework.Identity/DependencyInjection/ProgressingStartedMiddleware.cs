using Fermion.Domain.Extensions.Claims;
using Fermion.Domain.Extensions.HttpContexts;
using Microsoft.AspNetCore.Http;

namespace Fermion.EntityFramework.Identity.DependencyInjection;

public class ProgressingStartedMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            if (context.User.Identity is { IsAuthenticated: true })
            {
                var sessionId = context.User.GetUserSessionId();
                if (sessionId.HasValue && sessionId.Value != Guid.Empty)
                {
                    context.SetSessionId(sessionId.Value);
                }
            }
        }
        catch (Exception)
        {
            // ignored
        }

        await next(context);
    }
}