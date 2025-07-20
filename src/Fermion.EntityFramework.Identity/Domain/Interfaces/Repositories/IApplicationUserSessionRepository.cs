using Fermion.EntityFramework.Identity.Domain.Entities;
using Fermion.EntityFramework.Shared.Interfaces;

namespace Fermion.EntityFramework.Identity.Domain.Interfaces.Repositories;

public interface IApplicationUserSessionRepository : IRepository<ApplicationUserSession, Guid>
{

}