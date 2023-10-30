using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.Language.Intermediate;
using Microsoft.AspNetCore.Routing;
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
        private readonly IAssessmentRepository _assessmentRepository;
        public ReservesController(IReserveRepository reserveRepository,
            IBookRepository bookRepository,
            ICustomerRepository customerRepository,
            IPhysicalLibraryRepository physicalLibraryRepository,
            IFlashMessage flashMessage,
            IMailHelper mailHelper,
            IUserHelper userHelper,
            DataContext context,
            IAssessmentRepository assessmentRepository)
        {
            _reserveRepository = reserveRepository;
            _bookRepository = bookRepository;
            _customerRepository = customerRepository;
            _physicalLibraryRepository = physicalLibraryRepository;
            _flashMessage = flashMessage;
            _mailHelper = mailHelper;
            _userHelper = userHelper;
            _context = context;
            _assessmentRepository=assessmentRepository;
        }


        public async Task<IActionResult> Index()
        {
            var model = await _reserveRepository.GetReserveAsync(this.User.Identity.Name);
            return View(model);
        }
        [Route("detailsreserve")]
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
        [Route("createreserve")]
        public async Task<IActionResult> Create()
        {
            var model = await _reserveRepository.GetDetailsTempAsync(this.User.Identity.Name);
            return View(model);
        }
        [Route("addreserve")]
        public async Task<IActionResult> AddReserve(User user)
        {

            user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);

            var model = await _reserveRepository.ReturnReserveViewModel(this.User.Identity.Name);
            return View(model);
        }

        [HttpPost]
        [Route("addreserve")]
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
                    model = await _reserveRepository.ReturnReserveViewModel(this.User.Identity.Name);

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
                    model = await _reserveRepository.ReturnReserveViewModel(this.User.Identity.Name);

                    return View(model);

                }
                var book = _context.Books.FindAsync(model.BookId);
                if (model.Quantity > book.Result.Quantity)
                {
                    _flashMessage.Danger("Quantity Invalid!");
                    model = await _reserveRepository.ReturnReserveViewModel(this.User.Identity.Name);

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
        [Route("editreserve")]
        public async Task<IActionResult> EditReserve(int? id, User user)
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
            var model = await _reserveRepository.ReturnReserveViewModel(this.User.Identity.Name);

            return View(model);

        }
           
        

        [HttpPost]
        [Route("editreserve")]
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
                        model = await _reserveRepository.ReturnReserveViewModel(this.User.Identity.Name);

                        return View(model);
                    }
                    if (model.Quantity > 3)
                    {
                        _flashMessage.Danger("Quantity Invalid! Only 3 books per customer");
                        model = await _reserveRepository.ReturnReserveViewModel(this.User.Identity.Name);

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
            return RedirectToAction("Index");
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
           
            var res = _context.Reserves
            .Include(x => x.Customer)
            .ToList();
            var customer = _context.Customers
           .Where(c => c.User == response.User)
           .FirstOrDefault();

            if (res.Count >= 4 && customer == response.Customer)
            {
                _mailHelper.SendEmail(customer.Email,
                 "Congratulation!", $"<h1> World Library</h1>" +
                 $"Dear {customer.FullName}, " +
                 $"Customer Premium!...</br></br>" +
                 $"Congratulation</br>" +
                 $"Now you became premium!</br>");
                await _reserveRepository.SendReserveNotification(response, customer.User.UserName, NotificationType.Premium);
                customer.Premium = true;
                _context.Update(customer);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        [Route("editconfirmedreserve")]
        public async Task<IActionResult> Edit(int? id)
        {
            var reserveToEdit = await _reserveRepository.GetReserveByIdAsync(id.Value);
            if (reserveToEdit.StatusReserve ==  StatusReserve.Concluded || reserveToEdit.StatusReserve == StatusReserve.Cancelled)
            {
                _flashMessage.Warning("Impossible to Edit!!!");
                return RedirectToAction("Index");
            }
            if (id == null)
            {

                return new NotFoundViewResult("ReserveNotFound");

            }


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
        [Route("editconfirmedreserve")]
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
        [Route("deliveryreserve")]
        public async Task<IActionResult> Deliver(int? id)
        {
            var reserve = await _reserveRepository.GetReserveAsync(id.Value); 
            if (reserve.StatusReserve == StatusReserve.Concluded || reserve.StatusReserve == StatusReserve.Cancelled)
            {
                _flashMessage.Warning("Impossible to Deliver!!!");
                return RedirectToAction("Index");
            }
            if (id == null)
            {
                return new NotFoundViewResult("ReserveNotFound");

            }

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
        [Route("deliveryreserve")]
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
        [Route("bookreturn")]
        public async Task<IActionResult> BookReturn(int? id)
        {
            var reserve = await _reserveRepository.GetReserveAsync(id.Value); // Alterei
            if (reserve.StatusReserve == StatusReserve.Concluded || reserve.StatusReserve == StatusReserve.Cancelled)
            {
                _flashMessage.Warning("Impossible to Action!!!");
                return RedirectToAction("Index");
            }
            if (id == null)
            {
                return new NotFoundViewResult("ReserveNotFound");

            }
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
        [Route("bookreturn")]
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
                string myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(response.User);
                string link = Url.Action("AssessmentBook", "Reserves", new
                {
                    userid = response.User.Id,
                    token = myToken,
                    reserveid = response.Id,
                }, protocol: HttpContext.Request.Scheme);

                if (response != null)
                {
                    _mailHelper.SendEmail(response.Customer.Email,
                      "Book Return! Thank You!!", $"<h1> World Library</h1>" +
                      $"Dear {response.Customer.FullName}, " +
                      $"Thank you for Return the book...</br></br>" +
                      $"Book:  {response.Book.Title}</br>" +
                      $"Quantity:  {response.Quantity}</br>" +
                      $"Return: {response.ReturnDate}</br>" +
                      $"Status: {response.StatusReserve}</br>" +
                     $"Please click in this link :</br></br><a href = \"{link}\">Assessment</a>");
                }
                var reserves = _context.Reserves
                    .Include(x => x.Customer)
                    .ToList();
                var customer = _context.Customers
               .Where(c => c.User == reserve.User)
               .FirstOrDefault();

                if (reserves.Count == 5 && customer == response.Customer)
                {
                    _mailHelper.SendEmail(response.Customer.Email,
                     "Congratulation!", $"<h1> World Library</h1>" +
                     $"Dear {response.Customer.FullName}, " +
                     $"Customer Premium!...</br></br>" +
                     $"Congratulation</br>" +
                     $"Now you became premium!</br>");

                    await _reserveRepository.SendReserveNotification(response, response.User.UserName, NotificationType.Premium);

                }



                return RedirectToAction("Index");


            }
            return View();

        }
        [Route("assessment")]
        public async Task<IActionResult> AssessmentBook(string userId, string token, string reserveId)
        {

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token) || string.IsNullOrEmpty(reserveId))
            {
                return NotFound();
            }

            var user = await _userHelper.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userHelper.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {

            }

            var reserve = await _reserveRepository.GetReserveByIdAsync(Convert.ToInt32(reserveId));

            var model = new AssessmentViewModel
            {

                Reserve = reserve,
                Assessments = _assessmentRepository.GetComboAssessment(),

            };
            ViewBag.Book = model.Reserve.Book.Title;
            return View(model);

        }
        [HttpPost]
        [Route("assessment")]
        public async Task<IActionResult> AssessmentBook(AssessmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _assessmentRepository.AddAssessmentAsync(model);
                _flashMessage.Confirmation("Thank you for your Assessment!!!");
                return View(model);
            }
            return RedirectToAction("index");

        }

        public async Task<IActionResult> Cancel(int id)
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
        [Route("renewbook")]
        public async Task<IActionResult> RenewBook(int? id) 
        {
            var reserve = await _reserveRepository.GetReserveAsync(id.Value);
            if (reserve.StatusReserve == StatusReserve.Concluded || reserve.StatusReserve == StatusReserve.Cancelled)
            {
                _flashMessage.Warning("Impossible to Deliver!!!");
                return RedirectToAction("Index");
            }
            if (id == null)
            {
                return new NotFoundViewResult("ReserveNotFound");

            }
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
        [Route("renewbook")]
        public async Task<IActionResult> RenewBook(BookReturnViewModel model) 
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
        [Route("reservenotfound")]
        public IActionResult ReserveNotFound()
        {

            return View();
        }
    }
}
