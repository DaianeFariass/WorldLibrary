using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using System;

namespace WorldLibrary.Web.Data.Entities
{
    public class ReserveDetailTemp : IEntity
    {
        public int Id { get; set; }

        [Required]
        public User User { get; set; }

        [Required]
        public Book Book { get; set; }

        [Required]
        [Display(Name = "Booking Date ")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy }", ApplyFormatInEditMode = false)]
        public DateTime BookingDate { get; set; }

        [Required]
        [Display(Name = "Delivery Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy }", ApplyFormatInEditMode = false)]
        public DateTime DeliveryDate { get; set; }


        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double Quantity { get; set; }

    }
}
