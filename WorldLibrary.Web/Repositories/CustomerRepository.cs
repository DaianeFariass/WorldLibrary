using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorldLibrary.Web.Data;
using WorldLibrary.Web.Data.Entities;
using WorldLibrary.Web.Helper;

namespace WorldLibrary.Web.Repositories
{
    public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        public CustomerRepository(DataContext context,
            IUserHelper userHelper) : base(context)
        {
            _context = context;
            _userHelper = userHelper;
        }

        public IQueryable GetAllWithUsers()
        {
            return _context.Customers.Include(p => p.User);
        }

        /// <summary>
        /// Método que disponibiliza que os funcionários visualizem o Id cliente
        /// </summary>
        /// <param name="userName"></param>
        /// <returns>Customers</returns>
        public async Task<IQueryable<Customer>> GetCustomerAsync(string userName)
        {
            var user = await _userHelper.GetUserByEmailAsync(userName);
            if (user == null)
            {
                return null;
            }
            if (await _userHelper.IsUserInRoleAsync(user, "Admin") ||
                await _userHelper.IsUserInRoleAsync(user, "Librarian") ||
                await _userHelper.IsUserInRoleAsync(user, "Assistant"))
               
            {
                return _context.Customers.Include(p => p.User);
            }

            return _context.Customers
                    .Where(c => c.User == user)
                    .OrderByDescending(c => c.FullName);
        }

        /// <summary>
        /// Método que preenche o combobox dos clientes
        /// </summary>
        /// <returns>Customers</returns>
        public IEnumerable<SelectListItem> GetComboCustomers()
        {
            var list = _context.Customers.Select(c => new SelectListItem
            {
                Text = c.FullName,
                Value = c.Id.ToString()
            }).ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "(Select the Customer...)",
                Value = "0"
            });

            return list;
        }

        /// <summary>
        /// Método que preenche o combobox com o email dos clientes disponibilizado no External Contact Form
        /// </summary>
        /// <returns>Emails</returns>
        public IEnumerable<SelectListItem> GetComboCustomerLogged(string username)
        {
            var list = _context.Customers
                .Where(c => c.User.UserName == username)
                .Select(c => new SelectListItem
            {
                Text = c.FullName,
                Value = c.Id.ToString()
            }).ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "(Select the Customer...)",
                Value = "0"
            });

            return list;
        }

        /// <summary>
        /// Método preenche a combobox com o customer que esta logado
        /// </summary>
        /// <param name="username"></param>
        /// <returns>Customer</returns>

        public IEnumerable<SelectListItem> GetComboCustomersEmail()
        {
            var list = _context.Customers.Select(p => new SelectListItem
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
                Text = "Empty",
                Value = null
            });
            list.Insert(1, new SelectListItem
            {
                Text = "All Emails",
                Value = allEmails
            });

            return list;
        }
    }
}
