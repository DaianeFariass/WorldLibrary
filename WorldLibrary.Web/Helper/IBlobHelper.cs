using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;

namespace WorldLibrary.Web.Helper
{
    public interface IBlobHelper
    {
        Task<Guid> UploadBlobAsync(IFormFile file, string containerName);

        Task<Guid> UploadBlobAsync(byte[] file, string containerName);

        Task<Guid> UploadBlobAsync(string image, string containerName);

        //criar
        Task<byte[]> DownloadBlobPdfAsString(int id);


    }
}
