using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using WorldLibrary.Web.Data.Entities;

namespace WorldLibrary.Web.Repositories
{
    public interface IEmployeeRepository : IGenericRepository<Employee>
    {
        public IQueryable GetAllWithUsers();
        IEnumerable<SelectListItem> GetComboEmployees();

        IEnumerable<SelectListItem> GetComboEmployeesEmail();

        IEnumerable<SelectListItem> GetComboRoles();
    }
}
