using WorldLibrary.Web.Data.Entities;
using WorldLibrary.Web.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WorldLibrary.Web.Models;
using System;
using WorldLibrary.Web.Helper;
using Vereyon.Web;

namespace WorldLibrary.Web.Repositories
{
    public class ReserveRepository : GenericRepository<Reserve>, IReserveRepository
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private readonly IBookRepository _bookRepository;
        private readonly IFlashMessage _flashMessage;
        public ReserveRepository(DataContext context,
            IUserHelper userHelper,
            IBookRepository bookRepository,
            IFlashMessage flashMessage) : base(context)
        {
            _context = context;
            _userHelper = userHelper;
            _bookRepository= bookRepository;
            _flashMessage = flashMessage;
        }

        public async Task AddItemReserveAsync(AddReserveViewModel model, string username)
        {
            var user = await _userHelper.GetUserByEmailAsync(username);
            if (user == null)
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
            .Where(rdt => rdt.User == user && rdt.Book == reserve && rdt.Customer == customer)
            .FirstOrDefaultAsync();
            if (reserveDetailTemp == null)
            {
                reserveDetailTemp = new ReserveDetailTemp
                {
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

        public Task BookReturnAsync(BookReturnViewModel model)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CancelReserveAsync(BookReturnViewModel model)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> ConfirmReservAsync(string userName)
        {
            var user = await _userHelper.GetUserByEmailAsync(userName);
            if (user == null)
            {
                return false;
            }
            var reserveTemp = await _context.ReserveDetailsTemp
                .Include(r => r.Book)
                .Where(r => r.User == user)
                .ToListAsync();
            if (reserveTemp == null || reserveTemp.Count == 0)
            {
                return false;
            }

            var details = reserveTemp.Select(r => new ReserveDetail
            {
                Book = r.Book,
                //BookingDate = r.BookingDate,
                //DeliveryDate = r.DeliveryDate,
                Quantity = r.Quantity,
            }).ToList();

            var reserve = new Reserve
            {
                BookingDate = DateTime.Now,
                User = user,
                //Items = details
            };
            await CreateAsync(reserve);
            _context.ReserveDetailsTemp.RemoveRange(reserveTemp);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task DeleteDetailTempAsync(int id)
        {
            var reserveDetailTemp = await _context.ReserveDetailsTemp.FindAsync(id);
            if (reserveDetailTemp == null)
            {
                return;

            }
            _context.ReserveDetailsTemp.Remove(reserveDetailTemp);
            await _context.SaveChangesAsync();
        }
        
        public async Task DeliverReserve(DeliveryViewModel model)
        {
            var reserve = await _context.Reserves.FindAsync(model.Id);
            if (reserve == null)
            {
                return;
            }
            reserve.DeliveryDate = model.DeliveryDate;
            _context.Reserves.Update(reserve);
            await _context.SaveChangesAsync();
        }

        public Task DeliverReserveAsync(DeliveryViewModel model)
        {
            throw new NotImplementedException();
        }

        public async Task EditReserveDetailTempAsync(AddReserveViewModel model, string username)
        {
            var user = await _userHelper.GetUserByEmailAsync(username);
            if (user == null)
            {
                return;

            }
            var book = await _context.Books.FindAsync(model.BookId);
            if (book == null)
            {
                return;
            }
            var reserveDetailTemp = await _context.ReserveDetailsTemp.FindAsync(model.Id);

            reserveDetailTemp.User= user;
            reserveDetailTemp.Book= book;
            //reserveDetailTemp.BookingDate = model.BookDate;
            //reserveDetailTemp.DeliveryDate= model.DeliveryDate;
            reserveDetailTemp.Quantity= model.Quantity;

            _context.ReserveDetailsTemp.Update(reserveDetailTemp);

            await _context.SaveChangesAsync();
        }

        public IQueryable GetAllWithUsers()
        {
            return _context.PhysicalLibraries.Include(p => p.User);
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
                .Include(r => r.Book)
                .Where(r => r.User == user)
                .OrderBy(r => r.Book.Title);
        }

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
                    .Include(b => b.User)
                    .Include(c => c.Customer)
                    .Include(i => i.Book)
                    .OrderByDescending(m => m.DeliveryDate);
            }

            return _context.Reserves
                    .Include(i => i.Book)
                    .Where(r => r.User == user)
                    .OrderByDescending(m => m.DeliveryDate);
        }

        public async Task<Reserve> GetReserveAsync(int id)
        {
            return await _context.Reserves.FindAsync(id);
        }

        public async Task<ReserveDetailTemp> GetReserveDetailTempAsync(int id)
        {
            return await _context.ReserveDetailsTemp.FindAsync(id);
        }

        public async Task ModifyReserveDetailTempQuantityAsync(int id, double quantity)
        {
            var orderDetailTemp = await _context.ReserveDetailsTemp.FindAsync(id);
            if (orderDetailTemp == null)
            {
                return;
            }
            orderDetailTemp.Quantity += quantity;
            if (orderDetailTemp.Quantity > 0)
            {
                _context.ReserveDetailsTemp.Update(orderDetailTemp);
                await _context.SaveChangesAsync();

            }

        }   
    }
}
