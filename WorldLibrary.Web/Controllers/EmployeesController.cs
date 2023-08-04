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

        public EmployeesController(IEmployeeRepository employeeRepository,
            IUserHelper userHelper)
        {

            _employeeRepository = employeeRepository;
            _userHelper = userHelper;
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
                return NotFound();
            }

            var employee = await _employeeRepository.GetByIdAsync(id.Value);
            if (employee == null)
            {
                return NotFound();
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
                var path = string.Empty;
                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    var guid = Guid.NewGuid().ToString();
                    var file = $"{guid}.jpg";

                    path= Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot\\image\\employees",
                        file);

                    using (var strem = new FileStream(path, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(strem);
                    }

                    path = $"~/image/employees/{file}";
                }

                var employee = this.ToEmployee(model, path);

                employee.User = await _userHelper.GetUserByEmailAsync("daiane.farias@cinel.pt");
                await _employeeRepository.CreateAsync(employee);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        private Employee ToEmployee(EmployeeViewModel model, string path)
        {
            return new Employee
            {
                Id = model.Id,
                ImageUrl = path,
                Address=model.Address,
                CellPhone=model.CellPhone,
                Document=model.Document,
                Email=model.Email,
                FullName=model.FullName,
                JobPosition=model.JobPosition,
                User=model.User,
            };
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _employeeRepository.GetByIdAsync(id.Value);
            if (employee == null)
            {
                return NotFound();
            }
            var model = this.ToEmployeeViewModel(employee);
            return View(model);
        }

        private EmployeeViewModel ToEmployeeViewModel(Employee employee)
        {
            return new EmployeeViewModel
            {
                Id=employee.Id,
                FullName=employee.FullName,
                Address=employee.Address,
                CellPhone=employee.CellPhone,
                Document=employee.Document,
                Email=employee.Email,
                ImageUrl=employee.ImageUrl,
                JobPosition=employee.JobPosition,
                User = employee.User,
            };
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
                    var path = string.Empty;
                    if (model.ImageFile != null && model.ImageFile.Length > 0)
                    {
                        var guid = Guid.NewGuid().ToString();
                        var file = $"{guid}.jpg";

                        path= Path.Combine(
                            Directory.GetCurrentDirectory(),
                            "wwwroot\\image\\employees",
                            file);

                        using (var strem = new FileStream(path, FileMode.Create))
                        {
                            await model.ImageFile.CopyToAsync(strem);
                        }

                        path = $"~/image/employees/{file}";
                    }

                    var employee = this.ToEmployee(model, path);
                    employee.User = await _userHelper.GetUserByEmailAsync("daiane.farias@cinel.pt");
                    await _employeeRepository.UpdateAsync(employee);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _employeeRepository.ExistAsync(model.Id))
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

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _employeeRepository.GetByIdAsync(id.Value);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            await _employeeRepository.DeleteAsync(employee);
            return RedirectToAction(nameof(Index));
        }
                
    }
}
