using System.Threading.Tasks;

namespace GadjIT_App.Services{

    public interface INotificationManager
    {
        Task ShowNotification(string _notificationType, string _message);
        Task ShowNotification(string _notificationType, string _message, int _displaySeconds);
    }
}