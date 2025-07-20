# Fermion.EntityFramework.Identity

A comprehensive ASP.NET Core Identity library built on Entity Framework Core with OpenIddict integration, providing a complete authentication and authorization solution for .NET applications.

## Overview

Fermion.EntityFramework.Identity is a modular and configurable identity management library that extends ASP.NET Core Identity with additional features including:

- **User Management**: Complete CRUD operations for application users
- **Role Management**: Role-based access control with custom roles
- **User-Role Management**: Assign and manage user roles
- **User Session Management**: Track and manage user sessions
- **OpenIddict Integration**: OAuth 2.0 and OpenID Connect support
- **JWT Token Authentication**: Secure token-based authentication
- **Database Seeding**: Automatic initialization with default users and roles
- **Configurable Controllers**: Enable/disable and customize API endpoints
- **Flexible Authorization**: Granular control over endpoint security

## Features

### 🔐 Authentication & Authorization
- JWT token-based authentication
- OAuth 2.0 and OpenID Connect support via OpenIddict
- Role-based authorization
- Custom authorization policies
- Session management

### 👥 User Management
- User registration and profile management
- Password policies and validation
- Account lockout protection
- Email confirmation support
- User claims management

### 🏷️ Role Management
- Custom role creation and management
- Role-based permissions
- Role claims support
- Hierarchical role structure

### 🔧 Configuration
- Highly configurable through options pattern
- Per-controller enable/disable functionality
- Customizable API routes
- Flexible authorization settings
- Database seeding options

## Installation

Add the Fermion.EntityFramework.Identity package to your project:

```bash
  dotnet add package Fermion.EntityFramework.Identity
```

## Quick Start

### 1. Create Your DbContext

Create a DbContext that inherits from `IdentityUserDbContext`:

```csharp
using Fermion.EntityFramework.Identity.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace YourApp.Data;

public class ApplicationDbContext : IdentityUserDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Additional model configurations can be added here
    }
}
```

### 2. Configure Services

In your `Program.cs` or `Startup.cs`:

```csharp
using Fermion.EntityFramework.Identity;
using Fermion.EntityFramework.Identity.DependencyInjection;
using Fermion.EntityFramework.Identity.Domain.Options;

var builder = WebApplication.CreateBuilder(args);

// Add your DbContext
var connectionString = builder.Configuration["ConnectionStrings:DefaultConnection"];
builder.Services.AddDbContextFactory<ApplicationDbContext>(opt =>
{
    opt.UseNpgsql(connectionString); // or UseSqlServer, UseSqlite, etc.
    opt.UseEntityMetadataTracking(); // Optional: Enable metadata tracking
    opt.UseOpenIddict(); // Required for OpenIddict integration
}, ServiceLifetime.Scoped);

// Add Fermion Identity Services
builder.Services.AddFermionIdentityServices<ApplicationDbContext>(options =>
{
    options.Enabled = true;
    
    // Configure controllers
    options.ConnectController.Enabled = true;
    
    options.RoleController.Enabled = true;
    options.RoleController.Route = "api/roles";
    options.RoleController.GlobalAuthorization.RequireAuthentication = false;
    
    options.UserController.Enabled = true;
    options.UserController.Route = "api/users";
    options.UserController.GlobalAuthorization.RequireAuthentication = false;
    
    options.UserRoleController.Enabled = true;
    options.UserRoleController.Route = "api/user-roles";
    options.UserRoleController.GlobalAuthorization.RequireAuthentication = false;
    
    options.UserSessionController.Enabled = true;
    options.UserSessionController.Route = "api/user-sessions";
    options.UserSessionController.GlobalAuthorization.RequireAuthentication = false;
});

// Add Identity Seed Service (optional)
builder.Services.AddFermionIdentitySeedService<ApplicationDbContext>(options =>
{
    options.Enabled = true;
    options.DefaultAdminUser.UserName = "admin";
    options.DefaultAdminUser.Email = "admin@example.com";
    options.DefaultAdminUser.Password = "Admin123!";
    options.DefaultRoles = new List<string> { "Admin", "User", "Manager" };
    options.OpenIddictClient.ClientId = "K7vQm9pX4sR8wN2jF6yU3zV5cB1nH9gL0oA8mE7iT4qD";
    options.OpenIddictClient.ClientSecret = "fermion-secret";
    options.OpenIddictClient.DisplayName = "Fermion Identity Client";
});

var app = builder.Build();

// Configure middleware
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Add Fermion Identity middleware
// Requires OpenIddict to be configured
app.FermionIdentityMiddleware();

app.Run();
```

