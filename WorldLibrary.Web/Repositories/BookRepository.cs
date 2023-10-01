using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorldLibrary.Web.Data;
using WorldLibrary.Web.Data.Entities;
using WorldLibrary.Web.Helper;
using WorldLibrary.Web.Models;

namespace WorldLibrary.Web.Repositories
{
    public class BookRepository : GenericRepository<Book>, IBookRepository
    {
        private readonly DataContext _context;
        private readonly IBlobHelper _blobHelper;
        private readonly IUserHelper _userHelper;
        private readonly IConverterHelper _converterHelper;

        public BookRepository(DataContext context,
            IBlobHelper blobHelper,
            IUserHelper userHelper,
           IConverterHelper converterHelper) : base(context)
        {
            _context = context;
            _blobHelper = blobHelper;
            _userHelper = userHelper;
            _converterHelper = converterHelper;
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

        public async Task<Book> EditBookAsync(BookViewModel model, string username)
        {
            Guid imageId = model.ImageId;

            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                imageId= await _blobHelper.UploadBlobAsync(model.ImageFile, "books");
            }

            _converterHelper.ToBook(model, imageId, false);
            var user = await _userHelper.GetUserByEmailAsync(username);
            if (user == null)
            {
                return null;
            }
            var customer = _context.Customers.ToList();

            model.Customers = customer;
            _context.Update(model);
            await _context.SaveChangesAsync();
            return model;
        }
    }
}
