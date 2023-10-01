using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorldLibrary.Web.Data.Entities;
using WorldLibrary.Web.Models;

namespace WorldLibrary.Web.Repositories
{
    public interface IBookRepository : IGenericRepository<Book>
    {
        public IQueryable GetAllWithUsers();

        IEnumerable<SelectListItem> GetComboBooks();

        Task<Book> EditBookAsync(BookViewModel model, string username);
    }
}
