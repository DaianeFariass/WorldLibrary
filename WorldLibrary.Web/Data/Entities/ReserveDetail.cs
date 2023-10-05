using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using System;

namespace WorldLibrary.Web.Data.Entities
{
    public class ReserveDetail : IEntity
    {
        public int Id { get; set; }

        [Display(Name = "Library ")]
        public PhysicalLibrary PhysicalLibrary { get; set; } 

        [Display(Name = "Customer Name ")]
        public Customer Customer { get; set; }

        public Book Book { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double Quantity { get; set; }
    }
}
