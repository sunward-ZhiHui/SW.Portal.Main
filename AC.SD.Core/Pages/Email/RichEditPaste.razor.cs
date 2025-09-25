using DevExpress.Blazor.RichEdit;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace AC.SD.Core.Pages.Email
{
    public partial class RichEditPaste : ComponentBase
    {
        DxRichEdit richEdit;
        DotNetObjectReference<RichEditPaste>? _objRef;
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _objRef = DotNetObjectReference.Create(this);
                await JSRuntime.InvokeVoidAsync(
                    "attachRichEditPasteHandler",
                    "#myRichEditWrapper", // CSS selector
                    _objRef
                );
            }
        }

        [JSInvokable]
        public async Task OnImagePasted()
        {
            var documentAPI = richEdit.DocumentAPI;
            var images = await documentAPI.InlineImages.GetAllAsync();

            foreach (var img in images)
            {
                await img.ChangePropertiesAsync(properties =>
                {
                    properties.ActualSize.Width = UnitConverter.CentimetersToTwips(5);
                    properties.ActualSize.Height = UnitConverter.CentimetersToTwips(5);
                });
            }

            Console.WriteLine($"Resized {images.Count} images after paste.");
        }

        public void Dispose()
        {
            _objRef?.Dispose();
        }
    }
}
