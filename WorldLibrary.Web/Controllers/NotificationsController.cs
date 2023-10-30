using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WorldLibrary.Web.Data;
using WorldLibrary.Web.Helper;
using WorldLibrary.Web.Repositories;

namespace WorldLibrary.Web.Controllers
{
    public class NotificationsController : Controller
    {
        
        private readonly DataContext _context;
        private readonly INotificationRepository _notificationRepository;

        public NotificationsController(DataContext context,
            INotificationRepository notificationRepository)
        {
            _context = context;
            _notificationRepository = notificationRepository;
        }

        // GET: Notifications
        public IActionResult Index()
        {
            var model = _notificationRepository.GetNotificationsAsync();
            return View(model);
        }
      
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if (id == null)
            {

                return new NotFoundViewResult("NotificationNotFound");

            }
            await _notificationRepository.DeleteNotificationAsync(id.Value);
            return RedirectToAction("Index");
        }
        public IActionResult NotificationNotFound()
        {
            return View();
        }
    }
    
}
