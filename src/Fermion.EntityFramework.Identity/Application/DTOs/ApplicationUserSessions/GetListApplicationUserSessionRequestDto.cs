using System.Text.Json.Serialization;
using Fermion.EntityFramework.Shared.DTOs.Sorting;
using FluentValidation;

namespace Fermion.EntityFramework.Identity.Application.DTOs.ApplicationUserSessions;

public class GetListApplicationUserSessionRequestDto
{
    public int Page { get; set; } = 1;
    public int PerPage { get; set; } = 25;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public SortOrderTypes Order { get; set; } = SortOrderTypes.Desc;
    public string? Field { get; set; } = null;
    
    public string? Search { get; set; } = null;
    
    public Guid? UserId { get; set; } = null;
    
    public Guid? SnapshotId { get; set; }
    public Guid? CorrelationId { get; set; }
}

public class GetListUserSessionRequestValidator : AbstractValidator<GetListApplicationUserSessionRequestDto>
{
    public GetListUserSessionRequestValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0);

        RuleFor(x => x.PerPage)
            .InclusiveBetween(1, 100);

        RuleFor(x => x.Field)
            .MaximumLength(100)
            .Matches(@"^[a-zA-Z0-9_]+$");

        RuleFor(x => x.Order)
            .IsInEnum();

        RuleFor(x => x.Search)
            .MaximumLength(256)
            .Matches(@"^[a-zA-Z0-9\s]*$");

        RuleFor(x => x.UserId)
            .Must(x => x == null || x != Guid.Empty);
        
        RuleFor(x => x.SnapshotId)
            .Must(x => x == null || x != Guid.Empty);

        RuleFor(x => x.CorrelationId)
            .Must(x => x == null || x != Guid.Empty);
    }
}