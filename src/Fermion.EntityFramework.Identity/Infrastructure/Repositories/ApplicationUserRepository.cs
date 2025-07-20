using Fermion.EntityFramework.Identity.Domain.Entities;
using Fermion.EntityFramework.Identity.Domain.Interfaces.Repositories;
using Fermion.EntityFramework.Shared.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fermion.EntityFramework.Identity.Infrastructure.Repositories;

public class ApplicationUserRepository<TContext> : EfRepositoryBase<ApplicationUser, TContext>, IApplicationUserRepository where TContext : DbContext
{
    public ApplicationUserRepository(TContext context) : base(context)
    {
    }
}