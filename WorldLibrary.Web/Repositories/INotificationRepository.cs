using System.Linq;
using System.Threading.Tasks;
using WorldLibrary.Web.Data;

namespace WorldLibrary.Web.Repositories
{
    public interface INotificationRepository : IGenericRepository<Notification>
    {
        Task DeleteNotificationAsync(int id);

        IQueryable GetNotificationsAsync();
    }
}
