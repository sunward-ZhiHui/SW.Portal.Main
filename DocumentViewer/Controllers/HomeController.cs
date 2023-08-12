using DocumentViewer.Models;
using System;
using System.IO;
using System.Linq;
using System.Text;
using DevExpress.AspNetCore.Spreadsheet;
using DevExpress.Spreadsheet;
using DevExpress.XtraSpreadsheet.Export;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Policy;
using System.Net.Mime;

namespace DocumentViewer.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index(string url)
        {
            SpreadsheetDocumentContentFromBytes viewmodel = new SpreadsheetDocumentContentFromBytes();
            try
            {
                viewmodel.Extensions = "";
                viewmodel.Url = string.IsNullOrEmpty(url) ? "" : url;
                viewmodel.Id = 1;
                viewmodel.DocumentId = "1";
                if (!string.IsNullOrEmpty(url))
                {
                    string s = viewmodel.Url.Split('.').Last();
                    viewmodel.Extensions = s.ToLower();
                    var uri = new Uri(url);
                    var host = uri.Host;
                    // if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
                    // {
                    string contentType = "";
                    var request = HttpWebRequest.Create(url) as HttpWebRequest;
                    if (request != null)
                    {
                        var response = request.GetResponse() as HttpWebResponse;
                        if (response != null)
                            contentType = response.ContentType;
                    }
                    if (contentType != null)
                    {
                        var webClient = new WebClient();
                        {
                            byte[] byteArrayAccessor() => webClient.DownloadData(new Uri(url));
                            viewmodel.DocumentId = "DocumentId1";
                            viewmodel.ContentAccessorByBytes = byteArrayAccessor;
                            viewmodel.Type = contentType.Split("/")[0].ToLower();
                            viewmodel.ContentType = contentType;
                            return View(viewmodel);
                        }
                        //}
                        /* else
                         {
                             viewmodel.DocumentId = "0";
                             viewmodel.ContentType = contentType;
                             return View(viewmodel);
                         }*/
                    }
                    else
                    {
                        viewmodel.Id = 0;
                        return View(viewmodel);
                    }
                }
                else
                {
                    viewmodel.Id = 0;
                    return View(viewmodel);
                }
            }
            catch (Exception ex)
            {
                viewmodel.Id = 0;
                return View(viewmodel);
                //throw new Exception(ex.Message);
            }
        }
        [HttpPost]
        public ContentResult DxDocRequest()
        {
            return (ContentResult)SpreadsheetRequestProcessor.GetResponse(HttpContext);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = "10" });
        }
    }
}