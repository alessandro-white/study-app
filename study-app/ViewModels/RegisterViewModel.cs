using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StudyApp.Services;

namespace StudyApp.ViewModels;

public partial class RegisterViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _main;

    [ObservableProperty] private string _username = "";
    [ObservableProperty] private string _email = "";
    [ObservableProperty] private string _password = "";
    [ObservableProperty] private string _confirmPassword = "";
    [ObservableProperty] private string _errorMessage = "";

    public RegisterViewModel(MainWindowViewModel main) => _main = main;

    [RelayCommand]
    private void Register()
    {
        ErrorMessage = "";
        if (Password != ConfirmPassword) { ErrorMessage = "Passwords do not match."; return; }
        var (success, error) = AuthService.Instance.Register(Username, Email, Password);
        if (success)
            _main.Navigate(new DashboardViewModel(_main));
        else
            ErrorMessage = error;
    }

    [RelayCommand]
    private void GoToLogin() => _main.Navigate(new LoginViewModel(_main));
}
