using System.Linq;
using WorldLibrary.Web.Data.Entities;

namespace WorldLibrary.Web.Repositories
{
    public interface IPhysicalLibraryRepository : IGenericRepository<PhysicalLibrary>
    {
        public IQueryable GetAllWithUsers();
    }
}
