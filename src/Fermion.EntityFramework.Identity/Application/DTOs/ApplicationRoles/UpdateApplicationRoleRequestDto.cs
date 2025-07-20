using FluentValidation;

namespace Fermion.EntityFramework.Identity.Application.DTOs.ApplicationRoles;

public class UpdateApplicationRoleRequestDto
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string ConcurrencyStamp { get; set; } = null!;
}

public class UpdateApplicationRoleRequestValidator : AbstractValidator<UpdateApplicationRoleRequestDto>
{
    public UpdateApplicationRoleRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(256);
        
        RuleFor(x => x.Description)
            .MaximumLength(1024);
    }
}