using DevExpress.Blazor;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Command.FileUploads
{
    public class FileUploadCommand : IRequest<string>
    {
        public IEnumerable<UploadFileInfo> Files { get; set; }
        public string FileName { get; set; }
    }
}
