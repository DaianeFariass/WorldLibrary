using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using WorldLibrary.Web.Data.Entities;

namespace WorldLibrary.Web.Models
{
    public class ReserveViewModel : Reserve
    {
        [Display(Name = "Libraries")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select the Library.")]
        public int LibraryId { get; set; } 

        public IEnumerable<SelectListItem> Libraries { get; set; } 

        [Display(Name = "Customers")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select the Customer.")]
        public int CustomerId { get; set; }

        public IEnumerable<SelectListItem> Customers { get; set; }

        [Display(Name = "Books")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a Book.")]
        public int BookId { get; set; }

        public IEnumerable<SelectListItem> Books { get; set; }
    }
}
