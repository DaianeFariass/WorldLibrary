using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using WorldLibrary.Web.Data.Entities;

namespace WorldLibrary.Web.Repositories
{
    public interface IPhysicalLibraryRepository : IGenericRepository<PhysicalLibrary>
    {
        public IQueryable GetAllWithUsers();

        public IEnumerable<SelectListItem> GetComboLibraries();
    }
}
