using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using System;

namespace WorldLibrary.Web.Data.Entities
{
    public class ReserveDetailTemp : IEntity
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "User Name ")]
        public User User { get; set; }

        [Required]
        [Display(Name = "Customer Name ")]
        public Customer Customer { get; set; }

        [Required]
        public Book Book { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double Quantity { get; set; }

    }
}
