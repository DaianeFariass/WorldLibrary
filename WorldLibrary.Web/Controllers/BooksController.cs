using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
    public class BooksController : Controller
    {
        private readonly IBookRepository _bookRepository;
        private readonly IUserHelper _userHelper;
        public BooksController(IBookRepository bookRepository,
            IUserHelper userHelper)
        {
            _bookRepository = bookRepository;
            _userHelper = userHelper;
        }

        // GET: Books
        public async Task<IActionResult> Index()
        {
            return View(_bookRepository.GetAll().OrderBy(b => b.Title));
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _bookRepository.GetByIdAsync(id.Value);
            if (book == null)
            {
                return NotFound();
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
                var path = string.Empty;
                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    var guid = Guid.NewGuid().ToString();
                    var file = $"{guid}.jpg";

                    path= Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot\\image\\books",
                        file);

                    using (var strem = new FileStream(path, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(strem);
                    }

                    path = $"~/image/books/{file}";
                }

                var book = this.ToBook(model, path);

                book.User = await _userHelper.GetUserByEmailAsync("evelyn.nunes@cinel.pt");
                await _bookRepository.CreateAsync(book);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        private Book ToBook(BookViewModel model, string path)
        {
            return new Book
            {
                Id = model.Id,
                ImageUrl = path,
                Title = model.Title,
                Author = model.Author,
                Category=model.Category,
                IsAvailable=model.IsAvailable,
                Synopsis=model.Synopsis,
                Year=model.Year,
                User=model.User,
            };
        }

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _bookRepository.GetByIdAsync(id.Value);
            if (book == null)
            {
                return NotFound();
            }

            var model = this.ToBookViewModel(book);
            return View(model);
        }

        private BookViewModel ToBookViewModel(Book book)
        {
            return new BookViewModel
            {
                Id=book.Id,
                Title = book.Title,
                Author=book.Author,
                Category=book.Category,
                ImageUrl=book.ImageUrl,
                IsAvailable=book.IsAvailable,
                Synopsis=book.Synopsis,
                Year=book.Year,
                User = book.User,
            };
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
                    var path = string.Empty;
                    if (model.ImageFile != null && model.ImageFile.Length > 0)
                    {
                        var guid = Guid.NewGuid().ToString();
                        var file = $"{guid}.jpg";

                        path= Path.Combine(
                            Directory.GetCurrentDirectory(),
                            "wwwroot\\image\\books",
                            file);

                        using (var strem = new FileStream(path, FileMode.Create))
                        {
                            await model.ImageFile.CopyToAsync(strem);
                        }

                        path = $"~/image/books/{file}";
                    }

                    var book = this.ToBook(model, path);

                    book.User = await _userHelper.GetUserByEmailAsync("evelyn.nunes@cinel.pt");
                    await _bookRepository.UpdateAsync(book);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _bookRepository.ExistAsync(model.Id))
                    {
                        return NotFound();
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
                return NotFound();
            }

            var book = await _bookRepository.GetByIdAsync(id.Value);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _bookRepository.GetByIdAsync(id);
            await _bookRepository.DeleteAsync(book);
            return RedirectToAction(nameof(Index));
        }

       
    }
}
