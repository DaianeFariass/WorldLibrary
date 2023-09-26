using WorldLibrary.Web.Data.Entities;
using WorldLibrary.Web.Models;

namespace WorldLibrary.Web.Helper
{
    public interface IConverterHelper
    {
        Book ToBook(BookViewModel model, string path, bool isNew);
        Employee ToEmployee(EmployeeViewModel model, string path, bool isNew);
        PhysicalLibrary ToLibrary(PhysicalLibraryViewModel model, string path, bool isNew);
        BookViewModel ToBookViewModel(Book book);
        EmployeeViewModel ToEmployeeViewModel(Employee employee);
        PhysicalLibraryViewModel ToPhysicalLibraryViewModel(PhysicalLibrary physicalLibrary);
        Reserve ToReserve(ReserveViewModel model, bool isNew);
    }
}
