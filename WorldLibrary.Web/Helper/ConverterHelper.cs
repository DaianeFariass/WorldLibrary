using System;
using WorldLibrary.Web.Data.Entities;
using WorldLibrary.Web.Models;

namespace WorldLibrary.Web.Helper
{
    public class ConverterHelper : IConverterHelper
    {
        public Customer ToCustomer(CustomerViewModel model, Guid imageId, bool isNew)
        {
            return new Customer
            {
                Id = isNew ? 0 : model.Id,
                FullName = model.FullName,
                Address = model.Address,
                Phone = model.Phone,
                Document = model.Document,
                Email = model.Email,
                ImageId = imageId,
                User = model.User,
            };

        }
        public CustomerViewModel ToCustomerViewModel(Customer customer)
        {
            return new CustomerViewModel 
            {
                Id = customer.Id,
                FullName = customer.FullName,
                Address = customer.Address,
                Phone = customer.Phone,
                Document = customer.Document,
                Email = customer.Email,
                ImageId = customer.ImageId,
                User = customer.User,
            };

        }
        public Book ToBook(BookViewModel model, Guid imageId, Guid imagePdf, bool isNew)
        {
            return new Book
            {
                Id = isNew ? 0 : model.Id,
                ImageId = imageId,
                Title = model.Title,
                Author = model.Author,
                Category=model.Category,
                Synopsis=model.Synopsis,
                Year=model.Year,
                StatusBook=model.StatusBook,
                Quantity=model.Quantity,
                Assessment=model.AssessmentId.ToString(),
                User=model.User,
                ImagePdf = imagePdf
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
                ImageId=book.ImageId,
                Synopsis=book.Synopsis,
                Year=book.Year,
                StatusBook=book.StatusBook,
                Quantity = book.Quantity,
                User = book.User,
                ImagePdf = book.ImagePdf
            };
        }

        public Employee ToEmployee(EmployeeViewModel model, Guid imageid, bool isNew)
        {
            return new Employee
            {
                Id = isNew ? 0 : model.Id,
                ImageId = imageid,
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
                ImageId=employee.ImageId,
                JobPosition=employee.JobPosition,
                User = employee.User,
            };
        }

        public PhysicalLibrary ToLibrary(PhysicalLibraryViewModel model, Guid imageid, bool isNew)
        {
            return new PhysicalLibrary
            {
                Id = isNew ? 0 : model.Id,
                ImageId= imageid,
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
                ImageId=physicalLibrary.ImageId,
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
