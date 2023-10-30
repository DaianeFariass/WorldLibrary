using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using WorldLibrary.Web.Data.Entities;

namespace WorldLibrary.Web.Models
{
    public class BookViewModel : Book
    {
        [Display(Name = "Image")]
        public IFormFile ImageFile { get; set; }

        [Display(Name = "Book PDF")]
        public IFormFile BookPdf { get; set; }

        [Display(Name = "Assessment")]
        public string AssessmentId { get; set; }

        public IEnumerable<SelectListItem> Assessments { get; set; }


    }
}
