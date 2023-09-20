using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using System;
using WorldLibrary.Web.Data.Entities;

namespace WorldLibrary.Web.Models
{
    public class DeliveryViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Delivery Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        public DateTime DeliveryDate { get; set; }

        [Display(Name = "Return Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        public DateTime ReturnDate { get; set; }

        [Display(Name = "Local Use")]
        public bool IsLocalUse { get; set; }

        public Reserve Reserve { get; set; }
    }
}
