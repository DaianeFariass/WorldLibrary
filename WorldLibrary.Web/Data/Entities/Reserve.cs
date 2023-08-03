using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using System;

namespace WorldLibrary.Web.Data.Entities
{
    public class Reserve : IEntity
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Book ")]
        public string Book { get; set; }  //vai receber a lista de livros 

        [Required]
        [Display(Name = "Booking Date ")]
        public DateTime BookingDate { get; set; }

        [Required]
        [Display(Name = "Delivery Date")]
        public DateTime DeliveryDate { get; set; }

        public string Rate { get; set; }

        public User User { get; set; }
    }
}
