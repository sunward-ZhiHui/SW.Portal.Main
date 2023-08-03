using Application.Common.Mapper;
using Application.Queries;
using Application.Response;
using Core.Entities;
using Core.Repositories.Query;
using Core.Repositories.Query.Base;
using MediatR;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Hosting;


namespace CMS.Application.Handlers.QueryHandlers
{
    public class UploadFilesHandler : IRequestHandler<UploadFilesRequest, bool>
    {
        private readonly IFileStorageService _fileStorageService;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public UploadFilesHandler(IFileStorageService fileStorageService, IWebHostEnvironment hostingEnvironment)
        {           
            _fileStorageService = fileStorageService;
            _hostingEnvironment = hostingEnvironment;
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
    public class FileUploadCommandHandler : IRequestHandler<FileUploadCommand, string>
    {
        private readonly IFileStorageService _fileStorageService;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public FileUploadCommandHandler(IFileStorageService fileStorageService, IWebHostEnvironment hostingEnvironment)
        {
            _fileStorageService = fileStorageService;
            _hostingEnvironment = hostingEnvironment;
        }
        public async Task<string> Handle(FileUploadCommand request, CancellationToken cancellationToken)
        {
            string BaseDirectory = System.AppContext.BaseDirectory;
            var reportFolder = Path.Combine(BaseDirectory, "Reports");
            //File.WriteAllBytes(reportFolder + request.FileName, request.FileContent);

            if (string.IsNullOrEmpty(reportFolder) || string.IsNullOrEmpty(request.FileName))
            {
                // Handle invalid folder or filename here.
                Console.WriteLine("Invalid folder or filename.");
                //return; // Or throw an exception if appropriate.
            }

            if (request.FileContent == null || request.FileContent.Length == 0)
            {
                // Handle empty or null file content here.
                Console.WriteLine("File content is empty or null.");
                //return; // Or throw an exception if appropriate.
            }



            try
            {
                File.WriteAllBytes(Path.Combine(reportFolder, request.FileName), request.FileContent);
                // File writing successful.
            }
            catch (Exception ex)
            {
                // Handle the exception here. You can log the error message or take appropriate actions.
                Console.WriteLine("Error writing file: " + ex.Message);
            }

            // Save each file using the fileStorageService
            //var saved = await _fileStorageService.SaveFileAsync(request.FileContent, request.SessionId);               

            return request.FileName; ; // Return true if all files are saved successfully
        }

    }
    public class DownloadFileHandler : IRequestHandler<DownloadFileRequest, Documents>
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;
        public DownloadFileHandler(IWebHostEnvironment host, IConfiguration configuration)
        {
            _hostingEnvironment = host;
            _configuration = configuration;
        }
        public Task<Documents> Handle(DownloadFileRequest request, CancellationToken cancellationToken)
        {
            // Read the file content from the specified FilePath
            string BaseUrll = _configuration["DocumentsUrl:FileUrl"];


            string originalString = _hostingEnvironment.ContentRootPath;
            string substringToRemove = "SW.Portal.Solutions\\";
            string result = originalString.Replace(substringToRemove, string.Empty);


            //var BaseUrl = result + @"\DocumentApi\AppUpload\" + request.FilePath;
            var BaseUrl = BaseUrll + request.FilePath;
            //var filePath = @"D:\Projects\SW.Portal.Solutions\DocumentApi\AppUpload\" + request.FilePath;
            //var fileContent = File.ReadAllBytes(BaseUrl);
            var serverFilePath = Path.Combine(_hostingEnvironment.WebRootPath ?? "", "AppUpload", request.FilePath);

            // Create and populate the DownloadFileResponse
            var response = new Documents
            {
                FileName = request.FileName,
                //FileData = fileContent,
                ContentType = request.ContentType,
                FilePath = BaseUrl,
                ServerFilePath= serverFilePath
                //FilePath = serverFilePath,

            };

            return Task.FromResult(response);
        }
    }
    public class DownloadReportFileHandler : IRequestHandler<DownloadReportFileRequest, ReportDocuments>
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;
        public DownloadReportFileHandler(IWebHostEnvironment host, IConfiguration configuration)
        {
            _hostingEnvironment = host;
            _configuration = configuration;
        }
        public Task<ReportDocuments> Handle(DownloadReportFileRequest request, CancellationToken cancellationToken)
        {
            // Read the file content from the specified FilePath
            string BaseUrll = _configuration["DocumentsUrl:FileUrl"];


            string originalString = _hostingEnvironment.ContentRootPath;
            string substringToRemove = "SW.Portal.Solutions\\";
            string result = originalString.Replace(substringToRemove, string.Empty);


            //var BaseUrl = result + @"\DocumentApi\AppUpload\" + request.FilePath;
          //  var BaseUrl = BaseUrll + request.FilePath;
            //var filePath = @"D:\Projects\SW.Portal.Solutions\DocumentApi\AppUpload\" + request.FilePath;
            //var fileContent = File.ReadAllBytes(BaseUrl);
          //  var serverFilePath = Path.Combine(_hostingEnvironment.WebRootPath ?? "", "AppUpload", request.FilePath);

            // Create and populate the DownloadFileResponse
            var response = new ReportDocuments
            {
                FileName = request.FileName,
                //FileData = fileContent,
               // ContentType = request.ContentType,
               // FilePath = BaseUrl,
               // ServerFilePath = serverFilePath
                //FilePath = serverFilePath,

            };

            return Task.FromResult(response);
        }
    }
}
