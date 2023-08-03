using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using WorldLibrary.Web.Data.Entities;

namespace WorldLibrary.Web.Helper
{
    public interface IUserHelper
    {
        Task<User> GetUserByEmailAsync(string email);

        Task<IdentityResult> AddUserAsync(User user, string password);
    }
}
