using System.Windows;
using HomeCollection.Stores;
using HomeCollection.Utils;
using HomeCollection.ViewModels;
using HomeCollection.Views.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HomeCollection
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IHost _host;

        public App()
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddDbContext<DataBase.AppDbContext>();
                    services.AddSingleton<DataStore>();

                    services.AddSingleton<MainWindow>();
                    services.AddTransient<AddEditBuildingWindow>();
                    services.AddTransient<AddEditEnteranceWindow>();
                    services.AddTransient<AddEditFlatWindow>();
                    services.AddTransient<AddEditPeopleWindow>();

                    services.AddSingleton<NavigationStore>();

                    services.AddTransient<MainWindowViewModel>();
                    services.AddTransient<BuildingsListViewModel>();
                    services.AddTransient<EnterancesListViewModel>();
                    services.AddTransient<FlatsListViewModel>();
                    services.AddTransient<PeoplesListViewModel>();
                })
                .Build();
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await _host.StopAsync(TimeSpan.FromSeconds(5));
            _host.Dispose();
            base.OnExit(e);
        }

        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            await _host.StartAsync();

            ServiceLocator.SetLocatorProvider(_host.Services);

            var mainWindow = _host.Services.GetRequiredService<MainWindow>();
            mainWindow.DataContext = _host.Services.GetRequiredService<MainWindowViewModel>();
            mainWindow.Show();
        }
    }
}
