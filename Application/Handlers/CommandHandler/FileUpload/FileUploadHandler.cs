using Application.Command.Departments;
using Application.Command.EmployeeICTInformations;
using Application.Command.EmployeeOtherDutyInformations;
using Application.Command.FileUploads;
using Application.Commands;
using Application.Common.Mapper;
using Application.Response;
using Core.Entities;
using Core.Entities.Base;
using Core.Repositories.Command;
using Core.Repositories.Command.Base;
using Core.Repositories.Query.Base;
using Core.Repositories.Query;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handlers.CommandHandler
{
    public class FileUploadHandler : IRequestHandler<FileUploadCommand, String>
    {
        public FileUploadHandler()
        {
           
        }
        public async Task<string>  Handle(FileUploadCommand request, CancellationToken cancellationToken)
        {
            
            return "Ok";
        }
    }

}
