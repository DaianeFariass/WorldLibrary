using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using WorldLibrary.Web.Data.Entities;

namespace WorldLibrary.Web.Models
{
    public class EmployeeViewModel : Employee
    {
        [Display(Name = "Image")]
        public IFormFile ImageFile { get; set; }
    }
}
