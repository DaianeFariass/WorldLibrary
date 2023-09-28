using System;
using WorldLibrary.Web.Data.Entities;
using WorldLibrary.Web.Models;
using static System.Net.Mime.MediaTypeNames;

namespace WorldLibrary.Web.Helper
{
    public interface IConverterHelper
    {
        Book ToBook(BookViewModel model, Guid imageId, bool isNew);
        Employee ToEmployee(EmployeeViewModel model, Guid imageId, bool isNew);
        PhysicalLibrary ToLibrary(PhysicalLibraryViewModel model, Guid imageId, bool isNew);
        BookViewModel ToBookViewModel(Book book);
        EmployeeViewModel ToEmployeeViewModel(Employee employee);
        PhysicalLibraryViewModel ToPhysicalLibraryViewModel(PhysicalLibrary physicalLibrary);
        Reserve ToReserve(ReserveViewModel model, bool isNew);
    }
}
