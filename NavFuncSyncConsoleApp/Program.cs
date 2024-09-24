// See https://aka.ms/new-console-template for more information
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
var url = "https://portal.sunwardpharma.com:8423/api/NavFun/InsertOrUpdateFinishedProdOrderLineData";
restApi(url);
var ItemBatchurl = "https://portal.sunwardpharma.com:8423/api/NavFun/InsertOrUpdateItemBatch";
restApi(ItemBatchurl);
var navprodOrderLineUrl = "https://portal.sunwardpharma.com:8423/api/NavFun/InsertOrUpdateNavprodOrderLine";
restApi(navprodOrderLineUrl);
void restApi(string? Url)
{
    var options = new RestClientOptions
    {
        //MaxTimeout = 5 * 1000,
    };
    var client = new RestClient(options);
    var request = new RestRequest(Url, Method.Get);
    RestResponse response = client.Get(request);
   // var result = JsonConvert.DeserializeObject<dynamic>(response.Content);
}
