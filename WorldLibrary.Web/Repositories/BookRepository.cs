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

        /// <summary>
        /// Método que busca todos o livros disponíveis
        /// </summary>
        /// <returns>Books</returns>
        public IQueryable GetAllWithUsers()
        {
            return _context.Books.Include(p => p.User);
        }

        /// <summary>
        /// Método que preenche o combobox com nome dos livros 
        /// </summary>
        /// <returns>Books</returns>
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

        /// <summary>
        /// Método que Edita e atualiza a lista de livros 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task<Book> EditBookAsync(BookViewModel model, string username)
        {
            Guid imageId = model.ImageId;

            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                imageId= await _blobHelper.UploadBlobAsync(model.ImageFile, "books");
            }

            Guid imagePdf = Guid.Empty;

            if (model.BookPdf != null && model.BookPdf.Length > 0)
            {
                imagePdf = await _blobHelper.UploadBlobAsync(model.BookPdf, "pdfs");
            }

            var user = await _userHelper.GetUserByEmailAsync(username);
            if (user == null)
            {
                return null;
            }
            var customer = _context.Customers.ToList();
            model.Customers = customer;

            var book = _converterHelper.ToBook(model, imageId, imagePdf, false);
            book.User = user;
            book.Assessment = model.AssessmentId.ToString();

            _context.Books.Update(book);
            await _context.SaveChangesAsync();
            return book;
        }
    }
}
