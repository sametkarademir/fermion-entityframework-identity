using Fermion.EntityFramework.Identity.Domain.Entities;
using Fermion.EntityFramework.Identity.Domain.Interfaces.Repositories;
using Fermion.EntityFramework.Shared.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fermion.EntityFramework.Identity.Infrastructure.Repositories;

public class ApplicationUserLoginRepository<TContext> : EfRepositoryBase<ApplicationUserLogin, TContext>, IApplicationUserLoginRepository where TContext : DbContext
{
    public ApplicationUserLoginRepository(TContext context) : base(context)
    {
    }
}