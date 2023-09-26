using System;
using WorldLibrary.Web.Data.Entities;
using WorldLibrary.Web.Models;

namespace WorldLibrary.Web.Helper
{
    public class ConverterHelper : IConverterHelper
    {
        public Book ToBook(BookViewModel model, string path, bool isNew)
        {
            return new Book
            {
                Id = isNew ? 0 : model.Id,
                ImageUrl = path,
                Title = model.Title,
                Author = model.Author,
                Category=model.Category,
                Synopsis=model.Synopsis,
                Year=model.Year,
                User=model.User,
            };
        }

        public BookViewModel ToBookViewModel(Book book)
        {
            return new BookViewModel
            {
                Id=book.Id,
                Title = book.Title,
                Author=book.Author,
                Category=book.Category,
                ImageUrl=book.ImageUrl,
                Synopsis=book.Synopsis,
                Year=book.Year,
                User = book.User,
            };
        }

        public Employee ToEmployee(EmployeeViewModel model, string path, bool isNew)
        {
            return new Employee
            {
                Id = isNew ? 0 : model.Id,
                ImageUrl = path,
                Address=model.Address,
                CellPhone=model.CellPhone,
                Document=model.Document,
                Email=model.Email,
                FullName=model.FullName,
                JobPosition=model.JobPosition,
                User=model.User,
            };
        }

        public EmployeeViewModel ToEmployeeViewModel(Employee employee)
        {
            return new EmployeeViewModel
            {
                Id=employee.Id,
                FullName=employee.FullName,
                Address=employee.Address,
                CellPhone=employee.CellPhone,
                Document=employee.Document,
                Email=employee.Email,
                ImageUrl=employee.ImageUrl,
                JobPosition=employee.JobPosition,
                User = employee.User,
            };
        }

        public PhysicalLibrary ToLibrary(PhysicalLibraryViewModel model, string path, bool isNew)
        {
            return new PhysicalLibrary
            {
                Id = isNew ? 0 : model.Id,
                ImageUrl= path,
                Country =model.Country,
                Email=model.Email,
                Name= model.Name,
                PhoneNumber= model.PhoneNumber,
                User= model.User,
            };
        }

        public PhysicalLibraryViewModel ToPhysicalLibraryViewModel(PhysicalLibrary physicalLibrary)
        {
            return new PhysicalLibraryViewModel
            {
                Id = physicalLibrary.Id,
                Country=physicalLibrary.Country,
                Name=physicalLibrary.Name,
                Email=physicalLibrary.Email,
                PhoneNumber=physicalLibrary.PhoneNumber,
                ImageUrl=physicalLibrary.ImageUrl,
                User=physicalLibrary.User,
            };
        }

        public Reserve ToReserve(ReserveViewModel model, bool isNew)
        {
            return new Reserve
            {
                Id = isNew ? 0 : model.Id,
                User= model.User,
                Book = model.Book,
                Quantity= model.Quantity,
                BookingDate = DateTime.Now.Date,


            };
        }
    }
}
