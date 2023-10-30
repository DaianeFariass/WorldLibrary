using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorldLibrary.Web.Data.Entities;
using WorldLibrary.Web.Models;

namespace WorldLibrary.Web.Repositories
{
    public interface IAssessmentRepository : IGenericRepository<Assessment>
    {
        IEnumerable<SelectListItem> GetComboAssessment();

        public Task AddAssessmentAsync(AssessmentViewModel model);
    }
}
