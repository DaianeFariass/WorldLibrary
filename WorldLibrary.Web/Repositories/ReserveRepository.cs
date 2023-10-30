using WorldLibrary.Web.Data.Entities;
using WorldLibrary.Web.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WorldLibrary.Web.Models;
using System;
using WorldLibrary.Web.Helper;
using Vereyon.Web;
using WorldLibrary.Web.Enums;

namespace WorldLibrary.Web.Repositories
{
    public class ReserveRepository : GenericRepository<Reserve>, IReserveRepository
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private readonly IBookRepository _bookRepository;
        private readonly IPhysicalLibraryRepository _physicalLibraryRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IFlashMessage _flashMessage;
        private readonly IConverterHelper _converterHelper;
        public ReserveRepository(DataContext context,
            IUserHelper userHelper,
            IBookRepository bookRepository,
            IFlashMessage flashMessage,
            IConverterHelper converterHelper,
            IPhysicalLibraryRepository physicalLibraryRepository,
            ICustomerRepository customerRepository) : base(context)
        {
            _context = context;
            _userHelper = userHelper;
            _bookRepository= bookRepository;
            _physicalLibraryRepository= physicalLibraryRepository;
            _customerRepository= customerRepository;
            _flashMessage = flashMessage;
            _converterHelper=converterHelper;
        }

        /// <summary>
        /// Método que adiciona a reserva 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="username"></param>
        /// <returns>Reserva</returns>
        public async Task AddItemReserveAsync(AddReserveViewModel model, string username)
        {
            var user = await _userHelper.GetUserByEmailAsync(username);
            if (user == null)
            {
                return;
            }
            var library = await _context.PhysicalLibraries.FindAsync(model.LibraryId); //Criar
            if (library == null)
            {
                return;
            }
            var reserve = await _context.Books.FindAsync(model.BookId);
            if (reserve == null)
            {
                return;
            }
            var customer = await _context.Customers.FindAsync(model.CustomerId);
            if (customer == null)
            {
                return;
            }
            var reserveDetailTemp = await _context.ReserveDetailsTemp
            .Where(rdt => rdt.User == user && rdt.Book == reserve && rdt.Customer == customer && rdt.PhysicalLibrary == library)
            .FirstOrDefaultAsync();
            if (reserveDetailTemp == null)
            {
                reserveDetailTemp = new ReserveDetailTemp
                {
                    PhysicalLibrary = library, 
                    Customer = customer,
                    Book = reserve,
                    Quantity = model.Quantity,
                    User = user,
                };
                _context.ReserveDetailsTemp.Add(reserveDetailTemp);
            }
            else
            {
                reserveDetailTemp.Quantity += model.Quantity;
                _context.ReserveDetailsTemp.Update(reserveDetailTemp);
            }
            await _context.SaveChangesAsync();

        }

        /// <summary>
        /// Método que adiciona o book a reserva 
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Books</returns>
        public async Task<Reserve> BookReturnAsync(BookReturnViewModel model)
        {
            var reserve = await _context.Reserves.FindAsync(model.Id);
            if (reserve == null)
            {
                return null;
            }
            var book = await _context.Books.FindAsync(model.BookId);
            if (book == null)
            {
                return null;
            }
            var reserves = _context.Reserves
               .Include(r => r.PhysicalLibrary)
               .Include(r => r.Book)
               .Include(r => r.User)
               .Include(r => r.Customer)
               .ToList();

            
           
            var details = reserves.Select(r => new ReserveViewModel
            {
                PhysicalLibrary = r.PhysicalLibrary,
                Customer = r.Customer,
                Book = r.Book,
                User = r.User
            });

            reserve.Book= book;
            reserve.ActualReturnDate = model.ActualReturnDate;
            reserve.Rate = model.Rate;
            reserve.ReturnDate = model.ReturnDate;

            if (model.Quantity == reserve.Quantity)
            {
                book.Quantity += (double)model.Quantity;
                reserve.Quantity -= (double)model.Quantity;
                reserve.StatusReserve = StatusReserve.Concluded;

            }
            double sub;
            if (model.Quantity < reserve.Quantity)
            {
                sub = reserve.Quantity -(double)model.Quantity;
                book.Quantity += sub;
                reserve.Quantity -= sub;
                if (reserve.Quantity > 1)
                {
                    reserve.StatusReserve = StatusReserve.Renewed;
                }

            }

            _context.Reserves.Update(reserve);
            await _context.SaveChangesAsync();
            return (reserve);

        }

