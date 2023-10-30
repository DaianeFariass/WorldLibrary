using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WorldLibrary.Web.Data;
using WorldLibrary.Web.Data.Entities;
using WorldLibrary.Web.Models;

namespace WorldLibrary.Web.Repositories
{
    public class EmployeeRepository : GenericRepository<Employee>, IEmployeeRepository
    {
        private readonly DataContext _context;
        public EmployeeRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Métodod que mostra todos os funcionários 
        /// </summary>
        /// <returns>Employees>returns>

        public IQueryable GetAllWithUsers()
        {
            return _context.Employees.Include(p => p.User);
        }


        /// <summary>
        /// Método que preenche o combobox com o nome dos funcionários 
        /// </summary>
        /// <returns>Employees</returns>
        public IEnumerable<SelectListItem> GetComboEmployees()
        {
            var list = _context.Employees.Select(c => new SelectListItem
            {
                Text = c.FullName,
                Value = c.Id.ToString()
            }).ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "(Select the Employee...)",
                Value = "0"
            });

            return list;
        }

        /// <summary>
        /// Método que preenche o combobox com elai dos funcionários 
        /// </summary>
        /// <returns>Employees Email</returns>

        public IEnumerable<SelectListItem> GetComboEmployeesEmail()
        {
            var list = _context.Employees.Select(p => new SelectListItem
            {
                Text = p.FullName,
                Value = p.Email,

            }).ToList();
            var allEmails = "";
            foreach (var email in list)
            {
                if (allEmails != "")
                {
                    allEmails = allEmails + ",";
                }
                allEmails = allEmails + email.Value.ToString();

            }
            list.Insert(0, new SelectListItem
            {
                Text = "All Emails",
                Value = allEmails
            });

            return list;
        }

        /// <summary>
        /// Método que preenche a combobox para escolha dos roles
        /// </summary>
        /// <returns>Roles</returns>
        public IEnumerable<SelectListItem> GetComboRoles()
        {
            var model = new RegisterNewUserViewModel
            {
                Roles = new List<SelectListItem>
                {
                    new SelectListItem{Text = "Select the Role...",Value = "" },
                    new SelectListItem{Text = "Admin", Value = "Admin"},
                    new SelectListItem{Text = "Librarian", Value = "Librarian"},
                    new SelectListItem{Text = "Assistant", Value = "Assistant"},

                },
            };
            return model.Roles;
        }
    }
}
