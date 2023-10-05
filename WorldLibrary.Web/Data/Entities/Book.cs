using System;
using System.Collections.Generic;
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
        public string Assessment { get; set; }

        public List<Customer> Customers { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double Quantity { get; set; }

        [Display(Name = "Status")]
        public StatusBook? StatusBook { get; set; }

        [Display(Name = "Image")]
        public Guid ImageId { get; set; }
        public string BookPdfUrl { get; set; }
        public User User { get; set; }

        public string ImageFullPath => ImageId == Guid.Empty ?
        $"https://worldlibraryweb.blob.core.windows.net/books/noimage.png"
        : $"https://worldlibraryweb.blob.core.windows.net/books/{ImageId}";


    }
}
