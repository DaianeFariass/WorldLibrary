using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WorldLibrary.Web.Data;
using WorldLibrary.Web.Data.Entities;

namespace WorldLibrary.Web.Repositories
{
    public class BookRepository : GenericRepository<Book>, IBookRepository
    {
        private readonly DataContext _context;
        public BookRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable GetAllWithUsers()
        {
            return _context.Books.Include(p => p.User);
        }
        public IEnumerable<SelectListItem> GetComboBooks()
        {
            var list = _context.Books.Select(c => new SelectListItem
            {
                Text= c.Title,
                Value= c.Id.ToString()
            }).ToList();

            list.Insert(0, new SelectListItem
            {
                Text="(Select Book...)",
                Value="0"
            });

            return list;
        }
    }
}
