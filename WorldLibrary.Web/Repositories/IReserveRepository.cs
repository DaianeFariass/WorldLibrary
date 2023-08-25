using System;
using System.Linq;
using System.Threading.Tasks;
using WorldLibrary.Web.Data.Entities;
using WorldLibrary.Web.Models;

namespace WorldLibrary.Web.Repositories
{
    public interface IReserveRepository : IGenericRepository<Reserve>
    {
        public IQueryable GetAllWithUsers();

        Task<IQueryable<Reserve>> GetReserveAsync(string userName);
        Task<IQueryable<ReserveDetailTemp>> GetDetailsTempAsync(string userName);

        Task AddItemReserveAsync(AddReserveViewModel model, string username);

        Task<ReserveDetailTemp> GetReserveDetailTempAsync(int id);

        Task EditReserveDetailTempAsync(AddReserveViewModel model, string username);

        Task ModifyReserveDetailTempQuantityAsync(int id, double quantity);

        Task DeleteDetailTempAsync(int id);

        DateTime GetBookingDate();

        DateTime GetDeliveryDate();

        Task<bool> ConfirmReservAsync(string userName);

        Task DeliverReserve(DeliveryViewModel model);

        Task<Reserve> GetReserveAsync(int id);
    }
}
