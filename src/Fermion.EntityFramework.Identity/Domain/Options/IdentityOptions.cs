using Fermion.Domain.Shared.Conventions;

namespace Fermion.EntityFramework.Identity.Domain.Options;

public class IdentityOptions
{
    public bool Enabled { get; set; } = true;
    
    public ConnectControllerOptions ConnectController { get; set; } = new();
    public RoleControllerOptions RoleController { get; set; } = new();
    public UserControllerOptions UserController { get; set; } = new();
    public UserRoleControllerOptions UserRoleController { get; set; } = new();
    public UserSessionControllerOptions UserSessionController { get; set; } = new();
}

public class ConnectControllerOptions
{
    public bool Enabled { get; set; } = true;
}

public class RoleControllerOptions
{
    public bool Enabled { get; set; } = true;
    public string Route { get; set; } = "api/roles";
    
    public AuthorizationOptions GlobalAuthorization { get; set; } = new()
    {
        RequireAuthentication = true,
        Policy = null,
        Roles = null
    };
    
    public List<EndpointOptions>? Endpoints { get; set; }
}

public class UserControllerOptions
{
    public bool Enabled { get; set; } = true;
    public string Route { get; set; } = "api/users";
    
    public AuthorizationOptions GlobalAuthorization { get; set; } = new()
    {
        RequireAuthentication = true,
        Policy = null,
        Roles = null
    };
    
    public List<EndpointOptions>? Endpoints { get; set; }
}

public class UserRoleControllerOptions
{
    public bool Enabled { get; set; } = true;
    public string Route { get; set; } = "api/user-roles";
    
    public AuthorizationOptions GlobalAuthorization { get; set; } = new()
    {
        RequireAuthentication = true,
        Policy = null,
        Roles = null
    };
    
    public List<EndpointOptions>? Endpoints { get; set; }
}

public class UserSessionControllerOptions
{
    public bool Enabled { get; set; } = true;
    public string Route { get; set; } = "api/user-sessions";
    
    public AuthorizationOptions GlobalAuthorization { get; set; } = new()
    {
        RequireAuthentication = true,
        Policy = null,
        Roles = null
    };
    
    public List<EndpointOptions>? Endpoints { get; set; }
}