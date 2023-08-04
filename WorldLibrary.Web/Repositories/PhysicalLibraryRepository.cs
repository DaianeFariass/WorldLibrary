using Microsoft.EntityFrameworkCore;
using System.Linq;
using WorldLibrary.Web.Data;
using WorldLibrary.Web.Data.Entities;

namespace WorldLibrary.Web.Repositories
{
    public class PhysicalLibraryRepository : GenericRepository<PhysicalLibrary>, IPhysicalLibraryRepository
    {
        private readonly DataContext _context;
        public PhysicalLibraryRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable GetAllWithUsers()
        {
            return _context.PhysicalLibraries.Include(p => p.User);
        }
    }
}
