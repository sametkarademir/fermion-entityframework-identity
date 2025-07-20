using Fermion.EntityFramework.Identity.Domain.Entities;
using Fermion.EntityFramework.Identity.Domain.Interfaces.Repositories;
using Fermion.EntityFramework.Shared.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fermion.EntityFramework.Identity.Infrastructure.Repositories;

public class ApplicationUserTokenRepository<TContext> : EfRepositoryBase<ApplicationUserToken, TContext>, IApplicationUserTokenRepository where TContext : DbContext
{
    public ApplicationUserTokenRepository(TContext context) : base(context)
    {
    }
}