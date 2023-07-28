using DevExpress.XtraReports.Services;
using DevExpress.XtraReports.UI;
using System;
using System.IO;
using System.Linq;

namespace AC.SD.Core.ReportProvider
{
    public class ReportByNameService : IReportProvider
    {
        public XtraReport GetReport(string reportName, ReportProviderContext context)
        {
            string BaseDirectory = System.AppContext.BaseDirectory;
            XtraReport report = new XtraReport();
            var reportFolder = Path.Combine(BaseDirectory, "Reports");

            if (Directory.EnumerateFiles(reportFolder).
                Select(Path.GetFileNameWithoutExtension).Contains(reportName))
            {
                byte[] reportBytes = File.ReadAllBytes(Path.Combine(reportFolder, reportName + ".repx"));
                using (MemoryStream ms = new MemoryStream(reportBytes))
                    report = XtraReport.FromXmlStream(ms);
            }
            return report;
        }
    }
}
