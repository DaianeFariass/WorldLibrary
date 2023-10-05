using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using WorldLibrary.Web.Data.Entities;

namespace WorldLibrary.Web.Repositories
{
    public interface ICustomerRepository : IGenericRepository<Customer>
    {
        public IQueryable GetAllWithUsers();

        IEnumerable<SelectListItem> GetComboCustomers();
        IEnumerable<SelectListItem> GetComboCustomersEmail();
    }
}
