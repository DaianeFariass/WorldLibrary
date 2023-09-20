using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using WorldLibrary.Web.Enums;

namespace WorldLibrary.Web.Data.Entities
{
    public class Book : IEntity
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Author { get; set; }

        [Required]
        public string Year { get; set; }

        [Required]
        public string Synopsis { get; set; }

        [Required]
        public string Category { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double Quantity { get; set; }

        [Display(Name = "Status")]
        public StatusBook? StatusBook { get; set; }

        [Display(Name = "Image")]
        public string ImageUrl { get; set; }

        public User User { get; set; }

        public string ImageFullPath
        {
            get
            {
                if (string.IsNullOrEmpty(ImageUrl))
                {
                    return null;
                }

                return $"https://localhost:44338{ImageUrl.Substring(1)}";
            }
        }
    }
}
