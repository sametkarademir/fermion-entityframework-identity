using Fermion.EntityFramework.Identity.Domain.Interfaces.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Fermion.EntityFramework.Identity.DependencyInjection;

public class AppSeedInitializer<TContext>(IServiceProvider serviceProvider) : IHostedService 
    where TContext : DbContext
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var identitySeedService = scope.ServiceProvider.GetRequiredService<IIdentitySeedService<TContext>>();
        await identitySeedService.SeedAsync(cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        // No specific stop actions needed for the seed initializer
        await Task.CompletedTask;
    }
}