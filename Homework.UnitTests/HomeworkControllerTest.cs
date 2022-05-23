using Homework.Business.Models;
using Homework.Business.Services;
using Homework.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Homework.UnitTests
{
    public class HomeworkControllerTest
    {
        private Mock<IHomeworkService> _homeworkServiceMock;
        private Mock<IFormFile> _formFileMock;
        string recipient = "someone@example.com";
        string url = "https://www.w3schools.com/xml/note.xml";

        public HomeworkControllerTest()
        {
            _homeworkServiceMock = new Mock<IHomeworkService>();
            _formFileMock = new Mock<IFormFile>();
        }

        private HomeworkController CreateHomeworkController()
        {
            return new HomeworkController(_homeworkServiceMock.Object);
        }

        [Fact]
        public async void UploadFile_IsResponseFileContentResult_WhenFileProvided()
        {
            Setup();
            
            var controller = CreateHomeworkController();
            
            var actionResult = await controller.UploadFile(_formFileMock.Object, FileExtension.json);
            
            Assert.IsType<FileContentResult>(actionResult);
        }

        [Fact]
        public async void UploadFile_IsResponseBadRequestResult_WhenFileNotProvided()
        {
            Setup();

            var controller = CreateHomeworkController();

            var actionResult = await controller.UploadFile(null, FileExtension.json);

            Assert.IsType<BadRequestResult>(actionResult);
        }
        
        [Fact]
        public void SendEmail_MustReturn_EmailSent()
        {
            Setup();

            var controller = CreateHomeworkController();

            var actionResult = controller.SendEmail(_formFileMock.Object, FileExtension.json, recipient);

            Assert.True(actionResult.Result.Equals($"An email was sent to {recipient}"));
        }
        
        [Fact]
        public void SendEmail_MustReturn_NoFileWasChosen()
        {
            Setup();

            var controller = CreateHomeworkController();

            var actionResult = controller.SendEmail(null, FileExtension.json, recipient);

            Assert.True(actionResult.Result.Equals($"No file was chosen"));
        }
        
        [Fact]
        public void SendEmail_MustReturn_NoRecepientWasSpecified()
        {
            Setup();

            var controller = CreateHomeworkController();

            var actionResult = controller.SendEmail(_formFileMock.Object, FileExtension.json, string.Empty);

            Assert.True(actionResult.Result.Equals($"No recepient was specified"));
        }
        
        [Fact]
        public async void ConvertFromUrl_IsResponseFileContentResult_WhenFileProvided()
        {
            Setup();

            var controller = CreateHomeworkController();

            var actionResult = await controller.ConvertFromUrl(url, FileExtension.json);

            Assert.IsType<FileContentResult>(actionResult);
        }
        
        [Fact]
        public async void ConvertFromUrl_IsResponseBadRequestResult_WhenUrlNotProvided()
        {
            Setup();

            var controller = CreateHomeworkController();

            var actionResult = await controller.ConvertFromUrl(string.Empty, FileExtension.json);

            Assert.IsType<BadRequestResult>(actionResult);
        }

        private void Setup()
        {
            var content = "Mock content";
            var fileName = "test.pdf";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;

            _formFileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
            _formFileMock.Setup(_ => _.FileName).Returns(fileName);
            _formFileMock.Setup(_ => _.Length).Returns(ms.Length);

            DocumentDto documentDto = new DocumentDto
            {
                FileData = ms.ToArray(),
                FileName = fileName,
                MimeType = "application/octet-stream"
            };

            string result = $"An email was sent to {recipient}";

            _homeworkServiceMock.Setup(x => x.UploadAndSave(_formFileMock.Object, FileExtension.json)).Returns(Task.FromResult(documentDto));
            _homeworkServiceMock.Setup(x => x.UploadAndSendEmail(_formFileMock.Object, FileExtension.json, recipient)).Returns(Task.FromResult(result));
            _homeworkServiceMock.Setup(x => x.ConvertFromUrl(url, FileExtension.json)).Returns(Task.FromResult(documentDto));
        }
    }
}