using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WorldLibrary.Web.Data;
using WorldLibrary.Web.Data.Entities;
using WorldLibrary.Web.Helper;
using WorldLibrary.Web.Repositories;

namespace WorldLibrary.Web.Controllers
{
    public class ReservesController : Controller
    {
        private readonly IReserveRepository _reserveRepository;
        private readonly IUserHelper _userHelper;

        public ReservesController(IReserveRepository reserveRepository,
            IUserHelper userHelper)
        {
            _reserveRepository = reserveRepository;
            _userHelper = userHelper;
        }

        // GET: Reserves
        public async Task<IActionResult> Index()
        {
            return View(_reserveRepository.GetAll());
        }

        // GET: Reserves/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reserve = await _reserveRepository.GetByIdAsync(id.Value);
            if (reserve == null)
            {
                return NotFound();
            }

            return View(reserve);
        }

        // GET: Reserves/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Reserves/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Reserve reserve)
        {
            if (ModelState.IsValid)
            {
                reserve.User = await _userHelper.GetUserByEmailAsync("daiane.farias@cinel.pt");
                await _reserveRepository.CreateAsync(reserve);
                return RedirectToAction(nameof(Index));
            }
            return View(reserve);
        }

        // GET: Reserves/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reserve = await _reserveRepository.GetByIdAsync(id.Value);
            if (reserve == null)
            {
                return NotFound();
            }
            return View(reserve);
        }

        // POST: Reserves/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Reserve reserve)
        {
            if (id != reserve.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    reserve.User = await _userHelper.GetUserByEmailAsync("daiane.farias@cinel.pt");
                    await _reserveRepository.UpdateAsync(reserve);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _reserveRepository.ExistAsync(reserve.Id))
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
            return View(reserve);
        }

        // GET: Reserves/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reserve = await _reserveRepository.GetByIdAsync(id.Value);
            if (reserve == null)
            {
                return NotFound();
            }

            return View(reserve);
        }

        // POST: Reserves/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reserve = await _reserveRepository.GetByIdAsync(id);
            await _reserveRepository.DeleteAsync(reserve);
            return RedirectToAction(nameof(Index));
        }

        
    }
}
