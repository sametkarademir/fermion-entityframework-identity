using Fermion.EntityFramework.Identity.Domain.Entities;
using Fermion.EntityFramework.Identity.Domain.Interfaces.Repositories;
using Fermion.EntityFramework.Shared.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fermion.EntityFramework.Identity.Infrastructure.Repositories;

public class ApplicationUserClaimRepository<TContext> : EfRepositoryBase<ApplicationUserClaim, TContext>, IApplicationUserClaimRepository where TContext : DbContext
{
    public ApplicationUserClaimRepository(TContext context) : base(context)
    {
    }
}