using Core.Entities;
using Core.Repositories.Query;
using Infrastructure.Repository.Query.Base;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Hosting;
using System.Net.NetworkInformation;
using System.IO;
using System.Threading;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Infrastructure.Repository.Query
{
    public class FileStorageService : QueryRepository<Documents>, IFileStorageService
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        public FileStorageService(IConfiguration configuration, IWebHostEnvironment host)
            : base(configuration)
        {
            _hostingEnvironment = host;
        }
        public async Task<bool> SaveFileAsync(IFormFile file, Guid? SessionId)
        {
            // Implement the file saving logic here
            // You can use the necessary mechanisms to save the file to the desired location or storage          

            try
            {
                string sourceFilePath = file.Name;
                SessionId = Guid.NewGuid();
                var fileName = Guid.NewGuid().ToString() + "_" + file.Name;
                var filePath = Path.Combine("Uploads", fileName);              

                
                //string targetFilePath = Path.Combine(filePath, fileName);

                //using (var inputStream = file.OpenReadStream())
                //{
                //    using (var outputStream = new FileStream(targetFilePath, FileMode.Create))
                //    {
                //        await inputStream.CopyToAsync(outputStream);
                //    }
                //}              

                var serverPaths = _hostingEnvironment.ContentRootPath + @"\AppUpload\Documents\" + SessionId;

                if (!System.IO.Directory.Exists(serverPaths))
                {
                    System.IO.Directory.CreateDirectory(serverPaths);
                }

                var ext = "";
                var newFile = "";
                ext = file.Name;
                int i = ext.LastIndexOf('.');
                string lhs = i < 0 ? ext : ext.Substring(0, i), rhs = i < 0 ? "" : ext.Substring(i + 1);
                var fileName1 = SessionId + "." + rhs;
                var serverPath = serverPaths + @"\" + fileName1;
                var filePaths = getNextFileName(serverPath);
                newFile = filePath.Replace(serverPaths + @"\", "");
                //using (var targetStream = System.IO.File.Create(serverPath))
                //{
                //   await file.Content.CopyToAsync(targetStream);
                //    targetStream.Flush();
                //}

                using (var targetStream = File.Create(serverPath))
                {
                    // await file.CopyToAsync(targetStream);

                    // Copy the file to the destination folder
                    File.Copy(sourceFilePath, serverPath, true);
                    targetStream.Flush();
                }



                using (var connection = CreateConnection())
                {                        
                    var parameters = new DynamicParameters();
                    parameters.Add("FileName", file.Name);
                    parameters.Add("ContentType", file.ContentType);
                    parameters.Add("FileSize", file.Length);
                    parameters.Add("UploadDate", DateTime.Now);
                    parameters.Add("AddedDate", DateTime.Now);
                    parameters.Add("SessionId", SessionId);
                    parameters.Add("IsLatest", true);
                    parameters.Add("FilePath", filePath.Replace(_hostingEnvironment.ContentRootPath + @"\AppUpload\", ""));

                    var query = "INSERT INTO Documents(FileName,ContentType,FileSize,UploadDate,AddedDate,SessionId,IsLatest,FilePath) VALUES (@FileName,@ContentType,@FileSize,@UploadDate,@AddedDate,@SessionId,@IsLatest,@FilePath)";

                    var rowsAffected = await connection.ExecuteAsync(query, parameters);

                    //return rowsAffected;
                           
                }  
                return true; // Return true if the file is saved successfully
            }
            catch (Exception)
            {
                // Handle any exceptions that occurred during file saving
                // You can log the error or perform any other necessary actions
                return false; // Return false if the file saving fails
            }
        }
        private string getNextFileName(string fileName)
        {
            string extension = Path.GetExtension(fileName);

            int i = 0;
            while (System.IO.File.Exists(fileName))
            {
                if (i == 0)
                    fileName = fileName.Replace(extension, "(" + ++i + ")" + extension);
                else
                    fileName = fileName.Replace("(" + i + ")" + extension, "(" + ++i + ")" + extension);
            }

            return fileName;
        }

    }
}
