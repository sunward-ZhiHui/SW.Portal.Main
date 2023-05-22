using Application.Common.Mapper;
using Application.Queries;
using Application.Response;
using Core.Entities;
using Core.Repositories.Query;
using Core.Repositories.Query.Base;
using MediatR;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CMS.Application.Handlers.QueryHandlers
{
    public class UploadFilesHandler : IRequestHandler<UploadFilesRequest, bool>
    {
        private readonly IFileStorageService _fileStorageService;
        public UploadFilesHandler(IFileStorageService fileStorageService)
        {           
            _fileStorageService = fileStorageService;
        }

        public async Task<bool> Handle(UploadFilesRequest request, CancellationToken cancellationToken)
        {
            foreach (var file in request.Files)
            {
                // Save each file using the fileStorageService
                var saved = await _fileStorageService.SaveFileAsync(file,request.SessionId);
                if (!saved)
                {
                    return false; // Return false if any file fails to save
                }
            }

            return true; // Return true if all files are saved successfully

        }


    }  
}