## API Endpoints

The library provides the following REST API endpoints:

### Authentication
- `POST /connect/token` - Get access token
- `POST /connect/token` - Refresh token

### User Management
- `GET /api/users` - Get users list
- `GET /api/users/{id}` - Get user by ID
- `POST /api/users` - Create new user
- `PUT /api/users/{id}` - Update user
- `DELETE /api/users/{id}` - Delete user
- `POST /api/users/change-password` - Change user password

### Role Management
- `GET /api/roles` - Get roles list
- `GET /api/roles/{id}` - Get role by ID
- `POST /api/roles` - Create new role
- `PUT /api/roles/{id}` - Update role
- `DELETE /api/roles/{id}` - Delete role

### User-Role Management
- `GET /api/user-roles` - Get user roles
- `POST /api/user-roles` - Assign role to user
- `DELETE /api/user-roles` - Remove role from user

### Session Management
- `GET /api/user-sessions` - Get user sessions
- `DELETE /api/user-sessions/{id}` - Terminate session

## Configuration Options

### IdentityOptions

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Enabled` | bool | true | Enable/disable the entire identity system |
| `ConnectController` | ConnectControllerOptions | - | OAuth/OpenID Connect controller settings |
| `RoleController` | RoleControllerOptions | - | Role management controller settings |
| `UserController` | UserControllerOptions | - | User management controller settings |
| `UserRoleController` | UserRoleControllerOptions | - | User-role management controller settings |
| `UserSessionController` | UserSessionControllerOptions | - | Session management controller settings |

### Controller Options

Each controller has the following configuration options:

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Enabled` | bool | true | Enable/disable the controller |
| `Route` | string | varies | Custom route prefix for the controller |
| `GlobalAuthorization` | AuthorizationOptions | - | Global authorization settings |

### AuthorizationOptions

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `RequireAuthentication` | bool | true | Require authentication for all endpoints |
| `Policy` | string | null | Custom authorization policy name |
| `Roles` | string | null | Required roles (comma-separated) |

### IdentitySeedOptions

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Enabled` | bool | true | Enable/disable database seeding |
| `DefaultAdminUser` | DefaultUserOptions | - | Default admin user configuration |
| `DefaultRoles` | List<string> | ["Admin", "User"] | Default roles to create |

### DefaultUserOptions

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `UserName` | string | "admin" | Default admin username |
| `Email` | string | "admin@example.com" | Default admin email |
| `Password` | string | "1q2w3E*" | Default admin password |

## Database Entities

The library includes the following entities:

- `ApplicationUser` - User entity with extended properties
- `ApplicationRole` - Role entity
- `ApplicationUserRole` - User-role relationship
- `ApplicationUserClaim` - User claims
- `ApplicationRoleClaim` - Role claims
- `ApplicationUserLogin` - External login providers
- `ApplicationUserToken` - User tokens
- `ApplicationUserSession` - User sessions

## Dependencies

- **.NET 8.0** or later
- **Entity Framework Core 9.0.3**
- **ASP.NET Core Identity**
- **OpenIddict 6.4.0**
- **AutoMapper 12.0.1**
- **FluentValidation 11.11.0**

## Security Features

- **Password Policies**: Configurable password requirements
- **Account Lockout**: Protection against brute force attacks
- **JWT Tokens**: Secure token-based authentication
- **OAuth 2.0**: Industry-standard authorization protocol
- **Role-based Access Control**: Granular permission management
- **Session Management**: Track and control user sessions

## Migration and Database Setup

1. Create your initial migration:
```bash
dotnet ef migrations add InitialIdentity
```

2. Update the database:
```bash
dotnet ef database update
```

The seed service will automatically create default users and roles on first run.

## Examples

### Custom Authorization

```csharp
builder.Services.AddFermionIdentityServices<ApplicationDbContext>(options =>
{
    options.RoleController.GlobalAuthorization = new AuthorizationOptions
    {
        RequireAuthentication = true,
        Policy = "AdminPolicy",
        Roles = "Admin,Manager"
    };
});
```

### Custom Routes

```csharp
builder.Services.AddFermionIdentityServices<ApplicationDbContext>(options =>
{
    options.UserController.Route = "api/v1/identity/users";
    options.RoleController.Route = "api/v1/identity/roles";
});
```

### Disable Controllers

```csharp
builder.Services.AddFermionIdentityServices<ApplicationDbContext>(options =>
{
    options.UserSessionController.Enabled = false;
    options.UserRoleController.Enabled = false;
});
```