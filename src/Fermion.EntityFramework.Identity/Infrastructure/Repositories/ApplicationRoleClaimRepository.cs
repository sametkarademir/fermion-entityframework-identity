using Fermion.EntityFramework.Identity.Domain.Entities;
using Fermion.EntityFramework.Identity.Domain.Interfaces.Repositories;
using Fermion.EntityFramework.Shared.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fermion.EntityFramework.Identity.Infrastructure.Repositories;

public class ApplicationRoleClaimRepository<TContext> : EfRepositoryBase<ApplicationRoleClaim, TContext>, IApplicationRoleClaimRepository
    where TContext : DbContext
{
    public ApplicationRoleClaimRepository(TContext context) : base(context)
    {
    }
}