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
    public class PhysicalLibrariesController : Controller
    {
        private readonly IPhysicalLibraryRepository _physicalLibraryRepository;
        private readonly IUserHelper _userHelper;

        public PhysicalLibrariesController(IPhysicalLibraryRepository physicalLibraryRepository,
            IUserHelper userHelper)
        {

            _physicalLibraryRepository = physicalLibraryRepository;
            _userHelper = userHelper;
        }

        // GET: PhysicalLibraries
        public async Task<IActionResult> Index()
        {
            return View(_physicalLibraryRepository.GetAll().OrderBy(p => p.Name));
        }

        // GET: PhysicalLibraries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var physicalLibrary = await _physicalLibraryRepository.GetByIdAsync(id.Value);
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
        public async Task<IActionResult> Create(PhysicalLibraryViewModel model)
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
                        "wwwroot\\image\\libraries",
                       file);

                    using (var strem = new FileStream(path, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(strem);
                    }

                    path = $"~/image/libraries/{file}";
                }

                var physicalLibrary = this.ToLibrary(model, path);

                physicalLibrary.User = await _userHelper.GetUserByEmailAsync("evelyn.nunes@cinel.pt");
                await _physicalLibraryRepository.CreateAsync(physicalLibrary);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        private PhysicalLibrary ToLibrary(PhysicalLibraryViewModel model, string path)
        {
            return new PhysicalLibrary
            {
                Id = model.Id,
                ImageUrl= path,
                Country =model.Country,
                Email=model.Email,
                Name= model.Name,
                PhoneNumber= model.PhoneNumber,
                User= model.User,
            };
        }

        // GET: PhysicalLibraries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var physicalLibrary = await _physicalLibraryRepository.GetByIdAsync(id.Value);
            if (physicalLibrary == null)
            {
                return NotFound();
            }

            var model = this.ToLibraryViewModel(physicalLibrary);
            return View(model);
        }

        private PhysicalLibraryViewModel ToLibraryViewModel(PhysicalLibrary physicalLibrary)
        {
            return new PhysicalLibraryViewModel
            {
                Id = physicalLibrary.Id,
                Country=physicalLibrary.Country,
                Name=physicalLibrary.Name,
                Email=physicalLibrary.Email,
                PhoneNumber=physicalLibrary.PhoneNumber,
                ImageUrl=physicalLibrary.ImageUrl,
                User=physicalLibrary.User,
            };
        }

        // POST: PhysicalLibraries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PhysicalLibraryViewModel model)
        {


            if (ModelState.IsValid)
            {
                try
                {
                    var path = model.ImageUrl;
                    if (model.ImageFile != null && model.ImageFile.Length > 0)
                    {
                        var guid = Guid.NewGuid().ToString();
                        var file = $"{guid}.jpg";

                        path= Path.Combine(
                            Directory.GetCurrentDirectory(),
                            "wwwroot\\image\\libraries",
                           file);

                        using (var strem = new FileStream(path, FileMode.Create))
                        {
                            await model.ImageFile.CopyToAsync(strem);
                        }

                        path = $"~/image/libraries/{file}";
                    }

                    var physicalLibrary = this.ToLibrary(model, path);
                    physicalLibrary.User = await _userHelper.GetUserByEmailAsync("evelyn.nunes@cinel.pt");
                    await _physicalLibraryRepository.UpdateAsync(physicalLibrary);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _physicalLibraryRepository.ExistAsync(model.Id))
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

        // GET: PhysicalLibraries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var physicalLibrary = await _physicalLibraryRepository.GetByIdAsync(id.Value);
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
            var physicalLibrary = await _physicalLibraryRepository.GetByIdAsync(id);
            await _physicalLibraryRepository.DeleteAsync(physicalLibrary);
            return RedirectToAction(nameof(Index));
        }

    }
}
