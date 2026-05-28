using CommunityToolkit.Mvvm.ComponentModel;

namespace StudyApp.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private ViewModelBase _currentView;

    public MainWindowViewModel()
    {
        _currentView = new LoginViewModel(this);
    }

    public void Navigate(ViewModelBase vm) => CurrentView = vm;
}
