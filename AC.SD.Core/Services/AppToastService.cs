using DevExpress.Blazor;

namespace AC.SD.Core.Services
{
    public class AppToastService : IAppToastService
    {
        private readonly IToastNotificationService _toastService;

        public AppToastService(IToastNotificationService toastService)
        {
            _toastService = toastService;
        }

        public void Show(string message, string title = "Notification")
        {
            _toastService.ShowToast(new ToastOptions
            {
                ProviderName = "Customization",
                Title = title,
                Text = message,
                RenderStyle = ToastRenderStyle.Primary,
                ThemeMode = ToastThemeMode.Saturated,
            });
        }

        public void ShowSuccess(string message, string title = "Success") =>
            ShowStyled(message, title, ToastRenderStyle.Success);

        public void ShowError(string message, string title = "Error") =>
            ShowStyled(message, title, ToastRenderStyle.Danger);

        public void ShowInfo(string message, string title = "Info") =>
            ShowStyled(message, title, ToastRenderStyle.Info);

        public void ShowWarning(string message, string title = "Warning") =>
            ShowStyled(message, title, ToastRenderStyle.Warning);

        private void ShowStyled(string message, string title, ToastRenderStyle style)
        {
            _toastService.ShowToast(new ToastOptions
            {
                ProviderName = "Customization",
                Title = title,
                Text = message,
                RenderStyle = style,
                ThemeMode = ToastThemeMode.Auto
            });
        }
    }


}
