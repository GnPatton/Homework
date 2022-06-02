using Homework.Business.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Net.Mail;
using System.Text;
using System.Xml;

namespace Homework.Business.Services.Implementation
{
    public class HomeworkService : IHomeworkService
    {
        public async Task<DocumentDto> UploadAndSave(IFormFile file, FileExtension fileExtension)
        {
            var documentDto = new DocumentDto
            {
                FileData = GetByteArrayFromFile(file, fileExtension),
                FileName = file.FileName.Replace(".xml", $".{fileExtension}"),
                MimeType = "application/octet-stream"
            };

            return documentDto;
        }

        private byte[] GetByteArrayFromFile(IFormFile file, FileExtension fileExtension)
        {
            var input = new StringBuilder();

            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                while (reader.Peek() >= 0)
                    input.AppendLine(reader.ReadLine());
            }

            return ChooseConvertingEngine(input.ToString(), fileExtension);
        }

        private byte[] ChooseConvertingEngine(string input, FileExtension fileExtension)
        {
            switch (fileExtension)
            {
                case FileExtension.json:
                    {
                        return GetByteArrayForJson(input.ToString());
                    }
                case FileExtension.protobuf:
                    //engine for converting to protobuf
                    throw new NotImplementedException();
                case FileExtension.config:
                    //engine for converting to config
                    throw new NotImplementedException();
                default:
                    throw new NotImplementedException();
            }
        }
        
        private byte[] GetByteArrayForJson(string input)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(input);

            if(doc.FirstChild.NodeType == XmlNodeType.XmlDeclaration)
            {
                doc.RemoveChild(doc.FirstChild);
            }

            var serializedDoc = JsonConvert.SerializeXmlNode(doc);

            return Encoding.ASCII.GetBytes(serializedDoc);
        }

        public void UploadAndSendEmail(IFormFile file, FileExtension fileExtension, string recipient)
        {
            var byteArray = GetByteArrayFromFile(file, fileExtension);
            var fileName = file.FileName.Replace(".xml", $".{fileExtension}");

            var client = new SmtpClient("smtp.vsb.cz", 25);

            using (MailMessage mail = new MailMessage("artem.kozhokaru.st@vsb.cz", recipient))
            {
                Stream stream = new MemoryStream(byteArray);
                Attachment attachment = new Attachment(stream, fileName);

                mail.Subject = "Converted file";
                mail.Body = $"Here is your file converted from XML to {fileExtension.ToString().ToUpper()}";
                mail.Attachments.Add(attachment);

                client.Send(mail);
            }
        }

        public async Task<DocumentDto> ConvertFromUrl(string url, FileExtension fileExtension)
        {
            byte[] byteArray;

            using (HttpClient httpClient = new HttpClient())
            {
                var str = await httpClient.GetStringAsync(url);
                byteArray = ChooseConvertingEngine(str, fileExtension);
            }

            var documentDto = new DocumentDto
            {
                FileData = byteArray,
                FileName = ($"converted.{fileExtension}"),
                MimeType = "application/octet-stream"
            };

            return documentDto;
        }
    }
}
