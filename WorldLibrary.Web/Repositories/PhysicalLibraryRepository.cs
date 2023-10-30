using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WorldLibrary.Web.Data;
using WorldLibrary.Web.Data.Entities;

namespace WorldLibrary.Web.Repositories
{
    public class PhysicalLibraryRepository : GenericRepository<PhysicalLibrary>, IPhysicalLibraryRepository
    {
        private readonly DataContext _context;
        public PhysicalLibraryRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Métodod que busca todos as livrarias físicas 
        /// </summary>
        /// <returns>Libraries</returns>
        public IQueryable GetAllWithUsers()
        {
            return _context.PhysicalLibraries.Include(p => p.User);
        }

        /// <summary>
        /// Método que preeenche o combobox com a lista de livrarias 
        /// </summary>
        /// <returns>Libraries</returns>

        public IEnumerable<SelectListItem> GetComboLibraries()
        {
            var list = _context.PhysicalLibraries.Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString()
            }).ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "(Select the Library...)",
                Value = "0"
            });

            return list;
        }
    }
}
