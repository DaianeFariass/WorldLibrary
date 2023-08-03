using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WorldLibrary.Web.Data;
using WorldLibrary.Web.Data.Entities;

namespace WorldLibrary.Web.Controllers
{
    public class PhysicalLibrariesController : Controller
    {
        private readonly DataContext _context;

        public PhysicalLibrariesController(DataContext context)
        {
            _context = context;
        }

        // GET: PhysicalLibraries
        public async Task<IActionResult> Index()
        {
            return View(await _context.PhysicalLibraries.ToListAsync());
        }

        // GET: PhysicalLibraries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var physicalLibrary = await _context.PhysicalLibraries
                .FirstOrDefaultAsync(m => m.Id == id);
            if (physicalLibrary == null)
            {
                return NotFound();
            }

            return View(physicalLibrary);
        }

        // GET: PhysicalLibraries/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PhysicalLibraries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Country,Email,PhoneNumber,ImageUrl")] PhysicalLibrary physicalLibrary)
        {
            if (ModelState.IsValid)
            {
                _context.Add(physicalLibrary);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(physicalLibrary);
        }

        // GET: PhysicalLibraries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var physicalLibrary = await _context.PhysicalLibraries.FindAsync(id);
            if (physicalLibrary == null)
            {
                return NotFound();
            }
            return View(physicalLibrary);
        }

        // POST: PhysicalLibraries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Country,Email,PhoneNumber,ImageUrl")] PhysicalLibrary physicalLibrary)
        {
            if (id != physicalLibrary.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(physicalLibrary);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PhysicalLibraryExists(physicalLibrary.Id))
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
            return View(physicalLibrary);
        }

        // GET: PhysicalLibraries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var physicalLibrary = await _context.PhysicalLibraries
                .FirstOrDefaultAsync(m => m.Id == id);
            if (physicalLibrary == null)
            {
                return NotFound();
            }

            return View(physicalLibrary);
        }

        // POST: PhysicalLibraries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var physicalLibrary = await _context.PhysicalLibraries.FindAsync(id);
            _context.PhysicalLibraries.Remove(physicalLibrary);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PhysicalLibraryExists(int id)
        {
            return _context.PhysicalLibraries.Any(e => e.Id == id);
        }
    }
}
