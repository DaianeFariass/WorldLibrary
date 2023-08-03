using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace WorldLibrary.Web.Data.Entities
{
    public class Customer : IEntity
    {
        public int Id { get; set; }

        [Display(Name = "Full Name")]
        [Required]
        [MaxLength(100, ErrorMessage = "The field {0} can contain {1} characters length.")]
        public string FullName { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public string Document { get; set; }

        public User User { get; set; }
    }
}
