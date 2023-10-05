using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using System;

namespace WorldLibrary.Web.Models
{
    public class AddReserveViewModel
    {
        public int Id { get; set; }

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

        [Display(Name = "Booking Date ")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        public DateTime BookDate { get; set; }

        [Display(Name = "Delivery Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        public DateTime DeliveryDate { get; set; }

        [Range(0.0001, double.MaxValue, ErrorMessage = "The quantity must be a positive number!")]
        public double Quantity { get; set; }
    }
}
