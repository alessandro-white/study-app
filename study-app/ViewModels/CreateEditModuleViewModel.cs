using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StudyApp.Models;
using StudyApp.Services;

namespace StudyApp.ViewModels;

public partial class CreateEditModuleViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _main;
    private readonly DashboardViewModel _dashboard;
    private readonly Module? _existing;

    [ObservableProperty] private string _name = "";
    [ObservableProperty] private string _description = "";
    [ObservableProperty] private string _color = "#007AFF";
    [ObservableProperty] private string _errorMessage = "";
    public string Title => _existing == null ? "New Module" : "Edit Module";

    public string[] ColorOptions { get; } = ["#007AFF", "#34C759", "#FF9500", "#FF3B30", "#AF52DE", "#5856D6", "#FF2D55", "#00C7BE"];

    public CreateEditModuleViewModel(MainWindowViewModel main, DashboardViewModel dashboard, Module? existing)
    {
        _main = main;
        _dashboard = dashboard;
        _existing = existing;
        if (existing != null) { Name = existing.Name; Description = existing.Description; Color = existing.Color; }
    }

    [RelayCommand]
    private void SelectColor(string color) => Color = color;

    [RelayCommand]
    private void Save()
    {
        ErrorMessage = "";
        if (string.IsNullOrWhiteSpace(Name)) { ErrorMessage = "Name is required."; return; }
        if (_existing == null)
            DatabaseService.Instance.CreateModule(AuthService.Instance.CurrentUser!.Id, Name, Description, Color);
        else
            DatabaseService.Instance.UpdateModule(_existing.Id, Name, Description, Color);
        _dashboard.LoadModules();
        _main.Navigate(_dashboard);
    }

    [RelayCommand]
    private void Cancel() => _main.Navigate(_dashboard);
}
