using Microsoft.AspNetCore.Mvc;
using Vereyon.Web;
using WorldLibrary.Web.Data;
using WorldLibrary.Web.Helper;
using WorldLibrary.Web.Models;
using WorldLibrary.Web.Repositories;

namespace WorldLibrary.Web.Controllers
{
    public class ContactsController : Controller
    {
        public readonly DataContext _context;
        public readonly ICustomerRepository _customerRepository;
        public readonly IEmployeeRepository _employeeRepository;
        public readonly IMailHelper _mailHelper;
        public readonly IFlashMessage _flashMessage;
        public ContactsController(
             DataContext context,
             ICustomerRepository customerRepository,
             IEmployeeRepository employeeRepository,
             IMailHelper mailHelper,
             IFlashMessage flashMessage)
        {
            _context = context;
            _customerRepository = customerRepository;
            _employeeRepository = employeeRepository;
            _mailHelper = mailHelper;
            _flashMessage = flashMessage;
        }
        public IActionResult Index()
        {
            var model = new ContactViewModel
            {
                
                Employees = _employeeRepository.GetComboEmployeesEmail(),
            };
            return View(model);
        }

        public IActionResult SendMail(ContactViewModel model) 
        {

            Response response = _mailHelper.SendEmail(model.Email, model.Subject, model.Message);
            _context.Contacts.Add(model);
            _context.SaveChangesAsync();
            if (response.IsSuccess)
            {

                _flashMessage.Confirmation("The email was sent successfully!!!");
                return RedirectToAction("Index");

            }

            return RedirectToAction("Index");


        }

        public IActionResult ExternalContact()
        {
            var model = new ContactViewModel
            {
                Customers = _customerRepository.GetComboCustomersEmail(),
                
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult ExternalContact(ContactViewModel model)
        {
            Response response = _mailHelper.SendEmail(model.Email, model.Subject, model.Message);
            _context.Contacts.Add(model);
            _context.SaveChangesAsync();
            if (response.IsSuccess)
            {

                _flashMessage.Confirmation("The email was sent successfully!!!");
                return RedirectToAction("Index");

            }

            return RedirectToAction("Index");
        }
    }
}
