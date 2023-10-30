using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WorldLibrary.Web.Data.Entities;
using System.Xml.Linq;


namespace WorldLibrary.Web.Models
{
    public class AssessmentViewModel : Assessment
    {
        public int IdAssessment { get; set; }

        [Display(Name = "Classification")]
        public string AssessmentId { get; set; }

        public IEnumerable<SelectListItem> Assessments { get; set; }


    }
}
