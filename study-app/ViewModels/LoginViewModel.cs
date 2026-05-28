using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StudyApp.Services;

namespace StudyApp.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _main;

    [ObservableProperty] private string _usernameOrEmail = "";
    [ObservableProperty] private string _password = "";
    [ObservableProperty] private string _errorMessage = "";

    public LoginViewModel(MainWindowViewModel main) => _main = main;

    [RelayCommand]
    private void Login()
    {
        ErrorMessage = "";
        var (success, error) = AuthService.Instance.Login(UsernameOrEmail, Password);
        if (success)
            _main.Navigate(new DashboardViewModel(_main));
        else
            ErrorMessage = error;
    }

    [RelayCommand]
    private void GoToRegister() => _main.Navigate(new RegisterViewModel(_main));
}
