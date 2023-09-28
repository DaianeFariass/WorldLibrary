using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

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
        public Guid ImageId { get; set; }
        public User User { get; set; }

        public string ImageFullPath => ImageId == Guid.Empty ?
        $"https://worldlibraryweb.blob.core.windows.net/physics/noimage.png"
        : $"https://worldlibraryweb.blob.core.windows.net/physics/{ImageId}";
    }
}
