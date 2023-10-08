using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.Language.Intermediate;
using Microsoft.EntityFrameworkCore;
using System;
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
    public class ReservesController : Controller
    {
        private readonly IReserveRepository _reserveRepository;
        private readonly IBookRepository _bookRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IPhysicalLibraryRepository _physicalLibraryRepository;
        private readonly IFlashMessage _flashMessage;
        private readonly IMailHelper _mailHelper;
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        public ReservesController(IReserveRepository reserveRepository,
            IBookRepository bookRepository,
            ICustomerRepository customerRepository,
            IPhysicalLibraryRepository physicalLibraryRepository,
            IFlashMessage flashMessage,
            IMailHelper mailHelper,
            IUserHelper userHelper,
            DataContext context)
        {
            _reserveRepository = reserveRepository;
            _bookRepository = bookRepository;
            _customerRepository = customerRepository;
            _physicalLibraryRepository = physicalLibraryRepository;
            _flashMessage = flashMessage;
            _mailHelper = mailHelper;
            _userHelper = userHelper;
            _context = context;
        }


        public async Task<IActionResult> Index()
        {
            var model = await _reserveRepository.GetReserveAsync(this.User.Identity.Name);
            return View(model);
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("ReserveNotFound");
            }
            var details = await _reserveRepository.GetByIdAsync(id.Value);
            if (details == null)
            {
                return new NotFoundViewResult("ReserveNotFound");
            }

            return View(details);
        }

        public async Task<IActionResult> Create()
        {
            var model = await _reserveRepository.GetDetailsTempAsync(this.User.Identity.Name);
            return View(model);
        }

        public IActionResult AddReserve()
        {
            var model = new AddReserveViewModel
            {
                Libraries = _physicalLibraryRepository.GetComboLibraries(), 
                Customers = _customerRepository.GetComboCustomers(),
                Books = _bookRepository.GetComboBooks(),
                BookDate = _reserveRepository.GetBookingDate(),
                DeliveryDate = _reserveRepository.GetDeliveryDate(),

            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddReserve(AddReserveViewModel model)
        {
            if (ModelState.IsValid)
            {

                var reserves = await _reserveRepository.GetDetailsTempAsync(this.User.Identity.Name);
                bool reserveDetailTemp = reserves.Any(r =>
                r.Customer.Id == model.CustomerId &&
                r.Book.Id == model.BookId);

                if (reserveDetailTemp)
                {
                    _flashMessage.Danger("It is not possible to make another reservation for the same book");
                    model = new AddReserveViewModel
                    {
                        Libraries = _physicalLibraryRepository.GetComboLibraries(),
                        Customers = _customerRepository.GetComboCustomers(),
                        Books = _bookRepository.GetComboBooks(),
                        BookDate = _reserveRepository.GetBookingDate(),
                        DeliveryDate = _reserveRepository.GetDeliveryDate(),

                    };
                    return View(model);

                }
                var reserve = await _reserveRepository.GetReserveAsync(this.User.Identity.Name);
                bool reserveConfirmed = reserve.Any(r =>
                r.Customer.Id == model.CustomerId &&
                r.Book.Id == model.BookId &&
                r.StatusReserve.Value == StatusReserve.Active ||
                r.StatusReserve.Value == StatusReserve.Renewed ||
                r.StatusReserve.Value == StatusReserve.Delayed);

                if (reserveConfirmed)
                {
                    _flashMessage.Danger("It is not possible to make another reservation for the same book");
                    model = new AddReserveViewModel
                    {

                        Libraries = _physicalLibraryRepository.GetComboLibraries(), 
                        Customers = _customerRepository.GetComboCustomers(),
                        Books = _bookRepository.GetComboBooks(),
                        BookDate = _reserveRepository.GetBookingDate(),
                        DeliveryDate = _reserveRepository.GetDeliveryDate(),

                    };
                    return View(model);

                }
                var book = _context.Books.FindAsync(model.BookId);
                if (model.Quantity > book.Result.Quantity)
                {
                    _flashMessage.Danger("Quantity Invalid!");
                    model = new AddReserveViewModel
                    {
                        Customers = _customerRepository.GetComboCustomers(),
                        Books = _bookRepository.GetComboBooks(),
                        BookDate = _reserveRepository.GetBookingDate(),
                        DeliveryDate = _reserveRepository.GetDeliveryDate(),

                    };
                    return View(model);
                }
                if (model.Quantity > 3)
                {
                    _flashMessage.Danger("Quantity Invalid! Only 3 books per customer");
                    model = new AddReserveViewModel
                    {
                        Customers = _customerRepository.GetComboCustomers(),
                        Books = _bookRepository.GetComboBooks(),
                        BookDate = _reserveRepository.GetBookingDate(),
                        DeliveryDate = _reserveRepository.GetDeliveryDate(),

                    };
                    return View(model);
                }
                if (model.Quantity == book.Result.Quantity)
                {

                    await _reserveRepository.AddItemReserveAsync(model, this.User.Identity.Name);
                    book.Result.Quantity -= model.Quantity;
                    book.Result.StatusBook = StatusBook.Unvailable;
                    _context.Books.Update(book.Result);
                    await _context.SaveChangesAsync();
                }
                else
                {

                    await _reserveRepository.AddItemReserveAsync(model, this.User.Identity.Name);
                    book.Result.Quantity -= model.Quantity;
                    book.Result.StatusBook = StatusBook.Available;
                    _context.Books.Update(book.Result);
                    await _context.SaveChangesAsync();

                }


                return RedirectToAction("Create");

            }

            return View(model);
        }
        public async Task<IActionResult> EditReserve(int? id)
        {
            if (id == null)
            {

                return new NotFoundViewResult("ReserveNotFound");

            }
            var reserveToEdit = await _reserveRepository.GetReserveDetailTempAsync(id.Value);

            if (reserveToEdit == null)
            {
                return NotFound();

            }
            var model = new AddReserveViewModel
            {
                Libraries = _physicalLibraryRepository.GetComboLibraries(),
                Customers = _customerRepository.GetComboCustomers(),
                Books = _bookRepository.GetComboBooks(),
                Quantity = reserveToEdit.Quantity,

            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditReserve(AddReserveViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    var book = _context.Books.FindAsync(model.BookId);
                    if (model.Quantity > book.Result.Quantity)
                    {
                        _flashMessage.Danger("Quantity Invalid!");
                        model = new AddReserveViewModel
                        {
                            Libraries = _physicalLibraryRepository.GetComboLibraries(),
                            Customers = _customerRepository.GetComboCustomers(),
                            Books = _bookRepository.GetComboBooks(),
                            BookDate = _reserveRepository.GetBookingDate(),
                            DeliveryDate = _reserveRepository.GetDeliveryDate(),

                        };
                        return View(model);
                    }
                    if (model.Quantity > 3)
                    {
                        _flashMessage.Danger("Quantity Invalid! Only 3 books per customer");
                        model = new AddReserveViewModel
                        {

                            Libraries = _physicalLibraryRepository.GetComboLibraries(),
                            Customers = _customerRepository.GetComboCustomers(),
                            Books = _bookRepository.GetComboBooks(),
                            BookDate = _reserveRepository.GetBookingDate(),
                            DeliveryDate = _reserveRepository.GetDeliveryDate(),

                        };
                        return View(model);
                    }
                    var reserve = _context.ReserveDetailsTemp.FindAsync(model.Id);
                    double sub;

                    if (model.Quantity < reserve.Result.Quantity)
                    {
                        sub = reserve.Result.Quantity - model.Quantity;
                        book.Result.Quantity += sub;
                        book.Result.StatusBook = StatusBook.Available;


                    }
                    if (model.Quantity > reserve.Result.Quantity)
                    {
                        sub = model.Quantity - reserve.Result.Quantity;
                        book.Result.Quantity -= sub;
                        if (book.Result.Quantity == 0)
                        {
                            book.Result.StatusBook = StatusBook.Unvailable;

                        }
                        else
                        {
                            book.Result.StatusBook = StatusBook.Available;


                        }

                    }
                    if (model.Quantity == book.Result.Quantity)
                    {
                        book.Result.Quantity -= model.Quantity;
                        book.Result.StatusBook = StatusBook.Unvailable;
                    }

                    await _reserveRepository.EditReserveDetailTempAsync(model, this.User.Identity.Name);
                    _context.Books.Update(book.Result);
                    await _context.SaveChangesAsync();


                }
                catch (DbUpdateConcurrencyException)
                {

                    if (!await _reserveRepository.ExistAsync(model.Id))
                    {
                        return new NotFoundViewResult("ReserveNotFound");
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Create");
            }
            return View(model);
        }

        public async Task<IActionResult> DeleteItem(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("ReserveNotFound");
            }
            await _reserveRepository.DeleteDetailTempAsync(id.Value);
            return RedirectToAction("Create");
        }

        public async Task<IActionResult> ConfirmReserve()
        {
            var response = await _reserveRepository.ConfirmReservAsync(this.User.Identity.Name);

            if (response != null)
            {

                _mailHelper.SendEmail(response.Customer.Email,
                  "Reserve Confirmed", $"<h1>Book World Library</h1>" +
              $"Dear {response.Customer.FullName}, " +
                    $"Follow Your Reserve Details</br></br>" +
                    $"Book:  {response.Book.Title}</br>" +
                    $"Quantity:  {response.Quantity}</br>" +
                    $"Date: {response.BookingDate.Date}</br>" +
                    $"Status: {response.StatusReserve}</br>");

            }
            return RedirectToAction("Index");
        }
        
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {

                return new NotFoundViewResult("ReserveNotFound");

            }
            var reserveToEdit = await _reserveRepository.GetReserveByIdAsync(id.Value);

            if (reserveToEdit == null)
            {
                return new NotFoundViewResult("ReserveNotFound");

            }
            var model = new ReserveViewModel
            {
                Id = reserveToEdit.Id,
                Customers = _customerRepository.GetComboCustomers(),
                Libraries = _physicalLibraryRepository.GetComboLibraries(),
                Books = _bookRepository.GetComboBooks(),
                Quantity = reserveToEdit.Quantity,

            };

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(ReserveViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var reserveToEdit = await _reserveRepository.GetReserveByIdAsync(model.Id);
                    var book = _context.Books.FindAsync(model.BookId);
                    if (model.Quantity > book.Result.Quantity)
                    {
                        _flashMessage.Danger("Quantity Invalid!");

                        model = new ReserveViewModel
                        {
                            Id = reserveToEdit.Id,
                            Libraries = _physicalLibraryRepository.GetComboLibraries(),
                            Customers = _customerRepository.GetComboCustomers(),
                            Books = _bookRepository.GetComboBooks(),
                            Quantity = reserveToEdit.Quantity,
                            BookingDate = reserveToEdit.BookingDate,

                        };
                        return View(model);
                    }
                    if (model.Quantity > 3)
                    {
                        _flashMessage.Danger("Quantity Invalid! Only 3 books per customer");

                        model = new ReserveViewModel
                        {
                            Id = reserveToEdit.Id,
                            Libraries = _physicalLibraryRepository.GetComboLibraries(),
                            Customers = _customerRepository.GetComboCustomers(),
                            Books = _bookRepository.GetComboBooks(),
                            Quantity = reserveToEdit.Quantity,
                            BookingDate = reserveToEdit.BookingDate,

                        };
                        return View(model);
                    }


                    var response = await _reserveRepository.EditReserveAsync(model, this.User.Identity.Name);
                    if (response != null)
                    {
                        _mailHelper.SendEmail(response.Customer.Email,
                         "Appointment Modified", $"<h1>Book World Library</h1>" +
                          $"Dear {response.Customer.FullName}, " +
                          $"Your reserve was modified! Follow the new details...</br></br>" +
                          $"Book:  {response.Book.Title}</br>" +
                          $"Quantity:  {response.Quantity}</br>" +
                          $"Date: {response.BookingDate.Date}</br>" +
                          $"Status: {response.StatusReserve}</br>");


                    }




                }
                catch (Exception)
                {

                    throw;
                }
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public async Task<IActionResult> Deliver(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("ReserveNotFound");

            }
            var reserve = await _reserveRepository.GetReserveAsync(id.Value);
            if (reserve == null)
            {
                return new NotFoundViewResult("ReserveNotFound");

            }
            var model = new DeliveryViewModel
            {
                DeliveryDate = DateTime.Today.Date,
                ReturnDate = DateTime.Today.Date,

            };


            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Deliver(DeliveryViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.DeliveryDate.Date < DateTime.Now.Date)
                {
                    _flashMessage.Danger("Date Invalid!");
                    return RedirectToAction("Deliver");


                }
                else
                {
                    var response = await _reserveRepository.DeliverReserveAsync(model);
                    if (response != null)
                    {
                        _mailHelper.SendEmail(response.Customer.Email,
                         "Check Book Return", $"<h1> World Library</h1>" +
                          $"Dear {response.Customer.FullName}, " +
                          $"Please check you Book Return Date! Follow the Return details...</br></br>" +
                          $"Book:  {response.Book.Title}</br>" +
                          $"Quantity:  {response.Quantity}</br>" +
                          $"Delivery: {response.DeliveryDate.Value}</br>" +
                          $"Return: {response.ReturnDate.Value}</br>" +
                          $"Status: {response.StatusReserve}</br>");
                    }



                    return RedirectToAction("Index");

                }

            }
            return View();
        }
        public async Task<IActionResult> BookReturn(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("ReserveNotFound");

            }
            var reserve = await _reserveRepository.GetReserveAsync(id.Value);
            if (reserve == null)
            {
                return new NotFoundViewResult("ReserveNotFound");

            }
            if (reserve.DeliveryDate == null)
            {
                _flashMessage.Warning("It is not possible to return a book without the Delivery");
                return RedirectToAction("Index");
            }
            var model = new BookReturnViewModel
            {
                Id = reserve.Id,
                Books = _bookRepository.GetComboBooks(),
                ReturnDate = reserve.ReturnDate,
                ActualReturnDate = DateTime.Today.Date,
                Rate = 0,


            };
            if (model.ReturnDate == null)
            {
                ViewBag.ReturnDate = DateTime.Now;
            }

            ViewBag.ReturnDate = model.ReturnDate.ToString();
            ViewBag.Rate = 0.50;
            return View(model);


        }
        [HttpPost]
        public async Task<IActionResult> BookReturn(BookReturnViewModel model)
        {
            if (ModelState.IsValid)
            {
                var reserve = await _reserveRepository.GetReserveAsync(model.Id);
                if (model.ActualReturnDate < DateTime.Now.Date)
                {
                    _flashMessage.Danger("Date Invalid!");

                    model = new BookReturnViewModel
                    {
                        Id = reserve.Id,
                        Books = _bookRepository.GetComboBooks(),
                        ReturnDate = reserve.ReturnDate,
                        ActualReturnDate = DateTime.Today.Date,
                        Rate = 0,

                    };

                    return View(model);


                }
                if (model.Quantity > reserve.Quantity)
                {
                    _flashMessage.Danger("Quantity Invalid!");
                    model = new BookReturnViewModel
                    {
                        Id = reserve.Id,
                        Books = _bookRepository.GetComboBooks(),
                        ReturnDate = reserve.ReturnDate,
                        ActualReturnDate = DateTime.Today.Date,
                        Rate = 0,


                    };
                    return View(model);

                }
                if (model.Quantity > 3)
                {
                    _flashMessage.Danger("Quantity Invalid!");
                    model = new BookReturnViewModel
                    {
                        Id = reserve.Id,
                        Books = _bookRepository.GetComboBooks(),
                        ReturnDate = reserve.ReturnDate,
                        ActualReturnDate = DateTime.Today.Date,
                        Rate = 0,


                    };
                    return View(model);

                }
                var response = await _reserveRepository.BookReturnAsync(model);
               // var user = await _userHelper.GetUserByEmailAsync(model.Username);
                if (response != null)
                {
                    //string myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                    //string tokenLink = Url.Action("AssessmentBook", "Account", new
                    //{
                    //    userid = user.Id,
                    //    token = myToken
                    //}, protocol: HttpContext.Request.Scheme);
                    _mailHelper.SendEmail(response.Customer.Email,
                     "Book Return! Thank You!!", $"<h1> World Library</h1>" +
                      $"Dear {response.Customer.FullName}, " +
                      $"Thank you for Return the book...</br></br>" +
                      $"Book:  {response.Book.Title}</br>" +
                      $"Quantity:  {response.Quantity}</br>" +
                      // $"Return: {response.ReturnDate.Value}</br>" +
                      $"Return: {(response.ReturnDate.HasValue ? response.ReturnDate.Value.ToString() : "N/A")}</br>" +
                    $"Status: {response.StatusReserve}</br>");
                    //$"Click on the link to rate your experience with the book:</br></br><a href = \"{tokenLink}\">Assessment Book</a>");
                }


                return RedirectToAction("Index");


            }
            return View();

        }
        public async Task<IActionResult> Cancel(int id, string username)
        {
            var response = await _reserveRepository.CancelReserveAsync(id, this.User.Identity.Name);
            if (response == null)
            {
                _flashMessage.Warning("It is not possible to cancel! The reserve is already cancelled...");
                return RedirectToAction("index");
            }
            if (response.DeliveryDate != null)
            {
                _flashMessage.Warning("It is not possible to cancel! The book is already deliveryed...");
                return RedirectToAction("index");
            }
            else
            {
                _mailHelper.SendEmail(response.Customer.Email,
                "Appointment Cancelled", $"<h1>Book World Library</h1>" +
                  $"Dear {response.Customer.FullName}, " +
                  $"Your reserve was cancelled! Follow the details...</br></br>" +
                  $"Book:  {response.Book.Title}</br>" +
                  $"Quantity:  {response.Quantity}</br>" +
                  $"Date: {response.BookingDate.Date}</br>" +
                  $"Status: {response.StatusReserve}</br>");


            }
            return RedirectToAction("Index");

        }
        public async Task<IActionResult> RenewBook(int? id) 
        {
            if (id == null)
            {
                return new NotFoundViewResult("ReserveNotFound");

            }
            var reserve = await _reserveRepository.GetReserveAsync(id.Value);
            if (reserve == null)
            {
                return new NotFoundViewResult("ReserveNotFound");

            }
            if (DateTime.Now.Date < reserve.ReturnDate)
            {
                _flashMessage.Warning("You can't renew the book before the Return Date");
               
                return RedirectToAction("index");


            }
            var model = new BookReturnViewModel
            {
                ReturnDate = reserve.ReturnDate.Value,

            };


            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> RenewBook(BookReturnViewModel model) //Criar
        {
            if (ModelState.IsValid)
            {
                var reserve = await _reserveRepository.GetReserveAsync(model.Id);
                if (model.ReturnDate < DateTime.Now.Date)
                {
                    _flashMessage.Danger("Date Invalid!");

                    model = new BookReturnViewModel
                    {
                        Id = reserve.Id,
                        ReturnDate = reserve.ReturnDate,
                    };

                    return View(model);


                }
                var response = await _reserveRepository.RenewBookReturnAsync(model);
                if (response != null)
                {
                    _mailHelper.SendEmail(response.Customer.Email,
                     "Renew Book Return Date!", $"<h1> World Library</h1>" +
                      $"Dear {response.Customer.FullName}, " +
                      $"You renewed the Book Return Date! Please Follow the new Details...</br></br>" +
                      $"Book:  {response.Book.Title}</br>" +
                      $"Quantity:  {response.Quantity}</br>" +
                      $"Return: {response.ReturnDate.Value}</br>" +
                      $"Status: {response.StatusReserve}</br>");
                }


                return RedirectToAction("Index");


            }
            return View();
        }
        public IActionResult ReserveNotFound()
        {

            return View();
        }
    }
}
