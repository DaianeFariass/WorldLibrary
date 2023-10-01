using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using WorldLibrary.Web.Data.Entities;
using WorldLibrary.Web.Models;
using WorldLibrary.Web.Data;

namespace WorldLibrary.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DataContext _context;
        public HomeController(ILogger<HomeController> logger,
            DataContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Forum()
        {
            var contactos = _context.Forums.ToList(); 
            return View(contactos);
        }

        [HttpPost]
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
                catch (Exception ex)
                {

                    return RedirectToAction("ErroAoEnviarEmail");
                }

                return RedirectToAction("Forum");
            }

            return View();
        }

    }

}

