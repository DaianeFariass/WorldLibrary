using WorldLibrary.Web.Data.Entities;
using WorldLibrary.Web.Enums;

namespace WorldLibrary.Web.Data
{
    public class Notification : IEntity
    {
        public int Id { get; set; }

        public Reserve Reserve { get; set; }

        public NotificationType NotificationType { get; set; }
    }
}
