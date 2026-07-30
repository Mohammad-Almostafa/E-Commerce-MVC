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
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            // تحديد مسار التخزين (مثل: wwwroot/uploads/products)
            var uploadPath = Path.Combine(rootPath, "uploads", folderName);

            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            using (var filestream = new FileStream(Path.Combine(uploadPath, filename + ext), FileMode.Create))
            {
                file.CopyTo(filestream);
            }

            return Path.Combine("uploads", folderName, filename + ext).Replace("\\", "/");
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

        public (bool IsValid, string ErrorMessage) ValidateImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return (true, string.Empty); // الصورة اختيارية في بعض الحالات

            // 1. Check File Size (Max 2 MB = 2 * 1024 * 1024 bytes)
            if (file.Length > 2 * 1024 * 1024)
            {
                return (false, "File size must be less than 2 MB.");
            }

            // 2. Check Extension
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (Array.IndexOf(allowedExtensions, extension) < 0)
            {
                return (false, "Invalid file format. Only JPG, JPEG, PNG, and WEBP files are allowed.");
            }

            return (true, string.Empty);
        }
    }
}