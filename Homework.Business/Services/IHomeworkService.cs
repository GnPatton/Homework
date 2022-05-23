using Homework.Business.Models;
using Microsoft.AspNetCore.Http;

namespace Homework.Business.Services
{
    public interface IHomeworkService
    {
        Task<DocumentDto> UploadAndSave(IFormFile file, FileExtension fileExtension);
        Task<string> UploadAndSendEmail(IFormFile file, FileExtension fileExtension, string recipient);
        Task<DocumentDto> ConvertFromUrl(string url, FileExtension fileExtension);

    }
}
