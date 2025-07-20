using Fermion.EntityFramework.Identity.Domain.Entities;
using Fermion.EntityFramework.Identity.Domain.Interfaces.Services;
using Fermion.EntityFramework.Identity.Domain.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;

namespace Fermion.EntityFramework.Identity.Application.Services;

public class IdentitySeedService<TContext> : IIdentitySeedService<TContext>
    where TContext : DbContext
{
    private readonly TContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IOpenIddictApplicationManager _manager;
    private readonly IOptions<IdentitySeedOptions> _options;
    private readonly ILogger<IdentitySeedService<TContext>> _logger;

    public IdentitySeedService(
        TContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IOpenIddictApplicationManager manager,
        IOptions<IdentitySeedOptions> options,
        ILogger<IdentitySeedService<TContext>> logger)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _manager = manager;
        _options = options;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (!_options.Value.Enabled)
        {
            _logger.LogInformation("Identity seed service is disabled");
            return;
        }

        try
        {
            _logger.LogInformation("Starting identity seed process");
            
            await CreateDefaultRolesAsync(cancellationToken);
            await CreateDefaultAdminUserAsync(cancellationToken);
            await CreateOpenIddictClientAsync(cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Identity seed process completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during identity seeding");
            throw;
        }
    }

    private async Task CreateDefaultRolesAsync(CancellationToken cancellationToken)
    {
        foreach (var roleName in _options.Value.DefaultRoles)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                var role = new ApplicationRole { Name = roleName };
                var result = await _roleManager.CreateAsync(role);
                
                if (result.Succeeded)
                {
                    _logger.LogInformation("Created role: {RoleName}", roleName);
                }
                else
                {
                    _logger.LogError("Failed to create role: {RoleName}. Errors: {Errors}", 
                        roleName, string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
            else
            {
                _logger.LogInformation("Role already exists: {RoleName}", roleName);
            }
        }
    }

    private async Task CreateDefaultAdminUserAsync(CancellationToken cancellationToken)
    {
        var adminUser = _options.Value.DefaultAdminUser;
        
        var existingUser = await _userManager.FindByNameAsync(adminUser.UserName);
        if (existingUser != null)
        {
            _logger.LogInformation("Admin user already exists: {UserName}", adminUser.UserName);
            return;
        }

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = adminUser.UserName,
            Email = adminUser.Email,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, adminUser.Password);
        
        if (result.Succeeded)
        {
            _logger.LogInformation("Created admin user: {UserName}", adminUser.UserName);
            foreach (var roleName in _options.Value.DefaultRoles)
            {
                if (await _roleManager.RoleExistsAsync(roleName))
                {
                    await _userManager.AddToRoleAsync(user, roleName);
                    _logger.LogInformation("Assigned role {RoleName} to admin user {UserName}", roleName, adminUser.UserName);
                }
                else
                {
                    _logger.LogWarning("Role {RoleName} does not exist, skipping assignment to admin user {UserName}", 
                        roleName, adminUser.UserName);
                }
            }
        }
        else
        {
            _logger.LogError("Failed to create admin user: {UserName}. Errors: {Errors}", 
                adminUser.UserName, string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }
    
    private async Task CreateOpenIddictClientAsync(CancellationToken cancellationToken)
    {
        if (await _manager.FindByClientIdAsync(_options.Value.OpenIddictClient.ClientId, cancellationToken) != null)
        {
            return;
        }

        await _manager.CreateAsync(new OpenIddictApplicationDescriptor
        {
            ClientId = _options.Value.OpenIddictClient.ClientId,
            ClientSecret = _options.Value.OpenIddictClient.ClientSecret,
            DisplayName = _options.Value.OpenIddictClient.DisplayName,
            Permissions =
            {
                OpenIddictConstants.Permissions.Endpoints.Token,
                OpenIddictConstants.Permissions.GrantTypes.Password,
                OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
                OpenIddictConstants.Permissions.Scopes.Profile,
                OpenIddictConstants.Permissions.Scopes.Email,
                OpenIddictConstants.Permissions.Scopes.Roles,
                OpenIddictConstants.Permissions.Prefixes.Scope + "api"
            }
        }, cancellationToken: cancellationToken);
    }
} 