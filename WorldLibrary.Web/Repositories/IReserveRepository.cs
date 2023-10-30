using System;
using System.Linq;
using System.Threading.Tasks;
using WorldLibrary.Web.Data.Entities;
using WorldLibrary.Web.Enums;
using WorldLibrary.Web.Models;

namespace WorldLibrary.Web.Repositories
{
    public interface IReserveRepository : IGenericRepository<Reserve>
    {
        Task<IQueryable<Reserve>> GetReserveAsync(string userName);

        public IQueryable GetAllWithUsers();

        Task<IQueryable<ReserveDetailTemp>> GetDetailsTempAsync(string userName);

        Task AddItemReserveAsync(AddReserveViewModel model, string username);

        Task<ReserveDetailTemp> GetReserveDetailTempAsync(int id);

        Task EditReserveDetailTempAsync(AddReserveViewModel model, string username);

        Task DeleteDetailTempAsync(int id);

        DateTime GetBookingDate();

        DateTime GetDeliveryDate();

        Task<Reserve> ConfirmReservAsync(string userName);

        Task<Reserve> DeliverReserveAsync(DeliveryViewModel model);

        Task<Reserve> GetReserveAsync(int id);

        Task<Reserve> BookReturnAsync(BookReturnViewModel model);

        Task<Reserve> RenewBookReturnAsync(BookReturnViewModel model);

        Task<Reserve> CancelReserveAsync(int id, string username);

        Task<Reserve> GetReserveByIdAsync(int id);

        Task<Reserve> EditReserveAsync(ReserveViewModel model, string username);

        Task SendReserveNotification(Reserve reserve, string username, NotificationType notificationType);
        Task<AddReserveViewModel> ReturnReserveViewModel(string userName);
    }
}
