using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework.Business.Models
{
    public class DocumentDto
    {
        public byte[]? FileData { get; set; }
        public string? FileName { get; set; }

        public string? MimeType { get; set; }
    }
}
