using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Vereyon.Web;
using WorldLibrary.Web.Data;
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
        private readonly IFlashMessage _flashMessage;
        private readonly DataContext _context;

        public ReservesController(IReserveRepository reserveRepository,
            IBookRepository bookRepository,
            ICustomerRepository customerRepository,
            IFlashMessage flashMessage,
            DataContext context)
        {
            _reserveRepository = reserveRepository;
            _bookRepository = bookRepository;
            _customerRepository = customerRepository;
            _flashMessage = flashMessage;
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

                    if (!await _reserveRepository.ExistAsync(model.BookId))
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
            if (response)
            {

                return RedirectToAction("Index");
            }
            return RedirectToAction("Create");
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
                    await _reserveRepository.DeliverReserveAsync(model);


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
                else
                {
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

                    await _reserveRepository.BookReturnAsync(model);


                    return RedirectToAction("Index");

                }

            }
            return View();

        }
        public async Task<IActionResult> Cancel(int id)
        {
            var reserve = await _reserveRepository.GetReserveAsync(id);
            if (reserve.DeliveryDate != null)
            {
                _flashMessage.Warning("It is not possible to cancel! The book is already deliveryed...");
                return RedirectToAction("index");
            }
            var model = new BookReturnViewModel
            {
                Books = _bookRepository.GetComboBooks(),
            };
            return View(model);

        }
        [HttpPost]
        public async Task<IActionResult> Cancel(BookReturnViewModel model)
        {
            if (ModelState.IsValid)
            {
                var reserve = await _reserveRepository.GetReserveAsync(model.Id);
                if (reserve.DeliveryDate != null)
                {
                    _flashMessage.Warning("It is not possible to cancel! The book is already deliveryed...");
                    return RedirectToAction("index");
                }
                var response = await _reserveRepository.CancelReserveAsync(model);
                if (response)
                {
                    return RedirectToAction("index");
                }


            }

            return RedirectToAction("index");

        }
        public IActionResult ReserveNotFound()
        {

            return View();
        }
    }
}
