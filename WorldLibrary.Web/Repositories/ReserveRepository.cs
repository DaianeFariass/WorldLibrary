using WorldLibrary.Web.Data.Entities;
using WorldLibrary.Web.Data;

namespace WorldLibrary.Web.Repositories
{
    public class ReserveRepository : GenericRepository<Reserve>, IReserveRepository
    {
        public ReserveRepository(DataContext context) : base(context)
        {

        }
    }
}
