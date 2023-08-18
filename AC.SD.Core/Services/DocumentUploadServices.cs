using Application.Common;
using Azure.Core;
using Core.Entities;
using Core.Repositories.Query;
using Infrastructure.Repository.Query;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AC.SD.Core.Services
{
    public class DocumentUploadServices
    {
        readonly IConfiguration config;
        public DocumentUploadServices(IConfiguration _config)
        {
            this.config = _config;
        }
        public async Task<string> GetGlobalLoginAsync(ApplicationUser Appresult)
        {
            string url = this.config.GetSection("DocumentsUrl:PortalApiUrl").Value + "UserAuth/login";
            HttpClient client = new HttpClient();
            var requestData = new { loginId = Appresult.LoginID, loginPassword = EncryptDecryptPassword.Decrypt(Appresult.LoginPassword) };
            string json = JsonSerializer.Serialize(requestData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);
            string responseContent = await response.Content.ReadAsStringAsync();
            var result = JObject.Parse(responseContent);
            string token = result["token"].ToString();
            return token;
        }
        public async Task GetGlobalDocumentsUrlAsync(ApplicationUser Appresult)
        {
            string accessToken = await GetGlobalLoginAsync(Appresult);
            string url = this.config.GetSection("DocumentsUrl:PortalApiUrl").Value + "FileProfileType/UploadDocument";
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
                var multiForm = new MultipartFormDataContent();

                // add API method parameters
               // multiForm.Add(new StringContent(token), "token");
               // multiForm.Add(new StringContent(channels), "channels");

                // add file and directly upload it
                //FileStream fs = File.OpenRead(path);
               // multiForm.Add(new StreamContent(fs), "file", Path.GetFileName(path));

                // send request to API
                var response = await client.PostAsync(url, multiForm);
            }
        }
    }
}
