# HomeCollection
#### Application for keeping records of houses, entrances, apartments and people. 
#### Developed in WPF (C#) using the MVVM pattern 🖥️
#### In this Application you won't find validations of model or anythink like that. There is only navigation, CRUD actions for using like some template for future projects. Thank's! 💖
#### If you find something that can be implemented better, please let me know. I will be glad to learn and use the best practices 🔍
#### And if you like the implementation of this project, please Star this repo 🌟

### Content
* [Command](#command)
* [Host](#host)
* [Navigation](#navigation)
* [ERD](#erd)
* [NuGet packages](#nuget)

------------
# <a name="command"></a> Command 
### Base command
```csharp
public abstract class Command : ICommand
{
    public event EventHandler? CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }

    public abstract bool CanExecute(object? parameter);

    public abstract void Execute(object? parameter);
}
```
### Initialization command
```csharp
public class LambdaCommand : Command
{
    private readonly Action<object> _execute;
    private readonly Func<object, bool> _canExecute;

    public LambdaCommand(Action<object> execute, Func<object, bool> canExecute = null)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    public override bool CanExecute(object? parameter)
    {
        return _canExecute.Invoke(parameter);
    }

    public override void Execute(object? parameter)
    {
        _execute(parameter);
    }
}
```

------------
# <a name="host"></a> Host 
#### All of it in App.xaml.cs

### Field IHost
```csharp
private readonly IHost _host;
```
### Configuring and building Host
```csharp
public App()
{
    _host = Host.CreateDefaultBuilder()
        .ConfigureServices((context, services) =>
        {
            services.AddSingleton<...>();
            services.AddTransient<...>();
        })
        .Build();
}
```
### Disposing Host on closing the Application
```csharp
protected override async void OnExit(ExitEventArgs e)
{
    await _host.StopAsync(TimeSpan.FromSeconds(5));
    _host.Dispose();
    base.OnExit(e);
}
```
### Startup Host, Setting ServiceLocator and showing MainWindow
```csharp
private async void Application_Startup(object sender, StartupEventArgs e)
{
    await _host.StartAsync();

    ServiceLocator.SetLocatorProvider(_host.Services);

    var mainWindow = _host.Services.GetRequiredService<MainWindow>();
    mainWindow.DataContext = _host.Services.GetRequiredService<MainWindowViewModel>();
    mainWindow.Show();
}
```
------------
# <a name="navigation"></a> Navigation
### Navigation store
```csharp
public class NavigationStore
{
    private ViewModel _currentViewModel;
    public ViewModel CurrentViewModel
    {
        get => _currentViewModel;
        set
        {
            _currentViewModel?.Dispose();
            _currentViewModel = value;
            OnCurrentViewModelChanged();
        }
    }

    public event Action CurrentViewModelChanged;

    private void OnCurrentViewModelChanged()
    {
        CurrentViewModelChanged?.Invoke();
    }
}
```
### Need to register this store as Singltone service in Host
```csharp
services.AddSingleton<NavigationStore>();
```
### In MainWindowViewModel setting action on CurrentViewModel was changed
```csharp
public class MainWindowViewModel : ViewModel
{
    private readonly NavigationStore navigationStore;

    public ViewModel CurrentViewModel => navigationStore.CurrentViewModel;

    public MainWindowViewModel(NavigationStore navigationStore)
    {
        this.navigationStore = navigationStore;
        navigationStore.CurrentViewModelChanged += NavigationStore_CurrentViewModelChanged;
    }

    private void NavigationStore_CurrentViewModelChanged()
    {
        OnPropertyChanged(nameof(CurrentViewModel));
    }
}
```
------------
# <a name="erd"></a> ERD

**Buildings** (∞)⇆(1) **Enterances** (∞)⇆(1) **Flats** (∞)⇆(1) **Peoples**

<table><thead>
  <tr>
    <th colspan="4">Building</th>
  </tr>
<tr>
    <th>IsPK</th>
    <th>IsFK</th>
    <th>Name</th>
    <th>Type</th>
  </tr></thead>
<tbody>
  <tr>
    <td>✅</td>
    <td></td>
    <td>Id</td>
    <td>String</td>
  </tr>
  <tr>
    <td></td>
    <td></td>
    <td>ShortName</td>
    <td>String</td>
  </tr>
  <tr>
    <td></td>
    <td></td>
    <td>Address</td>
    <td>String</td>
  </tr>
</tbody>
</table>

<table><thead>
  <tr>
    <th colspan="4">Enterance</th>
  </tr>
<tr>
    <th>IsPK</th>
    <th>IsFK</th>
    <th>Name</th>
    <th>Type</th>
  </tr></thead>
<tbody>
  <tr>
    <td>✅</td>
    <td></td>
    <td>Id</td>
    <td>String</td>
  </tr>
  <tr>
    <td></td>
    <td></td>
    <td>Number</td>
    <td>Int</td>
  </tr>
  <tr>
    <td></td>
    <td>✅</td>
    <td>BuildingId</td>
    <td>String</td>
  </tr>
</tbody>
</table>

<table><thead>
  <tr>
    <th colspan="4">Flat</th>
  </tr>
<tr>
    <th>IsPK</th>
    <th>IsFK</th>
    <th>Name</th>
    <th>Type</th>
  </tr></thead>
<tbody>
  <tr>
    <td>✅</td>
    <td></td>
    <td>Id</td>
    <td>String</td>
  </tr>
  <tr>
    <td></td>
    <td></td>
    <td>Number</td>
    <td>Int</td>
  </tr>
  <tr>
    <td></td>
    <td>✅</td>
    <td>EnteranceId</td>
    <td>String</td>
  </tr>
</tbody>
</table>

<table><thead>
  <tr>
    <th colspan="4">People</th>
  </tr>
<tr>
    <th>IsPK</th>
    <th>IsFK</th>
    <th>Name</th>
    <th>Type</th>
  </tr></thead>
<tbody>
  <tr>
    <td>✅</td>
    <td></td>
    <td>Id</td>
    <td>String</td>
  </tr>
  <tr>
    <td></td>
    <td></td>
    <td>FullName</td>
    <td>String</td>
  </tr>
  <tr>
    <td></td>
    <td></td>
    <td>PhoneNumber</td>
    <td>String</td>
  </tr>
  <tr>
    <td></td>
    <td></td>
    <td>DateBirth</td>
    <td>DateTime</td>
  </tr>
  <tr>
    <td></td>
    <td>✅</td>
    <td>FlatId</td>
    <td>String</td>
  </tr>
</tbody>
</table>

------------
# <a name="nuget"></a> Used NuGet packages

<table><thead>
<tr>
    <th>Name</th>
    <th>Version</th>
	<th>License</th>
  </tr></thead>
<tbody>
  <tr>
    <td><a href="https://www.nuget.org/packages/Microsoft.EntityFrameworkCore/">Microsoft.EntityFrameworkCore</a></td>
    <td>8.0.5</td>
	<td>MIT</td>
  </tr>
  <tr>
    <td><a href="https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Design/">Microsoft.EntityFrameworkCore.Design</a></td>
    <td>8.0.5</td>
	<td>MIT</td>
  </tr>
  <tr>
    <td><a href="https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Sqlite/">Microsoft.EntityFrameworkCore.Sqlite</a></td>
    <td>8.0.5</td>
	<td>MIT</td>
  </tr>
  <tr>
    <td><a href="https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Tools/">Microsoft.EntityFrameworkCore.Tools</a></td>
    <td>8.0.5</td>
	<td>MIT</td>
  </tr>
  <tr>
    <td><a href="https://www.nuget.org/packages/Microsoft.Extensions.Hosting/">Microsoft.Extensions.Hosting</a></td>
    <td>8.0.0</td>
	<td>MIT</td>
  </tr>
  <tr>
    <td><a href="https://www.nuget.org/packages/Microsoft.Extensions.Hosting.Abstractions/">Microsoft.Extensions.Hosting.Abstractions</a></td>
    <td>8.0.0</td>
	<td>MIT</td>
  </tr>
</tbody>
</table>
