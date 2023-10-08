using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using System;

namespace WorldLibrary.Web.Models
{
    public class BookReturnViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Books")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a Book.")]
        public int BookId { get; set; }
        public IEnumerable<SelectListItem> Books { get; set; }

        [Display(Name = "Return Date")]
        public DateTime? ReturnDate { get; set; }

        [Display(Name = "Actual Return Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy }", ApplyFormatInEditMode = false)]
        public DateTime? ActualReturnDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal Rate { get; set; }
        public string Username { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double Quantity { get; set; }
    }
}
