using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace WorldLibrary.Web.Data.Entities
{
    public class Employee : IEntity
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required]
        public string Document { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string CellPhone { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        public string JobPosition { get; set; }

        [Display(Name = "Image")]
        public Guid ImageId { get; set; }

        public User User { get; set; }

        public string ImageFullPath => ImageId == Guid.Empty ?
        $"https://worldlibraryweb.blob.core.windows.net/employees/noimage.png"
        : $"https://worldlibraryweb.blob.core.windows.net/employees/{ImageId}";
    }
}
