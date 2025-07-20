using Fermion.EntityFramework.Identity.Domain.Entities;
using Fermion.EntityFramework.Identity.Domain.Interfaces.Repositories;
using Fermion.EntityFramework.Shared.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fermion.EntityFramework.Identity.Infrastructure.Repositories;

public class ApplicationRoleRepository<TContext> : EfRepositoryBase<ApplicationRole, TContext>, IApplicationRoleRepository where TContext : DbContext
{
    public ApplicationRoleRepository(TContext context) : base(context)
    {
    }
}