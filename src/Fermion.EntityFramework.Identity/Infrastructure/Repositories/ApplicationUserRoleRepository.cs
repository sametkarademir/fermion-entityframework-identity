using Fermion.EntityFramework.Identity.Domain.Entities;
using Fermion.EntityFramework.Identity.Domain.Interfaces.Repositories;
using Fermion.EntityFramework.Shared.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fermion.EntityFramework.Identity.Infrastructure.Repositories;

public class ApplicationUserRoleRepository<TContext> : EfRepositoryBase<ApplicationUserRole, TContext>, IApplicationUserRoleRepository where TContext : DbContext
{
    public ApplicationUserRoleRepository(TContext context) : base(context)
    {
    }
}