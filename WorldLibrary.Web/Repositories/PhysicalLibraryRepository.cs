using WorldLibrary.Web.Data;
using WorldLibrary.Web.Data.Entities;

namespace WorldLibrary.Web.Repositories
{
    public class PhysicalLibraryRepository : GenericRepository<PhysicalLibrary>, IPhysicalLibraryRepository
    {
        public PhysicalLibraryRepository(DataContext context) : base(context)
        {

        }
    }
}
