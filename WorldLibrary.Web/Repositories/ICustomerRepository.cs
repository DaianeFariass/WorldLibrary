using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorldLibrary.Web.Data.Entities;

namespace WorldLibrary.Web.Repositories
{
    public interface ICustomerRepository : IGenericRepository<Customer>
    {
        public IQueryable GetAllWithUsers();

        public Task<IQueryable<Customer>> GetCustomerAsync(string userName);

        IEnumerable<SelectListItem> GetComboCustomers();

        IEnumerable<SelectListItem> GetComboCustomerLogged(string username);

        IEnumerable<SelectListItem> GetComboCustomersEmail();
    }
}
