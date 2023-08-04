using System.Linq;
using WorldLibrary.Web.Data.Entities;

namespace WorldLibrary.Web.Repositories
{
    public interface IReserveRepository : IGenericRepository<Reserve>
    {
        public IQueryable GetAllWithUsers();
    }
}