        /// <summary>
        /// ´Método que cancela a reserva 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="username"></param>
        /// <returns>Reserva</returns>
        public async Task<Reserve> CancelReserveAsync(int id, string username)
        {
            var reserve = await _context.Reserves.FindAsync(id);
            if (reserve == null)
            {
                return null;

            }
            if (reserve.StatusReserve == StatusReserve.Cancelled)
            {
                return null;
            }
            var user = await _userHelper.GetUserByEmailAsync(username);
            if (user == null)
            {
                return null;
            }

            var customers = _context.Reserves
                .Include(r => r.PhysicalLibrary)
                .Include(r => r.Book)
                .Include(r => r.User)
                .Include(r => r.Customer)
                .ToList();

            var details = customers.Select(r => new ReserveViewModel
            {
                PhysicalLibrary = reserve.PhysicalLibrary,
                Book = reserve.Book,
                User = reserve.User,
                Customer = reserve.Customer,
                Quantity= reserve.Quantity,
                StatusReserve = StatusReserve.Cancelled

            });
            var book = _context.Books.FindAsync(reserve.Book.Id);
            book.Result.Quantity += reserve.Quantity;
            reserve.StatusReserve = StatusReserve.Cancelled;
            await SendReserveNotification(reserve, user.UserName, NotificationType.Cancel);
            _context.Reserves.Update(reserve);
            _context.Books.Update(book.Result);
            await _context.SaveChangesAsync();

            return reserve;
        }

        /// <summary>
        /// Método que confirma a reserva 
        /// </summary>
        /// <param name="userName"></param>
        /// <returns>Reserva</returns>

        public async Task<Reserve> ConfirmReservAsync(string userName)
        {
            var user = await _userHelper.GetUserByEmailAsync(userName);
            if (user == null)
            {
                return null;
            }
            var reserveTemp = await _context.ReserveDetailsTemp
                .Include(r => r.PhysicalLibrary)
                .Include(r => r.Book)
                .Include(r => r.Customer)
                .Where(r => r.User == user)
                .ToListAsync();
            if (reserveTemp == null || reserveTemp.Count == 0)
            {
                return null;
            }

            var details = reserveTemp.Select(r => new ReserveDetail
            {
                PhysicalLibrary = r.PhysicalLibrary,
                Customer = r.Customer,
                Book = r.Book,
                Quantity = r.Quantity,
            }).ToList();
            var reserve = new Reserve();
            foreach (ReserveDetail reserveDetail in details)
            {
                reserve = new Reserve
                {
                    PhysicalLibrary = reserveDetail.PhysicalLibrary,
                    Customer = reserveDetail.Customer,
                    Book = reserveDetail.Book,
                    BookingDate = DateTime.Now,
                    User = user,
                    Quantity = reserveDetail.Quantity,
                    StatusReserve = StatusReserve.Active


                };
                await CreateAsync(reserve);
                await SendReserveNotification(reserve, user.UserName, NotificationType.Create);

            }
            _context.ReserveDetailsTemp.RemoveRange(reserveTemp);
            await _context.SaveChangesAsync();

            return reserve;
        }

