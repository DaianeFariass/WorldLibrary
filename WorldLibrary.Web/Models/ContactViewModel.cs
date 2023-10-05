using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using WorldLibrary.Web.Data.Entities;

namespace WorldLibrary.Web.Models
{
    public class ContactViewModel : Contact
    {
        [Display(Name = "Customers")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select the Customer.")]
        public int? CustomerId { get; set; }

        public IEnumerable<SelectListItem> Customers { get; set; }

        [Display(Name = "Employees")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select the Employee.")]
        public int? EmployeeId { get; set; }

        public IEnumerable<SelectListItem> Employees { get; set; }
    }
}
