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
    public class PeoplesListViewModel : ViewModel
    {
        private readonly AppDbContext appDbContext;
        private readonly DataStore dataStore;
        private readonly NavigationStore navigationStore;
        private AddEditPeopleWindow addEditPeopleWindow;

        public IEnumerable<People> Peoples => appDbContext.Peoples.Where(x => x.Flat == dataStore.CurrentFlat).ToList();

        #region CurrentPeople
        private People _currentPeople;
        public People CurrentPeople { get => _currentPeople; set => Set(ref _currentPeople, value); }
        #endregion

        /// <summary>
        /// Add new record
        /// </summary>
        #region Command AddCommand
        public ICommand AddCommand { get; set; }

        private bool CanAddCommandExecute(object obj) => true;

        private void OnAddCommandExecuted(object obj)
        {
            CurrentPeople = new People();
            CurrentPeople.DateBirth = DateTime.Now;
            addEditPeopleWindow = ServiceLocator.GetService<AddEditPeopleWindow>();
            addEditPeopleWindow.DataContext = this;
            addEditPeopleWindow.ShowDialog();
        }
        #endregion

        /// <summary>
        /// Remove selected record
        /// </summary>
        #region Command RemoveCommand
        public ICommand RemoveCommand { get; set; }

        private bool CanRemoveCommandExecute(object obj) => CurrentPeople != null;

        private void OnRemoveCommandExecuted(object obj)
        {
            if (MessageBox.Show($"Вы точно хотите удалить запись:\n{CurrentPeople.Id}\n{CurrentPeople.FullName}\n{CurrentPeople.PhoneNumber}\n{CurrentPeople.DateBirth.ToString("dd.MM.yyyy")}", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                People people = (People)obj;
                appDbContext.Remove(people);
                appDbContext.SaveChanges();
            }
        }
        #endregion

        /// <summary>
        /// Open adding/editing window with selected record
        /// </summary>
        #region Command EditCommand
        public ICommand EditCommand { get; set; }

        private bool CanEditCommandExecute(object obj) => CurrentPeople != null;

        private void OnEditCommandExecuted(object obj)
        {
            People people = (People)obj;
            CurrentPeople = people;
            addEditPeopleWindow = ServiceLocator.GetService<AddEditPeopleWindow>();
            addEditPeopleWindow.DataContext = this;
            addEditPeopleWindow.ShowDialog();
        }
        #endregion

        /// <summary>
        /// Save data from adding/editing window and close it
        /// </summary>
        #region Command SaveAndCloseCommand
        public ICommand SaveAndCloseCommand { get; set; }

        private bool CanSaveAndCloseCommandExecute(object obj)
        {
            if (CurrentPeople == null)
                return false;
            if (string.IsNullOrEmpty(CurrentPeople.FullName) ||
                string.IsNullOrEmpty(CurrentPeople.PhoneNumber))
                return false;
            if (CurrentPeople.FullName.Length < 10 || CurrentPeople.PhoneNumber.Length < 10)
                return false;
            return true;
        }

        private void OnSaveAndCloseCommandExecuted(object obj)
        {
            CurrentPeople.Flat = dataStore.CurrentFlat;
            if (appDbContext.Peoples.Any(x => x.Id == CurrentPeople.Id))
            {
                appDbContext.Peoples.Update(CurrentPeople);
            }
            else
            {
                appDbContext.Peoples.Add(CurrentPeople);
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
            CurrentPeople = null;
            addEditPeopleWindow.Close();
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
            dataStore.CurrentFlat = null;
            navigationStore.CurrentViewModel = ServiceLocator.GetService<FlatsListViewModel>();
        }
        #endregion



        public PeoplesListViewModel(AppDbContext appDbContext, DataStore dataStore, NavigationStore navigationStore)
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
            BackCommand = new LambdaCommand(OnBackCommandExecuted, CanBackCommandExecute);
        }

        /// <summary>
        /// When changes saved in DB
        /// Need to update flats list
        /// </summary>
        private void OnAppDbContext_SavedChanges(object? sender, SavedChangesEventArgs e)
        {
            OnPropertyChanged(nameof(Peoples));
        }
    }
}
