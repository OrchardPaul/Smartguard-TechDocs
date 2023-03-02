using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace GadjIT_App.Services{


    public class NotificationManager : INotificationManager
    {
        private readonly IJSRuntime jsRuntime;


        public NotificationManager(IJSRuntime _jsRuntime)
        {
            jsRuntime = _jsRuntime;
        }

        /// <summary>
        /// Displays a Notification Message
        /// </summary>
        /// <param name="_notificationType"></param>
        /// <param name="_message"></param>
        /// <returns></returns>
        public async Task ShowNotification(String _notificationType, String _message)
        {
            int _displaySeconds = 2;

            await ShowNotification(_notificationType, _message, _displaySeconds);
        }

        /// <summary>
        /// Overload to display Notification Message
        /// Takes number of seconds to be displayed
        /// </summary>
        /// <param name="_notificationType"></param>
        /// <param name="_message"></param>
        /// <param name="_displaySeconds"></param>
        /// <returns></returns>
        public async Task ShowNotification(String _notificationType, String _message, int _displaySeconds)
        {
            if (jsRuntime != null && !String.IsNullOrEmpty(_notificationType) && !String.IsNullOrEmpty(_message))
            {
                int displayMilliseconds = _displaySeconds * 1000;
                await jsRuntime.InvokeVoidAsync("showNotification", _notificationType, _message, displayMilliseconds);
            }
                
            
            
        }

    }
}