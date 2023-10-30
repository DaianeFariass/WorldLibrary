using System.ComponentModel.DataAnnotations;

namespace WorldLibrary.Web.Data.Entities
{
    public class Assessment : IEntity
    {
        public int Id { get; set; }

        [Display(Name = "Book Title")]
        public Reserve Reserve { get; set; }
    }
}
