using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WorldLibrary.Web.Data;
using Microsoft.AspNetCore.Mvc;

namespace WorldLibrary.Web.Helper
{
    public class BlobHelper : IBlobHelper
    {
        private readonly CloudBlobClient _blobClient;
        private readonly IConfiguration _configuration;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly DataContext _context;

        public BlobHelper(IConfiguration configuration,
            IWebHostEnvironment webHostEnvironment,
            DataContext context)
        {
            string keys = configuration["Blob:ConnectionString"];
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(keys);
            _blobClient = storageAccount.CreateCloudBlobClient();
            _blobServiceClient = new BlobServiceClient(keys);
            _webHostEnvironment = webHostEnvironment;
            _context = context;
        }
        public async Task<Guid> UploadBlobAsync(IFormFile file, string containerName)
        {
            Stream stream = file.OpenReadStream();
            return await UploadStreamAsync(stream, containerName);
        }

        public async Task<Guid> UploadBlobAsync(byte[] file, string containerName)
        {
            MemoryStream stream = new MemoryStream(file);
            return await UploadStreamAsync(stream, containerName);
        }

        public async Task<Guid> UploadBlobAsync(string image, string containerName)
        {
            Stream stream = File.OpenRead(image);
            return await UploadStreamAsync(stream, containerName);
        }

        private async Task<Guid> UploadStreamAsync(Stream stream, string containerName)
        {
            Guid name = Guid.NewGuid();
            CloudBlobContainer container = _blobClient.GetContainerReference(containerName);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference($"{name}");
            await blockBlob.UploadFromStreamAsync(stream);
            return name;
        }
        //criar
        public async Task<byte[]> DownloadBlobPdfAsString(int id)
        {
            var book = _context.Books.FirstOrDefault(x => x.Id == id);

            if (book != null)
            {
               
                string guid = book.ImagePdf.ToString();
                string containerName = "pdfs"; // Substitua com o nome do seu contêiner no Azure Blob Storage

                // Faz o download do PDF do Azure Blob Storage usando o GUID e o nome do contêiner
                byte[] pdfBytes = await DownloadPdfFromAzureBlob(Guid.Parse(guid), containerName);

                return pdfBytes;
               
            }
            return null;


        }
        //criar
        public async Task<byte[]> DownloadPdfFromAzureBlob(Guid guid, string containerName)
        {

            // Obtém uma referência para o contêiner no qual o PDF está armazenado
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

            // Obtém uma referência para o blob usando o GUID como nome do blob
            var blobClient = containerClient.GetBlobClient($"{guid}");

            // Faz o download do blob como um array de bytes
            BlobDownloadInfo blobDownloadInfo = await blobClient.DownloadAsync();

            // Lê o conteúdo do blob como um array de bytes
            using (MemoryStream memoryStream = new MemoryStream())
            {
                await blobDownloadInfo.Content.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }

}

