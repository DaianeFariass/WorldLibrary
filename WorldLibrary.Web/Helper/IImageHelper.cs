using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace WorldLibrary.Web.Helper
{
    public interface IImageHelper
    {
        Task<string> UploadImageAsync(IFormFile imageFile, string folder);
    }
}
