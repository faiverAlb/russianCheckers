using System;
using System.Windows.Input;
using RussianCheckers.MVVM;

namespace RussianCheckers
{
    public class NotificationDialogViewModel :ObservableObject, IDialogRequestClose
    {
        private string _notificationMessage;
        public ICommand OkCommand { get; }

        public string NotificationMessage
        {
            get { return _notificationMessage; }
            set
            {
                _notificationMessage = value;
                RaisePropertyChangedEvent(nameof(NotificationMessage));
            }
        }

        public NotificationDialogViewModel(string notificationMessage)
        {
            NotificationMessage = notificationMessage;

//            OkCommand = new RelayCommand(p => CloseRequested?.Invoke(this, new DialogCloseRequestedEventArgs(true)));

        }

        public event EventHandler<DialogCloseRequestedEventArgs> CloseRequested;
    }
}