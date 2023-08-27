using System.ComponentModel.DataAnnotations;

namespace WorldLibrary.Web.Models
{
    public class RecoverPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
