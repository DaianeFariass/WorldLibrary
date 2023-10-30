using System;
using WorldLibrary.Web.Data.Entities;
using WorldLibrary.Web.Models;
using static System.Net.Mime.MediaTypeNames;

namespace WorldLibrary.Web.Helper
{
    public interface IConverterHelper
    {
        Customer ToCustomer(CustomerViewModel model, Guid imageId, bool isNew);
        CustomerViewModel ToCustomerViewModel(Customer customer);
        Book ToBook(BookViewModel model, Guid imageId, Guid imagePdf, bool isNew);
        BookViewModel ToBookViewModel(Book book);
        Employee ToEmployee(EmployeeViewModel model, Guid imageId, bool isNew);
        EmployeeViewModel ToEmployeeViewModel(Employee employee);
        PhysicalLibrary ToLibrary(PhysicalLibraryViewModel model, Guid imageId, bool isNew);   
        PhysicalLibraryViewModel ToPhysicalLibraryViewModel(PhysicalLibrary physicalLibrary);
        Reserve ToReserve(ReserveViewModel model, bool isNew);
    }
}
