using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using WorldLibrary.Web.Helper;
using WorldLibrary.Web.Models;
using WorldLibrary.Web.Repositories;

namespace WorldLibrary.Web.Controllers
{
    public class ReservesController : Controller
    {
        private readonly IReserveRepository _reserveRepository;
        private readonly IBookRepository _bookRepository;

        public ReservesController(IReserveRepository reserveRepository,
           IBookRepository bookRepository)
        {
            _reserveRepository=reserveRepository;
            _bookRepository=bookRepository;
            ;
        }
        public async Task<IActionResult> Index()
        {
            var model = await _reserveRepository.GetReserveAsync(this.User.Identity.Name);
            return View(model);
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
                await _reserveRepository.AddItemReserveAsync(model, this.User.Identity.Name);
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
                Books = _bookRepository.GetComboBooks(),
                BookDate = reserveToEdit.BookingDate,
                DeliveryDate = reserveToEdit.DeliveryDate,
                Quantity = Convert.ToInt32(reserveToEdit.Quantity),
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
                    if (model.DeliveryDate.Date < DateTime.Now.Date)
                    {

                        //_flashMessage.Warning("Date Invalid!");
                        model = new AddReserveViewModel
                        {
                            Books = _bookRepository.GetComboBooks(),
                            BookDate = DateTime.Now.Date,
                            DeliveryDate = DateTime.Now.Date,
                            Quantity= Convert.ToInt32(model.Quantity),

                        };
                        return View(model);
                    }
                    else
                    {
                        await _reserveRepository.EditReserveDetailTempAsync(model, this.User.Identity.Name);

                    }

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
                Id = reserve.Id,
                DeliveryDate = DateTime.Today
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Deliver(DeliveryViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _reserveRepository.DeliverReserve(model);
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
