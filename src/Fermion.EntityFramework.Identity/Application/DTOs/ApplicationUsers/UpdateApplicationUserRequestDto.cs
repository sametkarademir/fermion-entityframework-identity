using FluentValidation;

namespace Fermion.EntityFramework.Identity.Application.DTOs.ApplicationUsers;

public class UpdateApplicationUserRequestDto
{
    public string? PhoneNumber { get; set; }

    public bool EmailConfirmed { get; set; }
    public bool PhoneNumberConfirmed { get; set; }
    public bool TwoFactorEnabled { get; set; }

    public DateTimeOffset? LockoutEnd { get; set; }
    public bool LockoutEnabled { get; set; } = true;

    public string ConcurrencyStamp { get; set; } = null!;
    public List<string> Roles { get; set; } = [];
}

public class UpdateApplicationUserRequestValidator : AbstractValidator<UpdateApplicationUserRequestDto>
{
    public UpdateApplicationUserRequestValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .Matches(@"^\+?[1-9]\d{1,14}$")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber))
            .WithMessage("Phone number must be in a valid international format if provided.");
    }
}