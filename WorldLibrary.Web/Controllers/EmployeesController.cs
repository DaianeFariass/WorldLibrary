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
    public class EmployeesController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUserHelper _userHelper;
        private readonly IBlobHelper _blobHelper;
        private readonly IConverterHelper _converterHelper;

        public EmployeesController(IEmployeeRepository employeeRepository,
            IUserHelper userHelper,
            IBlobHelper blobHelper,
            IConverterHelper converterHelper)
        {

            _employeeRepository = employeeRepository;
            _userHelper = userHelper;
            _blobHelper = blobHelper;
            _converterHelper=converterHelper;
        }

        // GET: Employees
        public IActionResult Index()
        {
            return View(_employeeRepository.GetAll().OrderBy(e => e.FullName));
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("EmployeeNotFound");
            }

            var employee = await _employeeRepository.GetByIdAsync(id.Value);
            if (employee == null)
            {
                return new NotFoundViewResult("EmployeeNotFound");
            }

            return View(employee);
        }

        // GET: Employees/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmployeeViewModel model)
        {
            if (ModelState.IsValid)
            {
                Guid imageid = Guid.Empty;

                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    imageid =  await _blobHelper.UploadBlobAsync(model.ImageFile, "employees");
                }

                var employee = _converterHelper.ToEmployee(model, imageid, true);

                employee.User = await _userHelper.GetUserByEmailAsync("daiane.farias@cinel.pt");
                await _employeeRepository.CreateAsync(employee);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

       
        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("EmployeeNotFound");
            }

            var employee = await _employeeRepository.GetByIdAsync(id.Value);
            if (employee == null)
            {
                return new NotFoundViewResult("EmployeeNotFound");
            }

            var model = _converterHelper.ToEmployeeViewModel(employee);
            return View(model);
        }

        
        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EmployeeViewModel model)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    Guid imageid = Guid.Empty;

                    if (model.ImageFile != null && model.ImageFile.Length > 0)
                    {
                        imageid =  await _blobHelper.UploadBlobAsync(model.ImageFile, "employees");
                    }

                    var employee = _converterHelper.ToEmployee(model, imageid, false);

                    employee.User = await _userHelper.GetUserByEmailAsync("daiane.farias@cinel.pt");
                    await _employeeRepository.UpdateAsync(employee);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _employeeRepository.ExistAsync(model.Id))
                    {
                        return new NotFoundViewResult("EmployeeNotFound");
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

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("EmployeeNotFound");
            }

            var employee = await _employeeRepository.GetByIdAsync(id.Value);
            if (employee == null)
            {
                return new NotFoundViewResult("EmployeeNotFound");
            }

            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            try
            {
                await _employeeRepository.DeleteAsync(employee);
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message.Contains("DELETE"))
                {
                    ViewBag.ErrorTitle = $"{employee.FullName} probably in been used!!!";
                    ViewBag.ErrorMessage = $"{employee.FullName} can not be deleted because there are reserves with this customer!</br></br>" +
                        $"First delete all the reserves with this customer" +
                        $" and please try again delete it!";

                }
                return View("Error");

            }
        }

        public IActionResult EmployeeNotFound()
        {
            return View();
        }
    }
}
