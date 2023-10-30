using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Vereyon.Web;
using WorldLibrary.Web.Data;
using WorldLibrary.Web.Data.Entities;
using WorldLibrary.Web.Enums;
using WorldLibrary.Web.Helper;
using WorldLibrary.Web.Models;
using WorldLibrary.Web.Repositories;

namespace WorldLibrary.Web.Controllers
{
    
    public class BooksController : Controller
    {
        private readonly IBookRepository _bookRepository;
        private readonly IAssessmentRepository _assessmentRepository;
        private readonly IUserHelper _userHelper;
        private readonly IBlobHelper _blobHelper;
        private readonly IConverterHelper _converterHelper;
        private readonly IMailHelper _mailHelper;
        private readonly IFlashMessage _flashMessage;
        private readonly DataContext _context;

        public BooksController(IBookRepository bookRepository,
            IAssessmentRepository assessmentRepository,
            IUserHelper userHelper,
            IBlobHelper blobHelper,
            IConverterHelper converterHelper,
            IMailHelper mailHelper,
            IFlashMessage flashMessage,
            DataContext context)
        {
            _bookRepository = bookRepository;
            _assessmentRepository = assessmentRepository;
            _userHelper = userHelper;
            _blobHelper = blobHelper;
            _converterHelper = converterHelper;
            _flashMessage = flashMessage;
            _mailHelper = mailHelper;
            _context = context;
        }

        // GET: Books
        public IActionResult Index()
        {
            return View(_bookRepository.GetAll().OrderBy(b => b.Title));
        }

        // GET: Books/Details/5
        [Route("details")]
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
        [Route("create")]
     
        public IActionResult Create()
        {
            var model = new BookViewModel
            {
                Assessments = _assessmentRepository.GetComboAssessment()
            };
            return View(model);
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("create")]

        public async Task<IActionResult> Create(BookViewModel model)
        {
            if (ModelState.IsValid)
            {
                Guid imageId = Guid.Empty;

                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "books");
                }

                Guid imagePdf = Guid.Empty;
                if (model.BookPdf != null && model.BookPdf.Length > 0)
                {
                    imagePdf = await _blobHelper.UploadBlobAsync(model.BookPdf, "pdfs");
                }

                var book = _converterHelper.ToBook(model, imageId, imagePdf, true);
                book.Assessment = model.AssessmentId.ToString();
                book.User = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
                await _bookRepository.CreateAsync(book);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }


        // GET: Books/Edit/5
        [Route("edit")]
        [Authorize(Policy = "LibrarianOrAssistant")]
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
            model.Assessments = _assessmentRepository.GetComboAssessment();
            return View(model);
        }


        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit")]
        public async Task<IActionResult> Edit(BookViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    if (model.Quantity != 0 && model.StatusBook == StatusBook.Available)
                    {
                        var response = await _bookRepository.EditBookAsync(model, this.User.Identity.Name);
                        
                        foreach (var customer in model.Customers)
                        {
                            _mailHelper.SendEmail(customer.Email,
                             "Book Available", $"<h1>Book World Library</h1>" +
                              $"Dear {customer.FullName}, " +
                              $"The book {response.Title} is now available again</br></br>" +
                              "<p>To make a reservation, visit our website</p>");
                        }

                    }
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
        [Route("delete")]
        [Authorize(Roles ="Librarian, Assistant")]
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
        [Route("delete")]
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

                if (ex.InnerException 
                    != null && ex.InnerException.Message.Contains("DELETE"))
                {
                    ViewBag.ErrorTitle = $"{book.Title} probably in been used!!!";
                    ViewBag.ErrorMessage = $"{book.Title} can not be deleted because there are reserves with this book.</br></br>" +
                        $"First delete all the reserves with this book" +
                        $" and please try again delete it!";

                }

                return View("Error");
            }
        }
        [Route("download")]
        //criar
        public async Task<IActionResult> DownloadPdf(BookViewModel model)
        {
             var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);

            bool librarian = await _userHelper.IsUserInRoleAsync(user, "Librarian");
            bool asssitant = await _userHelper.IsUserInRoleAsync(user, "Assistant");
            var book = _context.Books
               .Where(b => b.Id == model.Id)
               .FirstOrDefault();
           if(librarian == true || asssitant == true)
           {
                if (book.ImagePdf == Guid.Parse("00000000-0000-0000-0000-000000000000"))
                {
                    _flashMessage.Info("There is no book online");
                    return View();
                }
                
                var pdf = _blobHelper.DownloadBlobPdfAsString(model.Id);
                string base64String = Convert.ToBase64String(pdf.Result);
                string dataUrl = $"data:application/pdf;base64,{base64String}";
                ViewBag.PdfUrl = dataUrl;
                return View();
           }
               

            var customer = _context.Customers
                .Include(c => c.User)
                .Where(c => c.User.UserName == this.User.Identity.Name)
                .FirstOrDefault();
            if(customer == null) 
            {
                _flashMessage.Info("Access denied!!!" +                    
                    "Please became premium first!");
                return View();


            }
            if (customer.Premium == null) 
            {
                _flashMessage.Info("Access denied!!!" +
                    "Please became premium first!");
                return View();

            }
            if (book.ImagePdf == Guid.Parse("00000000-0000-0000-0000-000000000000"))
            {
                _flashMessage.Info("There is no book online");
                return View();
            }
            var pdf2 = _blobHelper.DownloadBlobPdfAsString(model.Id);
            string base64String2 = Convert.ToBase64String(pdf2.Result);
            string dataUrl2 = $"data:application/pdf;base64,{base64String2}";
            ViewBag.PdfUrl = dataUrl2;
            return View();
        }

        [Route("book")]
        public IActionResult BookNotFound()
        {
            return View();
        }
        [Route("advancedsearch")]
        public IActionResult AdvancedSearch(string category, string title1, string author, string year, string status)
        {
            if (!string.IsNullOrEmpty(category) || !string.IsNullOrEmpty(title1) || !string.IsNullOrEmpty(author) || !string.IsNullOrEmpty(year) || !string.IsNullOrEmpty(status))
            {

                var result = _context.Books.AsQueryable();

                if (!string.IsNullOrEmpty(category))
                {
                    result = result.Where(b => b.Category.Contains(category));
                }

                if (!string.IsNullOrEmpty(title1))
                {
                    result = result.Where(b => b.Title.Contains(title1));
                }

                if (!string.IsNullOrEmpty(author))
                {
                    result = result.Where(b => b.Author.Contains(author));
                }

                if (!string.IsNullOrEmpty(year))
                {
                    result = result.Where(b => b.Year == year);
                }

                if (!string.IsNullOrEmpty(status))
                {
                    StatusBook statusValue;
                    if (Enum.TryParse(status, out statusValue))
                    {
                        result = result.Where(b => b.StatusBook == statusValue);
                    }
                }


                var result2 = result.ToList();


                return View(result2);
            }
            else
            {

                var emptyList = new List<Book>();
                return View(emptyList);
            }

        }
    }
}
