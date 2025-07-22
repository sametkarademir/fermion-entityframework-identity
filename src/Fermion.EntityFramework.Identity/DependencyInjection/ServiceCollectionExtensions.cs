using System.Reflection;
using System.Security.Claims;
using Fermion.Domain.Shared.Conventions;
using Fermion.EntityFramework.Identity.Application.Services;
using Fermion.EntityFramework.Identity.Domain.Entities;
using Fermion.EntityFramework.Identity.Domain.Interfaces.Repositories;
using Fermion.EntityFramework.Identity.Domain.Interfaces.Services;
using Fermion.EntityFramework.Identity.Domain.Options;
using Fermion.EntityFramework.Identity.Infrastructure.Repositories;
using Fermion.EntityFramework.Identity.Presentation.Controllers;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using OpenIddict.Validation.AspNetCore;
using IdentityOptions = Fermion.EntityFramework.Identity.Domain.Options.IdentityOptions;

namespace Fermion.EntityFramework.Identity.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFermionIdentityServices<TContext>(
        this IServiceCollection services,
        Action<IdentityOptions> configureOptions)
        where TContext : DbContext
    {
        var options = new IdentityOptions();
        configureOptions.Invoke(options);
        services.Configure<IdentityOptions>(configureOptions.Invoke);

        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddHttpContextAccessor();

        services.AddScoped<IApplicationRoleClaimRepository, ApplicationRoleClaimRepository<TContext>>();
        services.AddScoped<IApplicationRoleRepository, ApplicationRoleRepository<TContext>>();
        services.AddScoped<IApplicationUserClaimRepository, ApplicationUserClaimRepository<TContext>>();
        services.AddScoped<IApplicationUserLoginRepository, ApplicationUserLoginRepository<TContext>>();
        services.AddScoped<IApplicationUserRepository, ApplicationUserRepository<TContext>>();
        services.AddScoped<IApplicationUserRoleRepository, ApplicationUserRoleRepository<TContext>>();
        services.AddScoped<IApplicationUserSessionRepository, ApplicationUserSessionRepository<TContext>>();
        services.AddScoped<IApplicationUserTokenRepository, ApplicationUserTokenRepository<TContext>>();

        services.AddScoped<IAccountAppService, AccountAppService>();
        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddScoped<IApplicationRoleAppService, ApplicationRoleAppService>();
        services.AddScoped<IApplicationUserAppService, ApplicationUserAppService>();
        services.AddScoped<IApplicationUserRoleAppService, ApplicationUserRoleAppService>();
        services.AddScoped<IApplicationUserSessionAppService, ApplicationUserSessionAppService>();

        services.AddIdentityCore<ApplicationUser>(opt =>
            {
                opt.Password.RequiredLength = 8;
                opt.Password.RequireDigit = true;
                opt.Password.RequireLowercase = true;
                opt.Password.RequireUppercase = true;
                opt.Password.RequireNonAlphanumeric = true;
                opt.SignIn.RequireConfirmedEmail = false;

                opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                opt.Lockout.MaxFailedAccessAttempts = 5;
                opt.Lockout.AllowedForNewUsers = true;

                opt.ClaimsIdentity.UserIdClaimType = ClaimTypes.NameIdentifier;
                opt.ClaimsIdentity.UserNameClaimType = ClaimTypes.Name;
                opt.ClaimsIdentity.EmailClaimType = ClaimTypes.Email;
                opt.ClaimsIdentity.RoleClaimType = ClaimTypes.Role;
            })
            .AddRoles<ApplicationRole>()
            .AddRoleManager<RoleManager<ApplicationRole>>()
            .AddEntityFrameworkStores<TContext>()
            .AddSignInManager<SignInManager<ApplicationUser>>()
            .AddUserManager<UserManager<ApplicationUser>>()
            .AddDefaultTokenProviders();

        services.AddOpenIddict()
            .AddCore(opt =>
            {
                opt.UseEntityFrameworkCore().UseDbContext<TContext>();
            })
            .AddServer(opt =>
            {
                opt.SetTokenEndpointUris("/connect/token");
                opt.AllowPasswordFlow().AllowRefreshTokenFlow();

                opt.RegisterScopes("api", "offline_access");
                opt.RegisterScopes(
                    OpenIddictConstants.Scopes.OpenId,
                    OpenIddictConstants.Scopes.Email,
                    OpenIddictConstants.Scopes.Profile,
                    OpenIddictConstants.Scopes.OfflineAccess,
                    OpenIddictConstants.Scopes.Roles
                );

                opt.AddDevelopmentEncryptionCertificate().AddDevelopmentSigningCertificate();
                opt.UseAspNetCore().EnableTokenEndpointPassthrough();

                opt.SetRefreshTokenLifetime(TimeSpan.FromDays(1));
                opt.SetAccessTokenLifetime(TimeSpan.FromMinutes(60));
            })
            .AddValidation(opt =>
            {
                opt.UseLocalServer();
                opt.UseAspNetCore();
                opt.UseSystemNetHttp();
            });

        services.AddAuthentication(opt =>
        {
            opt.DefaultAuthenticateScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
            opt.DefaultChallengeScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
        });

        if (options.ConnectController.Enabled)
        {
            services.AddControllers()
                .ConfigureApplicationPartManager(manager =>
                {
                    manager.ApplicationParts.Add(new AssemblyPart(typeof(ConnectController).Assembly));
                });
        }

        if (options.RoleController.Enabled)
        {
            services.AddControllers()
                .ConfigureApplicationPartManager(manager =>
                {
                    manager.ApplicationParts.Add(new AssemblyPart(typeof(ApplicationRoleController).Assembly));
                });

            services.PostConfigure<MvcOptions>(mvcOptions =>
            {
                mvcOptions.Conventions.Add(new ControllerAuthorizationConvention(
                    typeof(ApplicationRoleController),
                    options.RoleController.Route,
                    options.RoleController.GlobalAuthorization,
                    options.RoleController.Endpoints
                ));
            });
        }
        else
        {
            services.PostConfigure<MvcOptions>(mvcOptions =>
            {
                mvcOptions.Conventions.Add(new ControllerDisablingConvention(typeof(ApplicationRoleController)));
            });
            services.PostConfigure<MvcOptions>(mvcOptions =>
            {
                mvcOptions.Conventions.Add(new ControllerRemovalConvention(typeof(ApplicationRoleController)));
            });
        }

        if (options.UserController.Enabled)
        {
            services.AddControllers()
                .ConfigureApplicationPartManager(manager =>
                {
                    manager.ApplicationParts.Add(new AssemblyPart(typeof(ApplicationUserController).Assembly));
                });

            services.PostConfigure<MvcOptions>(mvcOptions =>
            {
                mvcOptions.Conventions.Add(new ControllerAuthorizationConvention(
                    typeof(ApplicationUserController),
                    options.UserController.Route,
                    options.UserController.GlobalAuthorization,
                    options.UserController.Endpoints
                ));
            });
        }
        else
        {
            services.PostConfigure<MvcOptions>(mvcOptions =>
            {
                mvcOptions.Conventions.Add(new ControllerDisablingConvention(typeof(ApplicationUserController)));
            });
            services.PostConfigure<MvcOptions>(mvcOptions =>
            {
                mvcOptions.Conventions.Add(new ControllerRemovalConvention(typeof(ApplicationUserController)));
            });
        }

        if (options.UserRoleController.Enabled)
        {
            services.AddControllers()
                .ConfigureApplicationPartManager(manager =>
                {
                    manager.ApplicationParts.Add(new AssemblyPart(typeof(ApplicationUserRoleController).Assembly));
                });

            services.PostConfigure<MvcOptions>(mvcOptions =>
            {
                mvcOptions.Conventions.Add(new ControllerAuthorizationConvention(
                    typeof(ApplicationUserRoleController),
                    options.UserRoleController.Route,
                    options.UserRoleController.GlobalAuthorization,
                    options.UserRoleController.Endpoints
                ));
            });
        }
        else
        {
            services.PostConfigure<MvcOptions>(mvcOptions =>
            {
                mvcOptions.Conventions.Add(new ControllerDisablingConvention(typeof(ApplicationUserRoleController)));
            });
            services.PostConfigure<MvcOptions>(mvcOptions =>
            {
                mvcOptions.Conventions.Add(new ControllerRemovalConvention(typeof(ApplicationUserRoleController)));
            });
        }

        if (options.UserSessionController.Enabled)
        {
            services.AddControllers()
                .ConfigureApplicationPartManager(manager =>
                {
                    manager.ApplicationParts.Add(new AssemblyPart(typeof(ApplicationUserSessionController).Assembly));
                });

            services.PostConfigure<MvcOptions>(mvcOptions =>
            {
                mvcOptions.Conventions.Add(new ControllerAuthorizationConvention(
                    typeof(ApplicationUserSessionController),
                    options.UserSessionController.Route,
                    options.UserSessionController.GlobalAuthorization,
                    options.UserSessionController.Endpoints
                ));
            });
        }
        else
        {
            services.PostConfigure<MvcOptions>(mvcOptions =>
            {
                mvcOptions.Conventions.Add(new ControllerDisablingConvention(typeof(ApplicationUserSessionController)));
            });
            services.PostConfigure<MvcOptions>(mvcOptions =>
            {
                mvcOptions.Conventions.Add(new ControllerRemovalConvention(typeof(ApplicationUserSessionController)));
            });
        }

        return services;
    }

    public static IServiceCollection AddFermionIdentitySeedService<TContext>(
        this IServiceCollection services,
        Action<IdentitySeedOptions> configureOptions)
        where TContext : DbContext
    {
        var options = new IdentitySeedOptions();
        configureOptions.Invoke(options);
        services.Configure<IdentitySeedOptions>(configureOptions.Invoke);

        services.AddScoped<IIdentitySeedService<TContext>, IdentitySeedService<TContext>>();
        services.AddHostedService<AppSeedInitializer<TContext>>();

        return services;
    }
}