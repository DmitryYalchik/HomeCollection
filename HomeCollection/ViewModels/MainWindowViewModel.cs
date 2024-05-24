using HomeCollection.Stores;
using HomeCollection.Utils;
using HomeCollection.ViewModels.Base;

namespace HomeCollection.ViewModels
{
    public class MainWindowViewModel : ViewModel
    {
        private readonly NavigationStore navigationStore;

        public ViewModel CurrentViewModel => navigationStore.CurrentViewModel;

        public MainWindowViewModel(NavigationStore navigationStore)
        {
            this.navigationStore = navigationStore;
            navigationStore.CurrentViewModelChanged += NavigationStore_CurrentViewModelChanged;
            navigationStore.CurrentViewModel = ServiceLocator.GetService<BuildingsListViewModel>();
        }

        /// <summary>
        /// Wher changed current view model
        /// Need fow update view in grid
        /// </summary>
        private void NavigationStore_CurrentViewModelChanged()
        {
            OnPropertyChanged(nameof(CurrentViewModel));
        }
    }
}
