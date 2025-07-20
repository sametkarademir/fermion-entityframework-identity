using Fermion.Domain.Extensions.Validations;
using FluentValidation;

namespace Fermion.EntityFramework.Identity.Application.DTOs.ApplicationUsers;

public class CreateApplicationUserRequestDto
{
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? PhoneNumber { get; set; }

    public string Password { get; set; } = null!;
    public string ConfirmPassword { get; set; } = null!;

    public bool EmailConfirmed { get; set; }
    public bool PhoneNumberConfirmed { get; set; }
    public bool TwoFactorEnabled { get; set; }

    public DateTimeOffset? LockoutEnd { get; set; }
    public bool LockoutEnabled { get; set; } = true;

    public List<string> Roles { get; set; } = [];
}

public class CreateApplicationUserRequestValidator : AbstractValidator<CreateApplicationUserRequestDto>
{
    public CreateApplicationUserRequestValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty()
            .MaximumLength(256);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256);

        RuleFor(x => x.PhoneNumber)
            .Matches(@"^\+?[1-9]\d{1,14}$")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber))
            .WithMessage("Phone number must be in a valid international format if provided.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .MaximumLength(128)
            .Must(PasswordValidationExtensions.ContainUppercase).WithMessage("Password must contain at least one uppercase letter.")
            .Must(PasswordValidationExtensions.ContainLowercase).WithMessage("Password must contain at least one lowercase letter.")
            .Must(PasswordValidationExtensions.ContainDigit).WithMessage("Password must contain at least one digit.")
            .Must(PasswordValidationExtensions.ContainSpecialCharacter).WithMessage("Password must contain at least one special character (!@#$%^&*()_+-=[]{}|;:,.<>?).")
            .Must(PasswordValidationExtensions.NotBeCommonPassword).WithMessage("This password is too common. Please choose a more secure password.")
            .Must(PasswordValidationExtensions.NotContainSequentialCharacters).WithMessage("Password must not contain sequential characters (e.g., 123, abc).")
            .Must(PasswordValidationExtensions.NotContainRepeatingCharacters).WithMessage("Password must not contain the same character 3 times in a row.")
            .Must((model, password) => !PasswordValidationExtensions.ContainUsername(password, model.UserName)).WithMessage("Password must not contain your username.")
            .Must((model, password) => !PasswordValidationExtensions.ContainEmailParts(password, model.Email)).WithMessage("Password must not contain parts of your email address.");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty()
            .Equal(x => x.Password).WithMessage("Passwords do not match.");
    }
}