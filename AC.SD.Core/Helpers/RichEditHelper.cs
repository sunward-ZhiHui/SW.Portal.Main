using DevExpress.Blazor.RichEdit;
using DevExpress.XtraRichEdit;
using System;
using System.Threading.Tasks;

// Aliases to avoid enum mixups
using BlazorDocFormat = DevExpress.Blazor.RichEdit.DocumentFormat;
using XtraDocFormat = DevExpress.XtraRichEdit.DocumentFormat;

namespace AC.SD.Core.Helpers
{
    public static class RichEditHelper
    {
        private static readonly RichEditDocumentServer SharedServer = new RichEditDocumentServer();

        public static async Task<byte[]> ExportHtmlFastAsync(DxRichEdit richEdit)
        {
            if (richEdit == null)
                throw new ArgumentNullException(nameof(richEdit));

            // Export as DOCX from Blazor RichEdit
            byte[] docxBytes = await richEdit.ExportDocumentAsync(BlazorDocFormat.OpenXml);

            lock (SharedServer)
            {
                SharedServer.LoadDocument(docxBytes, XtraDocFormat.OpenXml);

                SharedServer.Options.Export.Html.EmbedImages = true;
                SharedServer.Options.Export.Html.CssPropertiesExportType =
                    DevExpress.XtraRichEdit.Export.Html.CssPropertiesExportType.Inline;
                SharedServer.Options.Export.Html.ExportRootTag =
                    DevExpress.XtraRichEdit.Export.Html.ExportRootTag.Body;

                return SharedServer.SaveDocument(XtraDocFormat.Html);
            }
        }
    }
}
