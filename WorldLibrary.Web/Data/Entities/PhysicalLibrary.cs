using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace WorldLibrary.Web.Data.Entities
{
    public class PhysicalLibrary : IEntity
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Country { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Image")]
        public string ImageUrl { get; set; }

        public User User { get; set; }
    }
}
