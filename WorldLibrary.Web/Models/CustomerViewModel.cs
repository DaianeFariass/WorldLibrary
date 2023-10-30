using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using WorldLibrary.Web.Data.Entities;

namespace WorldLibrary.Web.Models
{
    public class CustomerViewModel : Customer
    {
        [Display(Name = "Image")]
        public IFormFile ImageFile { get; set; }
    }
}
