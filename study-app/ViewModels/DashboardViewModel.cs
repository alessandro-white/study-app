using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StudyApp.Models;
using StudyApp.Services;

namespace StudyApp.ViewModels;

public partial class DashboardViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _main;

    [ObservableProperty] private ObservableCollection<Module> _modules = new();
    [ObservableProperty] private string _welcomeText = "";

    public DashboardViewModel(MainWindowViewModel main)
    {
        _main = main;
        WelcomeText = $"Welcome, {AuthService.Instance.CurrentUser!.Username}!";
        LoadModules();
    }

    public void LoadModules()
    {
        var modules = DatabaseService.Instance.GetModules(AuthService.Instance.CurrentUser!.Id);
        Modules = new ObservableCollection<Module>(modules);
    }

    [RelayCommand]
    private void AddModule() => _main.Navigate(new CreateEditModuleViewModel(_main, this, null));

    [RelayCommand]
    private void EditModule(Module m) => _main.Navigate(new CreateEditModuleViewModel(_main, this, m));

    [RelayCommand]
    private void DeleteModule(Module m)
    {
        DatabaseService.Instance.DeleteModule(m.Id);
        LoadModules();
    }

    [RelayCommand]
    private void OpenModule(Module m) => _main.Navigate(new ModuleDetailViewModel(_main, this, m));

    [RelayCommand]
    private void StudyModule(Module m)
    {
        var due = DatabaseService.Instance.GetDueNotes(m.Id);
        if (due.Count == 0) return;
        _main.Navigate(new ReviewViewModel(_main, this, m, due));
    }

    [RelayCommand]
    private void Logout()
    {
        AuthService.Instance.Logout();
        _main.Navigate(new LoginViewModel(_main));
    }
}
