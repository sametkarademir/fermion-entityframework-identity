using Fermion.Domain.Extensions.Validations;
using FluentValidation;

namespace Fermion.EntityFramework.Identity.Application.DTOs.ApplicationUsers;

public class ChangePasswordApplicationUserRequestDto
{
    public string Password { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
    public string ConfirmPassword { get; set; } = null!;
}

public class ChangePasswordApplicationUserRequestValidator : AbstractValidator<ChangePasswordApplicationUserRequestDto>
{
    public ChangePasswordApplicationUserRequestValidator()
    {
        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .MaximumLength(128);

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .MinimumLength(8)
            .MaximumLength(128)
            .Must(PasswordValidationExtensions.ContainUppercase)
            .WithMessage("New Password must contain at least one uppercase letter.")
            .Must(PasswordValidationExtensions.ContainLowercase)
            .WithMessage("New Password must contain at least one lowercase letter.")
            .Must(PasswordValidationExtensions.ContainDigit).WithMessage("New Password must contain at least one digit.")
            .Must(PasswordValidationExtensions.ContainSpecialCharacter)
            .WithMessage("New Password must contain at least one special character (!@#$%^&*()_+-=[]{}|;:,.<>?).")
            .Must(PasswordValidationExtensions.NotBeCommonPassword)
            .WithMessage("This new password is too common. Please choose a more secure password.")
            .Must(PasswordValidationExtensions.NotContainSequentialCharacters)
            .WithMessage("New Password must not contain sequential characters (e.g., 123, abc).")
            .Must(PasswordValidationExtensions.NotContainRepeatingCharacters)
            .WithMessage("New Password must not contain the same character 3 times in a row.");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty()
            .Equal(x => x.NewPassword).WithMessage("Passwords do not match.");
    }
}