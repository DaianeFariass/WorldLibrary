using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WorldLibrary.Web.Helper;
using WorldLibrary.Web.Models;
using WorldLibrary.Web.Repositories;

namespace WorldLibrary.Web.Controllers
{

    public class CustomersController : Controller
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IBlobHelper _blobHelper;
        private readonly IConverterHelper _converterHelper;
        private readonly IUserHelper _userHelper;

        public CustomersController(ICustomerRepository customerRepository,
            IBlobHelper blobHelper,
            IConverterHelper converterHelper,
            IUserHelper userHelper)
        {
            _customerRepository = customerRepository;
            _blobHelper = blobHelper;
            _converterHelper = converterHelper;
            _userHelper = userHelper;
        }

        // GET: Customers
        public async Task<IActionResult> Index()
        {
            //Mudar
            var model = await _customerRepository.GetCustomerAsync(this.User.Identity.Name);
            return View(model); 
        }

        // GET: Customers/Details/5
        [Route("detailscustomers")]
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
        [Route("createcustomers")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("createcustomers")]
        public async Task<IActionResult> Create(CustomerViewModel model)
        {
            if (ModelState.IsValid)
            {
                Guid imageId = Guid.Empty;

                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "customers");

                }

                var customer = _converterHelper.ToCustomer(model, imageId, true);
                customer.User = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
                await _customerRepository.CreateAsync(customer);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Customers/Edit/5
        [Route("editcustomers")]
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
            var model = _converterHelper.ToCustomerViewModel(customer);
            return View(model);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("editcustomers")]
        public async Task<IActionResult> Edit(CustomerViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Guid imageId = Guid.Empty;

                    if (model.ImageFile != null && model.ImageFile.Length > 0)
                    {

                        imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "customers");

                    }

                    var customer = _converterHelper.ToCustomer(model, imageId, false);
                    customer.User = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
                    await _customerRepository.UpdateAsync(customer);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _customerRepository.ExistAsync(model.Id))
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
            return View(model);
        }

        // GET: Customers/Delete/5
        [Route("deletecustomers")]
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
        [Route("deletecustomers")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            try
            {
                await _customerRepository.DeleteAsync(customer);
                return RedirectToAction(nameof(Index));

            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message.Contains("DELETE"))
                {
                    ViewBag.ErrorTitle = $"{customer.FullName} probably in been used!";
                    ViewBag.ErrorMessage = $"{customer.FullName} can not be deleted because there are reserves with this customer.</br></br>" +
                        $"First delete all the reserves with this customer" +
                        $" and please try again delete it"!;

                }

                return View("Error");
            }
        }
        [Route("customernotfound")]
        public IActionResult CustomerNotFound()
        {
            return View();
        }

    }
}
