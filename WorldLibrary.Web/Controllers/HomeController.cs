using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WorldLibrary.Web.Data;
using WorldLibrary.Web.Data.Entities;
using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using WorldLibrary.Web.Models;
using WorldLibrary.Web.Helper;
using Vereyon.Web;

namespace WorldLibrary.Web.Controllers
{
   
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DataContext _context;
        private readonly IMailHelper _mailHelper;
        private readonly IFlashMessage _flashMessage;
        public HomeController(ILogger<HomeController> logger,
            DataContext context,
            IMailHelper mailHelper,
            IFlashMessage flashMessage)
        {
            _logger = logger;
            _context = context;
            _mailHelper = mailHelper;
            _flashMessage = flashMessage;
        }
   
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [Route("contactindex")]
        public IActionResult ContactIndex(ContactViewModel model)
        {

            Response response = _mailHelper.SendEmail("romeupires@yopmail.com", model.Subject, model.Message);

            _context.Contacts.Add(model);
            _context.SaveChangesAsync();
            if (response.IsSuccess)
            {

                _flashMessage.Confirmation("The email was sent successfully!!!");
                return RedirectToAction("Index");

            }

            return RedirectToAction("Index");
        }
        [Route("about")]
        public IActionResult About()
        {
            return View();
        }
        [Route("privacy")]
        public IActionResult Privacy()
        {
            return View();
        }
        [Route("forum")]
        public IActionResult Forum()
        {
            var contactos = _context.Forums.ToList(); 
            return View(contactos);
        }
       
        [HttpPost]
        [Route("forum")]
        public async Task<IActionResult> Forum(string name, string email, string menssage, int assessment)
        {
            if (ModelState.IsValid)
            {
                var forum = new Forum
                {
                    Name = name,
                    Email = email,
                    Menssage = menssage,
                    Assessment = assessment,
                    Date = DateTime.Now,
                };
                _context.Forums.Add(forum);
                await _context.SaveChangesAsync();


                string smtpServer = "smtp.gmail.com";
                int smtpPort = 587;
                string smtpUsername = "daiane.dcaf@gmail.com";
                string smtpPassword = "jhqziweoinbuxtlx";

                SmtpClient smtpClient = new SmtpClient(smtpServer)
                {
                    Port = smtpPort,
                    Credentials = new NetworkCredential(smtpUsername, smtpPassword),
                    EnableSsl = true
                };


                var mailMessage = new MailMessage
                {
                    From = new MailAddress("daiane.dcaf@gmail.com"),
                    Subject = "Thank you for your participation",
                    Body = "<p>Thank you for sending your comment or suggestion. Thank you for your participation!<p/>"+
                            "<p>Team World Library<p>",
                    IsBodyHtml = true,
                };

                mailMessage.To.Add(email);


                try
                {
                    smtpClient.Send(mailMessage);
                }
                catch (Exception )
                {

                    return RedirectToAction("ErroAoEnviarEmail");
                }

                return RedirectToAction("Forum");
            }

            return View();
        }

    }

}

