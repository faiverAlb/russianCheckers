namespace RussianCheckers.Infrastructure
{
    public interface IDialogService
    {
        void ShowDialog<TViewModel>(TViewModel viewModel) where TViewModel : IDialogRequestClose;
    }
}