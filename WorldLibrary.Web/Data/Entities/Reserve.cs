using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using WorldLibrary.Web.Enums;

namespace WorldLibrary.Web.Data.Entities
{
    public class Reserve : IEntity
    {
        public int Id { get; set; }

        [Display(Name = "User Name ")]
        public User User { get; set; }

        [Display(Name = "Library ")]
        public PhysicalLibrary PhysicalLibrary { get; set; }    

        public Book Book { get; set; }

        [Display(Name = "Customer Name ")]
        public Customer Customer { get; set; }

        [Display(Name = "Booking Date ")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy }", ApplyFormatInEditMode = false)]
        public DateTime BookingDate { get; set; }

        [Display(Name = "Delivery Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy }", ApplyFormatInEditMode = false)]
        public DateTime? DeliveryDate { get; set; }

        [Display(Name = "Return Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy }", ApplyFormatInEditMode = false)]
        public DateTime? ReturnDate { get; set; }

        [Display(Name = "Actual Return Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy }", ApplyFormatInEditMode = false)]
        public DateTime? ActualReturnDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double Quantity { get; set; }

        [Display(Name = "Status ")]
        public StatusReserve? StatusReserve { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal? Rate { get; set; }

        [Display(Name = "Booking Date ")]
        public DateTime? ReserveDateLocal => this.BookingDate == null ? null : this.BookingDate.Date.ToLocalTime();
    }
}
