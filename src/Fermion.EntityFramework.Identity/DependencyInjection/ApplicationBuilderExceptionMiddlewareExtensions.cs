using Microsoft.AspNetCore.Builder;

namespace Fermion.EntityFramework.Identity.DependencyInjection;

public static class ApplicationBuilderExceptionMiddlewareExtensions
{
    public static void FermionIdentityMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<ProgressingStartedMiddleware>();
    }
}