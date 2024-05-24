using System.Windows;
using System.Windows.Input;
using HomeCollection.Commands;
using HomeCollection.DataBase;
using HomeCollection.Models;
using HomeCollection.Stores;
using HomeCollection.Utils;
using HomeCollection.ViewModels.Base;
using HomeCollection.Views.Windows;

namespace HomeCollection.ViewModels
{
    public class BuildingsListViewModel : ViewModel
    {
        private readonly AppDbContext appDbContext;
        private readonly DataStore dataStore;
        private readonly NavigationStore navigationStore;
        private AddEditBuildingWindow addEditBuildingWindow;

        public IEnumerable<Building> Buildings => appDbContext.Buildings.ToList();

        #region CurrentBuilding
        private Building _currentBuilding;
        public Building CurrentBuilding { get => _currentBuilding; set => Set(ref _currentBuilding, value); }
        #endregion

        /// <summary>
        /// Add new record
        /// </summary>
        #region Command AddCommand
        public ICommand AddCommand { get; set; }

        private bool CanAddCommandExecute(object obj) => true;

        private void OnAddCommandExecuted(object obj)
        {
            CurrentBuilding = new Building();
            addEditBuildingWindow = ServiceLocator.GetService<AddEditBuildingWindow>();
            addEditBuildingWindow.DataContext = this;
            addEditBuildingWindow.ShowDialog();
        }
        #endregion

        /// <summary>
        /// Remove selected record
        /// </summary>
        #region Command RemoveCommand
        public ICommand RemoveCommand { get; set; }

        private bool CanRemoveCommandExecute(object obj) => CurrentBuilding != null;

        private void OnRemoveCommandExecuted(object obj)
        {
            if (MessageBox.Show($"Вы точно хотите удалить запись:\n{CurrentBuilding.Id}\n{CurrentBuilding.ShortName}\n{CurrentBuilding.Address}", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Building building = (Building)obj;
                appDbContext.Remove(building);
                appDbContext.SaveChanges();
            }
        }
        #endregion

        /// <summary>
        /// Open adding/editing window with selected record
        /// </summary>
        #region Command EditCommand
        public ICommand EditCommand { get; set; }

        private bool CanEditCommandExecute(object obj) => CurrentBuilding != null;

        private void OnEditCommandExecuted(object obj)
        {
            Building building = (Building)obj;
            CurrentBuilding = building;
            addEditBuildingWindow = ServiceLocator.GetService<AddEditBuildingWindow>();
            addEditBuildingWindow.DataContext = this;
            addEditBuildingWindow.ShowDialog();
        }
        #endregion

        /// <summary>
        /// Save data from adding/editing window and close it
        /// </summary>
        #region Command SaveAndCloseCommand
        public ICommand SaveAndCloseCommand { get; set; }

        private bool CanSaveAndCloseCommandExecute(object obj)
        {
            if (CurrentBuilding == null)
                return false;
            if (string.IsNullOrEmpty(CurrentBuilding.ShortName) || string.IsNullOrEmpty(CurrentBuilding.Address))
                return false;
            if (CurrentBuilding.ShortName.Length < 5 || CurrentBuilding.Address.Length < 10)
                return false;
            return true;
        }

        private void OnSaveAndCloseCommandExecuted(object obj)
        {
            if (appDbContext.Buildings.Any(x => x.Id == CurrentBuilding.Id))
            {
                appDbContext.Buildings.Update(CurrentBuilding);
            }
            else
            {
                appDbContext.Buildings.Add(CurrentBuilding);
            }
            appDbContext.SaveChanges();
            OnCloseCommandExecuted(obj);
        }
        //
        #endregion

        /// <summary>
        /// Close adding/editing window
        /// </summary>
        #region Command CloseCommand
        public ICommand CloseCommand { get; set; }

        private bool CanCloseCommandExecute(object obj) => true;

        private void OnCloseCommandExecuted(object obj)
        {
            CurrentBuilding = null;
            addEditBuildingWindow.Close();
        }
        #endregion

        /// <summary>
        /// Open child view
        /// </summary>
        #region Command OpenCommand
        public ICommand OpenCommand { get; set; }

        private bool CanOpenCommandExecute(object obj) => CurrentBuilding != null;

        private void OnOpenCommandExecuted(object obj)
        {
            dataStore.CurrentBuilding = CurrentBuilding;
            navigationStore.CurrentViewModel = ServiceLocator.GetService<EnterancesListViewModel>();
        }
        #endregion

        public BuildingsListViewModel(AppDbContext appDbContext, DataStore dataStore, NavigationStore navigationStore)
        {
            this.appDbContext = appDbContext;
            this.dataStore = dataStore;
            this.navigationStore = navigationStore;
            appDbContext.SavedChanges += OnAppDbContext_SavedChanges;

            AddCommand = new LambdaCommand(OnAddCommandExecuted, CanAddCommandExecute);
            RemoveCommand = new LambdaCommand(OnRemoveCommandExecuted, CanRemoveCommandExecute);
            EditCommand = new LambdaCommand(OnEditCommandExecuted, CanEditCommandExecute);
            OpenCommand = new LambdaCommand(OnOpenCommandExecuted, CanOpenCommandExecute);
            SaveAndCloseCommand = new LambdaCommand(OnSaveAndCloseCommandExecuted, CanSaveAndCloseCommandExecute);
            CloseCommand = new LambdaCommand(OnCloseCommandExecuted, CanCloseCommandExecute);
        }

        /// <summary>
        /// When changes saved in DB
        /// Need to update flats list
        /// </summary>
        private void OnAppDbContext_SavedChanges(object? sender, Microsoft.EntityFrameworkCore.SavedChangesEventArgs e)
        {
            OnPropertyChanged(nameof(Buildings));
        }
    }
}
