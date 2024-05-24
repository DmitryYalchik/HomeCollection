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
    public class EnterancesListViewModel : ViewModel
    {
        private readonly AppDbContext appDbContext;
        private readonly DataStore dataStore;
        private readonly NavigationStore navigationStore;
        private AddEditEnteranceWindow addEditEnteranceWindow;

        public IEnumerable<Enterance> Enterances => appDbContext.Enterances.Where(x => x.Building == dataStore.CurrentBuilding).ToList();

        #region CurrentEnterance
        private Enterance _currentEnterance;
        public Enterance CurrentEnterance { get => _currentEnterance; set => Set(ref _currentEnterance, value); }
        #endregion

        /// <summary>
        /// Add new record
        /// </summary>
        #region Command AddCommand
        public ICommand AddCommand { get; set; }

        private bool CanAddCommandExecute(object obj) => true;

        private void OnAddCommandExecuted(object obj)
        {
            CurrentEnterance = new Enterance();
            addEditEnteranceWindow = ServiceLocator.GetService<AddEditEnteranceWindow>();
            addEditEnteranceWindow.DataContext = this;
            addEditEnteranceWindow.ShowDialog();
        }
        #endregion

        /// <summary>
        /// Remove selected record
        /// </summary>
        #region Command RemoveCommand
        public ICommand RemoveCommand { get; set; }

        private bool CanRemoveCommandExecute(object obj) => CurrentEnterance != null;

        private void OnRemoveCommandExecuted(object obj)
        {
            if (MessageBox.Show($"Вы точно хотите удалить запись:\n{CurrentEnterance.Id}\n{CurrentEnterance.Number}", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Enterance enterance = (Enterance)obj;
                appDbContext.Remove(enterance);
                appDbContext.SaveChanges();
            }
        }
        #endregion

        /// <summary>
        /// Open adding/editing window with selected record
        /// </summary>
        #region Command EditCommand
        public ICommand EditCommand { get; set; }

        private bool CanEditCommandExecute(object obj) => CurrentEnterance != null;

        private void OnEditCommandExecuted(object obj)
        {
            Enterance enterance = (Enterance)obj;
            CurrentEnterance = enterance;
            addEditEnteranceWindow = ServiceLocator.GetService<AddEditEnteranceWindow>();
            addEditEnteranceWindow.DataContext = this;
            addEditEnteranceWindow.ShowDialog();
        }
        #endregion

        /// <summary>
        /// Save data from adding/editing window and close it
        /// </summary>
        #region Command SaveAndCloseCommand
        public ICommand SaveAndCloseCommand { get; set; }

        private bool CanSaveAndCloseCommandExecute(object obj)
        {
            if (CurrentEnterance == null)
                return false;
            if (string.IsNullOrEmpty(CurrentEnterance.Number.ToString()) || CurrentEnterance.Number <= 0)
                return false;
            return true;
        }

        private void OnSaveAndCloseCommandExecuted(object obj)
        {
            CurrentEnterance.Building = dataStore.CurrentBuilding;
            if (appDbContext.Enterances.Any(x => x.Id == CurrentEnterance.Id))
            {
                appDbContext.Enterances.Update(CurrentEnterance);
            }
            else
            {
                appDbContext.Enterances.Add(CurrentEnterance);
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
            CurrentEnterance = null;
            addEditEnteranceWindow.Close();
        }

        #endregion

        /// <summary>
        /// Open child view
        /// </summary>
        #region Command OpenCommand
        public ICommand OpenCommand { get; set; }

        private bool CanOpenCommandExecute(object obj) => CurrentEnterance != null;

        private void OnOpenCommandExecuted(object obj)
        {
            dataStore.CurrentEnterance = CurrentEnterance;
            navigationStore.CurrentViewModel = ServiceLocator.GetService<FlatsListViewModel>();
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
            dataStore.CurrentBuilding = null;
            navigationStore.CurrentViewModel = ServiceLocator.GetService<BuildingsListViewModel>();
        }
        #endregion



        public EnterancesListViewModel(AppDbContext appDbContext, DataStore dataStore, NavigationStore navigationStore)
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
            OnPropertyChanged(nameof(Enterances));
        }
    }
}
