using Homework.Business.Models;
using Homework.Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace Homework.Controllers
{
    [Route("api/[controller]")]
    public class HomeworkController : Controller
    {
        private readonly IHomeworkService _homeworkService;
        public HomeworkController(IHomeworkService homeworkService)
        {
            _homeworkService = homeworkService;
        }
        
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file, FileExtension fileExtension)
        {
            if (file == null)
            {
                return BadRequest();
            }

            var document = await _homeworkService.UploadAndSave(file, fileExtension);

            return File(document.FileData, document.MimeType, document.FileName);
        }

        [HttpPost("sendEmail")]
        public async Task<string> SendEmail(IFormFile file, FileExtension fileExtension, string recipient)
        {
            if (file == null)
            {
                return "No file was chosen";
            }

            if(string.IsNullOrEmpty(recipient))
            {
                return "No recepient was specified";
            }

            return await _homeworkService.UploadAndSendEmail(file, fileExtension, recipient);
        }
        
        [HttpPost("convertFromUrl")]
        public async Task<IActionResult> ConvertFromUrl(string url, FileExtension fileExtension)
        {
            if(string.IsNullOrEmpty(url))
            {
                return BadRequest();
            }

            var document = await _homeworkService.ConvertFromUrl(url, fileExtension);

            return File(document.FileData, document.MimeType, document.FileName);
        }
        
    }
}
