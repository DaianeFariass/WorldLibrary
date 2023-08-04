using WorldLibrary.Web.Data.Entities;
using WorldLibrary.Web.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace WorldLibrary.Web.Repositories
{
    public class ReserveRepository : GenericRepository<Reserve>, IReserveRepository
    {
        private readonly DataContext _context;
        public ReserveRepository(DataContext context) : base(context)
        {
            _context = context;
        }
        public IQueryable GetAllWithUsers()
        {
            return _context.PhysicalLibraries.Include(p => p.User);
        }
    }
}
