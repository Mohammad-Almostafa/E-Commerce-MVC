using Microsoft.AspNetCore.Http;

namespace myshop.Business.Services.Interfaces
{
    public interface IFileService
    {
        string UploadFile(IFormFile file, string folderName);

        void DeleteFile(string relativePath);
    }
}