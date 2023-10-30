using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WorldLibrary.Web.Data;

namespace WorldLibrary.Web.Repositories
{
    public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
    {
        private readonly DataContext _context;

        public NotificationRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Método que deleta as notificações 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Delete</returns>

        public async  Task DeleteNotificationAsync(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null)
            {
                return;

            }
            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();
        }


        /// <summary>
        /// Método que retorna as notificaçoes com nome do cliente,livro, reserva, e id de quem gerou a notificação 
        /// </summary>
        /// <returns>Notificações</returns>
        public IQueryable GetNotificationsAsync()
        {
            return _context.Notifications
                .Include(n => n.Reserve)
                .ThenInclude(n => n.Book)
                .Include(n => n.Reserve)
                .ThenInclude(n => n.Customer)
                .Include(n => n.Reserve)
                .ThenInclude(n => n.User);
        }
    }
}
