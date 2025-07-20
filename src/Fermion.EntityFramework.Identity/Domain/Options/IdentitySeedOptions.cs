namespace Fermion.EntityFramework.Identity.Domain.Options;

public class IdentitySeedOptions
{
    public bool Enabled { get; set; } = true;

    public DefaultUserOptions DefaultAdminUser { get; set; } = new();
    public List<string> DefaultRoles { get; set; } = new() { "Admin", "User" };

    public OpenIddictOptions OpenIddictClient { get; set; } = new();
}

public class DefaultUserOptions
{
    public string UserName { get; set; } = "admin";
    public string Email { get; set; } = "admin@example.com";
    public string Password { get; set; } = "1q2w3E*";
}

public class OpenIddictOptions
{
    public string ClientId { get; set; } = "K7vQm9pX4sR8wN2jF6yU3zV5cB1nH9gL0oA8mE7iT4qD";
    public string ClientSecret { get; set; } = "fermion-secret";
    public string DisplayName { get; set; } = "Fermion Identity Client";
}