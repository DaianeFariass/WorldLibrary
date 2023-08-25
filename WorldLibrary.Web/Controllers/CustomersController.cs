using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WorldLibrary.Web.Data;
using WorldLibrary.Web.Data.Entities;
using WorldLibrary.Web.Helper;
using WorldLibrary.Web.Repositories;

namespace WorldLibrary.Web.Controllers
{
    
    public class CustomersController : Controller
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IUserHelper _userHelper;

        public CustomersController(ICustomerRepository customerRepository,
            IUserHelper userHelper)
        {
            _customerRepository = customerRepository;
            _userHelper = userHelper;
        }

        // GET: Customers
        public  IActionResult Index()
        {
            return View(_customerRepository.GetAll().OrderBy(c => c.FullName));
        }

        // GET: Customers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("CustomerNotFound");
            }

            var customer = await _customerRepository.GetByIdAsync(id.Value);
            if (customer == null)
            {
                return new NotFoundViewResult("CustomerNotFound");
            }

            return View(customer);
        }

        // GET: Customers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Customer customer)
        {
            if (ModelState.IsValid)
            {
                customer.User = await _userHelper.GetUserByEmailAsync("livania.viegas@cinel.pt");
                await _customerRepository.CreateAsync(customer);
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("CustomerNotFound");
            }

            var customer = await _customerRepository.GetByIdAsync(id.Value);
            if (customer == null)
            {
                return new NotFoundViewResult("CustomerNotFound");
            }
            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Customer customer)
        {
            if (id != customer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    customer.User = await _userHelper.GetUserByEmailAsync("livania.viegas@cinel.pt");
                    await _customerRepository.UpdateAsync(customer);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _customerRepository.ExistAsync(customer.Id))
                    {
                        return new NotFoundViewResult("CustomerNotFound");
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("CustomerNotFound");
            }

            var customer = await _customerRepository.GetByIdAsync(id.Value);
            if (customer == null)
            {
                return new NotFoundViewResult("CustomerNotFound");
            }

            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            await _customerRepository.DeleteAsync(customer);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult CustomerNotFound()
        {
            return View();
        }

    }
}
