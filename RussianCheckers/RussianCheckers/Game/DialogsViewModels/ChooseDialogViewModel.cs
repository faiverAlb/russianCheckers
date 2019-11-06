using System;
using System.Windows.Input;
using RussianCheckers.Core;
using RussianCheckers.Infrastructure;

namespace RussianCheckers.Game.DialogsViewModels
{
    public class ChooseDialogViewModel : ObservableObject, IDialogRequestClose
    {
        public ICommand ChooseSideCommand { get; }
        public event EventHandler<DialogCloseRequestedEventArgs> CloseRequested;

        public ChooseDialogViewModel()
        {
            ChooseSideCommand = new ActionCommand(ChooseSideCommandAction);
            CloseRequested?.Invoke(this, new DialogCloseRequestedEventArgs(true));
        }

        private void ChooseSideCommandAction(object obj)
        {
            Side = (Side) obj;
            CloseRequested?.Invoke(this, new DialogCloseRequestedEventArgs(true));
        }

        public Side Side { get; set; }
    }
}