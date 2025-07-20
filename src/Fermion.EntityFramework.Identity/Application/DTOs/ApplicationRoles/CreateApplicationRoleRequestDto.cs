using FluentValidation;

namespace Fermion.EntityFramework.Identity.Application.DTOs.ApplicationRoles;

public class CreateApplicationRoleRequestDto
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}

public class CreateApplicationRoleRequestValidator : AbstractValidator<CreateApplicationRoleRequestDto>
{
    public CreateApplicationRoleRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(256);

        RuleFor(x => x.Description)
            .MaximumLength(1024);
    }
}