using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using myshop.Business.Services.Interfaces; 
using System;
using System.IO;

namespace myshop.Business.Services.Implementations 
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FileService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public string UploadFile(IFormFile file, string folderName)
        {
            if (file == null) return null;

            string rootPath = _webHostEnvironment.WebRootPath;
            string filename = Guid.NewGuid().ToString();
            var uploadPath = Path.Combine(rootPath, "Images", folderName);
            var ext = Path.GetExtension(file.FileName);

            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            using (var filestream = new FileStream(Path.Combine(uploadPath, filename + ext), FileMode.Create))
            {
                file.CopyTo(filestream);
            }

            return Path.Combine("Images", folderName, filename + ext);
        }

        public void DeleteFile(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath)) return;

            string fullPath = Path.Combine(_webHostEnvironment.WebRootPath, relativePath.TrimStart('\\', '/'));

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }
    }
}