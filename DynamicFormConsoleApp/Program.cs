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
var url = "https://portal.sunwardpharma.com:8423/api/DynamicForm/GetDynamicFormDataList?DynamicFormSessionId=cc0228a7-0ed4-421f-b78b-a816ad373586";
loadDynamicFomJobs(url);
//loadDynamicForm(url);
//void loadDynamicForm(string? Url)
//{
//    var options = new RestClientOptions
//    {
//        //MaxTimeout = 5 * 1000,
//    };
//    var client = new RestClient(options);
//    var request = new RestRequest(Url, Method.Get);
//    RestResponse response = client.Get(request);
//    var deserial = JsonConvert.DeserializeObject<dynamic>(response.Content);
//    if (deserial.results.Count > 0)
//    {
//        var counts = deserial.results.Count;
//        for (int i = 0; i < counts; i++)
//        {
//            Console.WriteLine("Start ************************************************");
//            Console.WriteLine("Profile No:" + deserial.results[i].profileNo);
//            Console.WriteLine("Name:" + deserial.results[i].name);
//            if (deserial.results[i].objectDataList != null)
//            {
//                var itemValue = deserial.results[0].objectDataList;
//                if (itemValue is JArray)
//                {
//                    List<ExpandoObject> listData = itemValue.ToObject<List<ExpandoObject>>();
//                    if (listData != null && listData.Count > 0)
//                    {
//                        var list = listData.FirstOrDefault().ToList();
//                        if (list != null)
//                        {
//                            list.ForEach(async s =>
//                            {
//                                dynamic val = s.Value;
//                                Console.WriteLine("Key:" + s.Key);
//                                Console.WriteLine("Label:" + val.Label);
//                                Console.WriteLine("Value:" + val.Value);
//                                if (((IDictionary<string, object>)s.Value).ContainsKey("IsGrid"))
//                                {
//                                    Console.WriteLine("IsGrid:");
//                                    var url = "https://portal.sunwardpharma.com:8423/api/DynamicForm/GetDynamicFormDataList" + val.Url;
//                                    loadDynamicForm(url);
//                                }
//                            });
//                        }
//                    }
//                }
//            }
//            Console.WriteLine("End ************************************************//");
//        }
//    }
//}
dynamic restApi(string? Url)
{
    var options = new RestClientOptions
    {
        //MaxTimeout = 5 * 1000,
    };
    var client = new RestClient(options);
    var request = new RestRequest(Url, Method.Get);
    RestResponse response = client.Get(request);
    return JsonConvert.DeserializeObject<dynamic>(response.Content);
}
void loadDynamicFomJobs(string? Url)
{
    var options = new RestClientOptions
    {
        //MaxTimeout = 5 * 1000,
    };
    var client = new RestClient(options);
    var request = new RestRequest(Url, Method.Get);
    RestResponse response = client.Get(request);
    var deserial = JsonConvert.DeserializeObject<dynamic>(response.Content);
    IctMaster ictMaster = new IctMaster();
    if (deserial.results.Count > 0)
    {
        var counts = 1;
        for (int i = 0; i < counts; i++)
        {
            ictMaster.ProfileNo = deserial.results[i].profileNo;
            if (deserial.results[i].objectDataList != null)
            {
                var itemValue = deserial.results[0].objectDataList;
                if (itemValue is JArray)
                {
                    List<ExpandoObject> listData = itemValue.ToObject<List<ExpandoObject>>();
                    if (listData != null && listData.Count > 0)
                    {
                        var list = listData.FirstOrDefault().ToList();
                        if (list != null && list.Count > 0)
                        {
                            list.ForEach(async s =>
                            {
                                dynamic val = s.Value;
                                if (s.Key == "43_Attr")
                                {
                                    ictMaster.Description = val.Value;
                                }
                                if (s.Key == "35_Attr")
                                {
                                    ictMaster.CompanyName = val.Value;
                                }
                                if (((IDictionary<string, object>)s.Value).ContainsKey("IsGrid"))
                                {
                                    if (s.Key == "45_Attr")
                                    {
                                        List<IctJobs1> ictJobs1s = new List<IctJobs1>();
                                        var url = "https://portal.sunwardpharma.com:8423/api/DynamicForm/GetDynamicFormDataList" + val.Url;
                                        var result = restApi(url);
                                        if (result.results.Count > 0)
                                        {
                                            var resultCount = result.results.Count;
                                            for (int j = 0; j < resultCount; j++)
                                            {
                                                if (result.results[j].objectDataList != null)
                                                {
                                                    var itemValue1 = result.results[0].objectDataList;
                                                    if (itemValue1 is JArray)
                                                    {
                                                        List<ExpandoObject> listData1 = itemValue1.ToObject<List<ExpandoObject>>();
                                                        if (listData1 != null && listData1.Count > 0)
                                                        {
                                                            var list1 = listData1.FirstOrDefault().ToList();
                                                            IctJobs1 ictJobs1 = new IctJobs1();
                                                            ictJobs1.ProfileNo = result.results[j].profileNo;
                                                            if (list1 != null && list1.Count > 0)
                                                            {
                                                                list1.ForEach(s1 =>
                                                                {
                                                                    
                                                                    dynamic val1 = s1.Value;
                                                                    if (s1.Key == "38_Attr")
                                                                    {
                                                                        ictJobs1.Description = val1.Value;
                                                                    }
                                                                    if (s1.Key == "37_Attr")
                                                                    {
                                                                        ictJobs1.JobNum = val1.Value;
                                                                    }
                                                                    
                                                                });
                                                            }
                                                            ictJobs1s.Add(ictJobs1);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        ictMaster.JobAssignments1 = ictJobs1s;
                                    }
                                    if (s.Key == "86_Attr")
                                    {
                                        List<IctJobs2> ictJobs2s = new List<IctJobs2>();
                                        var url = "https://portal.sunwardpharma.com:8423/api/DynamicForm/GetDynamicFormDataList" + val.Url;
                                        var results = restApi(url);
                                        if (results.results.Count > 0)
                                        {
                                            var resultCounts = results.results.Count;
                                            for (int k = 0; k < resultCounts; k++)
                                            {
                                                if (results.results[k].objectDataList != null)
                                                {
                                                    var itemValue2 = results.results[0].objectDataList;
                                                    if (itemValue2 is JArray)
                                                    {
                                                        List<ExpandoObject> listData2 = itemValue2.ToObject<List<ExpandoObject>>();
                                                        if (listData2 != null && listData2.Count > 0)
                                                        {
                                                            var list2 = listData2.FirstOrDefault().ToList();
                                                            IctJobs2 ictJobs2 = new IctJobs2();
                                                            ictJobs2.ProfileNo = results.results[k].profileNo;
                                                            if (list2 != null && list2.Count > 0)
                                                            {
                                                                list2.ForEach(s2 =>
                                                                {

                                                                    dynamic val2 = s2.Value;
                                                                    if (s2.Key == "38_Attr")
                                                                    {
                                                                        ictJobs2.Description = val2.Value;
                                                                    }
                                                                    if (s2.Key == "37_Attr")
                                                                    {
                                                                        ictJobs2.JobNum = val2.Value;
                                                                    }

                                                                });
                                                            }
                                                            ictJobs2s.Add(ictJobs2);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        ictMaster.JobAssignments2 = ictJobs2s;
                                    }
                                }
                            });
                        }
                    }
                }
            }
        }
    }
    var jsonString = JsonConvert.SerializeObject(ictMaster);
    var obj = JsonConvert.DeserializeObject<object>(jsonString);
    Console.WriteLine(obj);
}

public class IctMaster
{
    public string? ProfileNo { get; set; }
    public string? CompanyName { get; set; }
    public string? Description { get; set; }
    public List<IctJobs1>? JobAssignments1 { get; set; }
    public List<IctJobs2>? JobAssignments2 { get; set; }
}
public class IctJobs1
{
    public string? ProfileNo { get; set; }
    public string? JobNum { get; set; }
    public string? Description { get; set; }
}
public class IctJobs2
{
    public string? ProfileNo { get; set; }
    public string? JobNum { get; set; }
    public string? Description { get; set; }
}

