using System.Collections.Generic;
using System.Linq;

namespace AC.SD.Core.Services
{
    public static partial class ResourceCollection
    {
        public static List<Resource> GetResourcesForGrouping()
        {
            return GetResources().Take(3).ToList();
        }
        public static List<Resource> GetResources()
        {
            return new List<Resource>() {
                new Resource() { Id=0 , Name="Email", GroupId=100, BackgroundCss="dxbl-blue-color", TextCss="text-white" },
                new Resource() { Id=1 , Name="Todo", GroupId=100, BackgroundCss="dxbl-pink-color", TextCss="text-white" },
                new Resource() { Id=2 , Name="Appointments", GroupId=100, BackgroundCss="dxbl-purple-color", TextCss="text-white" }               
            };
        }
        public static List<Resource> GetResourceGroups()
        {
            return new List<Resource>() {
                new Resource() { Id=100, Name="Scheduler Filter", IsGroup=true }
                //new Resource() { Id=101, Name="Tags", IsGroup=true }
            };
        }
    }
}
