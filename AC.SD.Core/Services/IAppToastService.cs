namespace AC.SD.Core.Services
{
    public interface IAppToastService
    {
        void Show(string message, string title = "Notification");
        void ShowSuccess(string message, string title = "Success");
        void ShowError(string message, string title = "Error");
        void ShowInfo(string message, string title = "Info");
        void ShowWarning(string message, string title = "Warning");
    }

}
