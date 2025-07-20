using Fermion.EntityFramework.Identity.Domain.Entities;
using Fermion.EntityFramework.Identity.Domain.Interfaces.Repositories;
using Fermion.EntityFramework.Shared.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fermion.EntityFramework.Identity.Infrastructure.Repositories;

public class ApplicationUserSessionRepository<TContext> : EfRepositoryBase<ApplicationUserSession, Guid, TContext>, IApplicationUserSessionRepository where TContext : DbContext
{
    public ApplicationUserSessionRepository(TContext context) : base(context)
    {
    }
}