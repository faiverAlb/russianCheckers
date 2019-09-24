using System;
using System.Windows.Input;
using RussianCheckers.MVVM;

namespace RussianCheckers
{
    public class NotificationDialogViewModel : IDialogRequestClose
    {
        private readonly string _notificationMessage;
        public ICommand OkCommand { get; }

        public NotificationDialogViewModel(string notificationMessage)
        {
            _notificationMessage = notificationMessage;

//            OkCommand = new RelayCommand(p => CloseRequested?.Invoke(this, new DialogCloseRequestedEventArgs(true)));

        }

        public event EventHandler<DialogCloseRequestedEventArgs> CloseRequested;
    }
}