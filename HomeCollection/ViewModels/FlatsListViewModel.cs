using System.Windows;
using System.Windows.Input;
using HomeCollection.Commands;
using HomeCollection.DataBase;
using HomeCollection.Models;
using HomeCollection.Stores;
using HomeCollection.Utils;
using HomeCollection.ViewModels.Base;
using HomeCollection.Views.Windows;
using Microsoft.EntityFrameworkCore;

namespace HomeCollection.ViewModels
{
    public class FlatsListViewModel : ViewModel
    {
        private readonly AppDbContext appDbContext;
        private readonly DataStore dataStore;
        private readonly NavigationStore navigationStore;
        private AddEditFlatWindow addEditFlatWindow;

        public IEnumerable<Flat> Flats => appDbContext.Flats.Where(x => x.Enterance == dataStore.CurrentEnterance).ToList();

        #region CurrentFlat
        private Flat _currentFlat;
        public Flat CurrentFlat { get => _currentFlat; set => Set(ref _currentFlat, value); }
        #endregion

        /// <summary>
        /// Add new record
        /// </summary>
        #region Command AddCommand
        public ICommand AddCommand { get; set; }

        private bool CanAddCommandExecute(object obj) => true;

        private void OnAddCommandExecuted(object obj)
        {
            CurrentFlat = new Flat();
            addEditFlatWindow = ServiceLocator.GetService<AddEditFlatWindow>();
            addEditFlatWindow.DataContext = this;
            addEditFlatWindow.ShowDialog();
        }
        #endregion

        /// <summary>
        /// Remove selected record
        /// </summary>
        #region Command RemoveCommand
        public ICommand RemoveCommand { get; set; }

        private bool CanRemoveCommandExecute(object obj) => CurrentFlat != null;

        private void OnRemoveCommandExecuted(object obj)
        {
            if (MessageBox.Show($"Вы точно хотите удалить запись:\n{CurrentFlat.Id}\n{CurrentFlat.Number}", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Flat flat = (Flat)obj;
                appDbContext.Remove(flat);
                appDbContext.SaveChanges();
            }
        }
        #endregion

        /// <summary>
        /// Open adding/editing window with selected record
        /// </summary>
        #region Command EditCommand
        public ICommand EditCommand { get; set; }

        private bool CanEditCommandExecute(object obj) => CurrentFlat != null;

        private void OnEditCommandExecuted(object obj)
        {
            Flat flat = (Flat)obj;
            CurrentFlat = flat;
            addEditFlatWindow = ServiceLocator.GetService<AddEditFlatWindow>();
            addEditFlatWindow.DataContext = this;
            addEditFlatWindow.ShowDialog();
        }
        #endregion

        /// <summary>
        /// Save data from adding/editing window and close it
        /// </summary>
        #region Command SaveAndCloseCommand
        public ICommand SaveAndCloseCommand { get; set; }

        private bool CanSaveAndCloseCommandExecute(object obj)
        {
            if (CurrentFlat == null)
                return false;
            if (string.IsNullOrEmpty(CurrentFlat.Number.ToString()) || CurrentFlat.Number <= 0)
                return false;
            return true;
        }

        private void OnSaveAndCloseCommandExecuted(object obj)
        {
            CurrentFlat.Enterance = dataStore.CurrentEnterance;
            if (appDbContext.Flats.Any(x => x.Id == CurrentFlat.Id))
            {
                appDbContext.Flats.Update(CurrentFlat);
            }
            else
            {
                appDbContext.Flats.Add(CurrentFlat);
            }
            appDbContext.SaveChanges();
            OnCloseCommandExecuted(obj);
        }

        #endregion

        /// <summary>
        /// Close adding/editing window
        /// </summary>
        #region Command CloseCommand
        public ICommand CloseCommand { get; set; }

        private bool CanCloseCommandExecute(object obj) => true;

        private void OnCloseCommandExecuted(object obj)
        {
            CurrentFlat = null;
            addEditFlatWindow.Close();
        }

        #endregion

        /// <summary>
        /// Open child view
        /// </summary>
        #region Command OpenCommand
        public ICommand OpenCommand { get; set; }

        private bool CanOpenCommandExecute(object obj) => CurrentFlat != null;

        private void OnOpenCommandExecuted(object obj)
        {
            dataStore.CurrentFlat = CurrentFlat;
            navigationStore.CurrentViewModel = ServiceLocator.GetService<PeoplesListViewModel>();
        }
        #endregion

        /// <summary>
        /// Show prev view
        /// </summary>
        #region Command BackCommand
        public ICommand BackCommand { get; set; }

        private bool CanBackCommandExecute(object obj) => true;

        private void OnBackCommandExecuted(object obj)
        {
            dataStore.CurrentEnterance = null;
            navigationStore.CurrentViewModel = ServiceLocator.GetService<EnterancesListViewModel>();
        }
        #endregion


        public FlatsListViewModel(AppDbContext appDbContext, DataStore dataStore, NavigationStore navigationStore)
        {
            this.appDbContext = appDbContext;
            this.dataStore = dataStore;
            this.navigationStore = navigationStore;
            appDbContext.SavedChanges += OnAppDbContext_SavedChanges;


            AddCommand = new LambdaCommand(OnAddCommandExecuted, CanAddCommandExecute);
            RemoveCommand = new LambdaCommand(OnRemoveCommandExecuted, CanRemoveCommandExecute);
            EditCommand = new LambdaCommand(OnEditCommandExecuted, CanEditCommandExecute);
            SaveAndCloseCommand = new LambdaCommand(OnSaveAndCloseCommandExecuted, CanSaveAndCloseCommandExecute);
            CloseCommand = new LambdaCommand(OnCloseCommandExecuted, CanCloseCommandExecute);
            OpenCommand = new LambdaCommand(OnOpenCommandExecuted, CanOpenCommandExecute);
            BackCommand = new LambdaCommand(OnBackCommandExecuted, CanBackCommandExecute);
        }

        /// <summary>
        /// When changes saved in DB
        /// Need to update flats list
        /// </summary>
        private void OnAppDbContext_SavedChanges(object? sender, SavedChangesEventArgs e)
        {
            OnPropertyChanged(nameof(Flats));
        }
    }
}