        /// <summary>
        /// Métyodod que deleta a reserva temporária
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Reserva Temporária</returns>
        public async Task DeleteDetailTempAsync(int id)
        {
            var reserveDetailTemp = await _context.ReserveDetailsTemp.FindAsync(id);
            if (reserveDetailTemp == null)
            {
                return;
            }
            var reserveDetailTemps = await _context.ReserveDetailsTemp
                .Include(r => r.Book)
                .ToListAsync();
            var reserve = reserveDetailTemps.Select(r => new ReserveViewModel
            {
                Book = r.Book

            }); ;
            var book = _context.Books.FindAsync(reserveDetailTemp.Book.Id);
            book.Result.Quantity += reserveDetailTemp.Quantity;
            _context.ReserveDetailsTemp.Remove(reserveDetailTemp);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Métodod que adicona a data de entrega do livro
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Data</returns>
        public async Task<Reserve> DeliverReserveAsync(DeliveryViewModel model)
        {

            var reserve = await _context.Reserves.FindAsync(model.Id);

            if (reserve == null)
            {
                return null;
            }

            var reserves = _context.Reserves
                .Include(r => r.PhysicalLibrary)
                .Include(r => r.Book)
                .Include(r => r.User)
                .Include(r => r.Customer)
                .ToList();

            var details = reserves.Select(r => new ReserveViewModel
            {
                PhysicalLibrary= r.PhysicalLibrary,
                Customer = r.Customer,
                Book = r.Book,
                User = r.User
            });

            reserve.DeliveryDate = model.DeliveryDate;
            reserve.ReturnDate = model.ReturnDate;
            reserve.StatusReserve = StatusReserve.Active;

            _context.Reserves.Update(reserve);
            await _context.SaveChangesAsync();
            return reserve;
        }

        /// <summary>
        /// Método que edita e atualiza a reserva
        /// </summary>
        /// <param name="model"></param>
        /// <param name="username"></param>
        /// <returns>Reserva<returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<Reserve> EditReserveAsync(ReserveViewModel model, string username)
        {
            _converterHelper.ToReserve(model, false);
            var user = await _userHelper.GetUserByEmailAsync(username);
            if (user == null)
            {
                throw new NotImplementedException();

            }
            var library = await _context.PhysicalLibraries.FindAsync(model.LibraryId);
            if (library == null)
            {
                throw new NotImplementedException();
            }
            var book = await _context.Books.FindAsync(model.BookId);
            if (book == null)
            {
                throw new NotImplementedException();
            }
            var reserve = _context.Reserves.FindAsync(model.Id);
            double sub;

            if (model.Quantity < reserve.Result.Quantity)
            {
                sub = reserve.Result.Quantity - model.Quantity;
                book.Quantity += sub;
                book.StatusBook = StatusBook.Available;


            }
            if (model.Quantity > reserve.Result.Quantity)
            {
                sub = model.Quantity - reserve.Result.Quantity;
                book.Quantity -= sub;
                if (book.Quantity == 0)
                {
                    book.StatusBook = StatusBook.Unvailable;

                }
                else
                {
                    book.StatusBook = StatusBook.Available;


                }

            }
            if (model.Quantity == book.Quantity)
            {
                book.Quantity -= model.Quantity;
                book.StatusBook = StatusBook.Unvailable;
            }

            var customer = await _context.Customers.FindAsync(model.CustomerId);
            if (customer == null)
            {
                throw new NotImplementedException();
            }
            var reserves = _context.Reserves
                .Include(r => r.PhysicalLibrary)
                .Include(r => r.Book)
                .Include(r => r.Customer)
                .Include(r => r.User)
                .Where(r => r.User == user)
                .ToList();

            var details = reserves.Select(r => new ReserveViewModel
            {
                Id = r.Id,
                PhysicalLibrary = library,
                Book = book,
                Customer = customer,
                User= user

            });

            reserve.Result.Id = model.Id;
            reserve.Result.User= user;
            reserve.Result.PhysicalLibrary = library;
            reserve.Result.Book= book;
            reserve.Result.Customer= customer;
            reserve.Result.Quantity = model.Quantity;
            reserve.Result.BookingDate = DateTime.Now.Date;
            reserve.Result.StatusReserve = StatusReserve.Active;


            _context.Reserves.Update(reserve.Result);
            _context.Books.Update(book);
            await SendReserveNotification(reserve.Result, user.UserName, NotificationType.Edit);
            await _context.SaveChangesAsync();
            return reserve.Result;
        }


        /// <summary>
        /// Método que edita a reserva temporária
        /// </summary>
        /// <param name="model"></param>
        /// <param name="username"></param>
        /// <returns>Rerserva Temporária</returns>
        public async Task EditReserveDetailTempAsync(AddReserveViewModel model, string username)
        {
            var user = await _userHelper.GetUserByEmailAsync(username);
            if (user == null)
            {
                return;

            }
            var library = await _context.PhysicalLibraries.FindAsync(model.LibraryId);
            if (library == null)
            {
                return;
            }
            var book = await _context.Books.FindAsync(model.BookId);
            if (book == null)
            {
                return;
            }
            var customer = await _context.Customers.FindAsync(model.CustomerId);
            if (customer == null)
            {
                return;
            }
            var reserveDetailTemp = await _context.ReserveDetailsTemp.FindAsync(model.Id);

            reserveDetailTemp.PhysicalLibrary = library;
            reserveDetailTemp.Customer = customer;
            reserveDetailTemp.User= user;
            reserveDetailTemp.Book= book;
            reserveDetailTemp.Quantity= model.Quantity;

            _context.ReserveDetailsTemp.Update(reserveDetailTemp);

            await _context.SaveChangesAsync();
        }

        public IQueryable GetAllWithUsers()
        {
            return _context.Reserves
               .Include(r => r.User)
               .Include(b => b.Book)
               .Include(c => c.Customer);
        }

        public DateTime GetBookingDate()
        {
            return DateTime.Now;
        }

        public DateTime GetDeliveryDate()
        {
            return DateTime.Now;
        }

        public async Task<IQueryable<ReserveDetailTemp>> GetDetailsTempAsync(string userName)
        {
            var user = await _userHelper.GetUserByEmailAsync(userName);
            if (user == null)
            {
                return null;
            }
            return _context.ReserveDetailsTemp
                .Include(r => r.PhysicalLibrary)
                .Include(r => r.Book)
                .Include(r => r.Customer)
                .Where(r => r.User == user)
                .OrderBy(r => r.Customer.FullName);
        }

        /// <summary>
        /// Método que busca a reserva pelo Id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Reserva</returns>
        public async Task<IQueryable<Reserve>> GetReserveAsync(string userName)
        {
            var user = await _userHelper.GetUserByEmailAsync(userName);
            if (user == null)
            {
                return null;
            }
            if (await _userHelper.IsUserInRoleAsync(user, "Admin") ||
                await _userHelper.IsUserInRoleAsync(user, "Employee"))

            {
                return _context.Reserves
                    .Include(r => r.PhysicalLibrary)
                    .Include(b => b.User)
                    .Include(c => c.Customer)
                    .Include(i => i.Book)
                    .OrderByDescending(m => m.DeliveryDate);
            }

            return _context.Reserves
                    .Include(r => r.PhysicalLibrary)
                    .Include(r=>r.Customer)
                    .Include(i => i.Book)
                    .Where(r => r.User == user)
                    .OrderByDescending(m => m.DeliveryDate);
        }

        /// <summary>
        /// Método que busca a reserva pelo Id incluindo o livro
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Reserva</returns>
        public async Task<Reserve> GetReserveAsync(int id)
        {
            return await _context.Reserves.FindAsync(id);
        }


        /// <summary>
        /// Método que busca a reserva temporária pelo Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Reserva Temporaria</returns>
        public async Task<Reserve> GetReserveByIdAsync(int id)
        {
            await _context.Reserves.FindAsync(id);
            return _context.Reserves
                .Include(r => r.Book)
                .Where(r => r.Id == id)
                .FirstOrDefault();
        }

        /// <summary>
        /// Método que busca a reserva temporária pelo Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Reserva Temporaria</returns>
        public async Task<ReserveDetailTemp> GetReserveDetailTempAsync(int id)
        {
            return await _context.ReserveDetailsTemp.FindAsync(id);
        }


        /// <summary>
        /// Método que faz a renovação do livro
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Data</returns>
        public async Task<Reserve> RenewBookReturnAsync(BookReturnViewModel model)
        {
            var reserve = await _context.Reserves.FindAsync(model.Id);
            if (reserve == null)
            {
                return null;
            }
            var reserves = _context.Reserves
               .Include(r => r.PhysicalLibrary)
               .Include(r => r.Book)
               .Include(r => r.User)
               .Include(r => r.Customer)
               .ToList();

            var details = reserves.Select(r => new ReserveViewModel
            {
                PhysicalLibrary = r.PhysicalLibrary,
                Customer = r.Customer,
                Book = r.Book,
                User = r.User
            });

            reserve.ReturnDate = model.ReturnDate;
            _context.Reserves.Update(reserve);
            await _context.SaveChangesAsync();
            return (reserve);
        }

        /// <summary>
        /// Método que envia a notificação da reserva 
        /// </summary>
        /// <param name="reserve"></param>
        /// <param name="username"></param>
        /// <param name="notificationType"></param>
        /// <returns>notificação</returns>
        public async Task SendReserveNotification(Reserve reserve, string username, NotificationType notificationType)
        {
            var user = await _userHelper.GetUserByEmailAsync(username);
            if (user == null)
            {
                return;

            }
        
            var notification = new Notification
            {
                Reserve = reserve,
                NotificationType= notificationType,

            };

            _context.Notifications.Add(notification);

            await _context.SaveChangesAsync();
        }


        /// <summary>
        /// Método que preenche o combobox apenas customer que esta logado
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<AddReserveViewModel> ReturnReserveViewModel(string userName)
        {

            var user = await _userHelper.GetUserByEmailAsync(userName);

            bool role = await _userHelper.IsUserInRoleAsync(user, "Customer");
            if (role)
            {
                var view = new AddReserveViewModel
                {
                    Libraries = _physicalLibraryRepository.GetComboLibraries(),
                    Customers = _customerRepository.GetComboCustomerLogged(userName),
                    Books = _bookRepository.GetComboBooks(),

                };
                return view;



            }

            var model = new AddReserveViewModel
            {
                Libraries = _physicalLibraryRepository.GetComboLibraries(),
                Customers = _customerRepository.GetComboCustomers(),
                Books = _bookRepository.GetComboBooks(),



            };
            return model;
        }

    }
}
