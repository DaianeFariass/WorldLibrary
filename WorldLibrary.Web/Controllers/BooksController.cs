using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WorldLibrary.Web.Data;
using WorldLibrary.Web.Data.Entities;
using WorldLibrary.Web.Helper;
using WorldLibrary.Web.Models;
using WorldLibrary.Web.Repositories;

namespace WorldLibrary.Web.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class BooksController : Controller
    {
        private readonly IBookRepository _bookRepository;
        private readonly IUserHelper _userHelper;
        private readonly IBlobHelper _blobHelper;
        private readonly IConverterHelper _converterHelper;

        public BooksController(IBookRepository bookRepository,
            IUserHelper userHelper,
            IBlobHelper blobHelper,
            IConverterHelper converterHelper)
        {
            _bookRepository = bookRepository;
            _userHelper = userHelper;
            _blobHelper= blobHelper;
            _converterHelper=converterHelper;
        }

        // GET: Books
        public IActionResult Index()
        {
            return View(_bookRepository.GetAll().OrderBy(b => b.Title));
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("BookNotFound");
            }

            var book = await _bookRepository.GetByIdAsync(id.Value);
            if (book == null)
            {
                return new NotFoundViewResult("BookNotFound");
            }

            return View(book);
        }

        // GET: Books/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookViewModel model)
        {
            if (ModelState.IsValid)
            {
                Guid imageid= Guid.Empty;

                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    imageid =  await _blobHelper.UploadBlobAsync(model.ImageFile, "books");
                }

                var book = _converterHelper.ToBook(model, imageid, true);


                book.User = await _userHelper.GetUserByEmailAsync("evelyn.nunes@cinel.pt");
                await _bookRepository.CreateAsync(book);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

       
        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("BookNotFound");
            }

            var book = await _bookRepository.GetByIdAsync(id.Value);
            if (book == null)
            {
                return new NotFoundViewResult("BookNotFound");
            }

            var model = _converterHelper.ToBookViewModel(book);
            return View(model);
        }

        
        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BookViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Guid imageId = model.ImageId;

                    if (model.ImageFile != null && model.ImageFile.Length > 0)
                    {
                        imageId= await _blobHelper.UploadBlobAsync(model.ImageFile, "books");
                    }

                    var book = _converterHelper.ToBook(model, imageId, false);

                    book.User = await _userHelper.GetUserByEmailAsync("evelyn.nunes@cinel");
                    await _bookRepository.UpdateAsync(book);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _bookRepository.ExistAsync(model.Id))
                    {
                        return new NotFoundViewResult("BookNotFound");
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("BookNotFound");
            }

            var book = await _bookRepository.GetByIdAsync(id.Value);
            if (book == null)
            {
                return new NotFoundViewResult("BookNotFound");  
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _bookRepository.GetByIdAsync(id);
            try
            {
                await _bookRepository.DeleteAsync(book);
                return RedirectToAction(nameof(Index));

            }
            catch (DbUpdateException ex)
            {

                if (ex.InnerException != null && ex.InnerException.Message.Contains("DELETE"))
                {
                    ViewBag.ErrorTitle = $"{book.Title} probably in been used!!!";
                    ViewBag.ErrorMessage = $"{book.Title} can not be deleted because there are reserves with this book.</br></br>" +
                        $"First delete all the reserves with this book" +
                        $" and please try again delete it!";

                }

                return View("Error");
            }
        }

        public IActionResult BookNotFound()
        {
            return View();
        }
    }
}
