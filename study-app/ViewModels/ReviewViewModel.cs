using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StudyApp.Models;
using StudyApp.Services;

namespace StudyApp.ViewModels;

public partial class ReviewViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _main;
    private readonly DashboardViewModel _dashboard;
    private readonly Module _module;
    private readonly List<Note> _queue;
    private int _index;

    [ObservableProperty] private Note _currentNote;
    [ObservableProperty] private bool _isFlipped;
    [ObservableProperty] private int _reviewed;
    [ObservableProperty] private int _total;
    [ObservableProperty] private string _moduleName;

    public ReviewViewModel(MainWindowViewModel main, DashboardViewModel dashboard, Module module, List<Note> due)
    {
        _main = main;
        _dashboard = dashboard;
        _module = module;
        _queue = due;
        _index = 0;
        _total = due.Count;
        _moduleName = module.Name;
        _currentNote = _queue[0];
    }

    [RelayCommand]
    private void Flip() => IsFlipped = !IsFlipped;

    [RelayCommand]
    private void Rate(string qualityStr)
    {
        int quality = int.Parse(qualityStr);
        SpacedRepetitionService.ApplyReview(CurrentNote, quality);
        Reviewed++;
        _index++;
        IsFlipped = false;

        if (_index >= _queue.Count)
        {
            _dashboard.LoadModules();
            _main.Navigate(_dashboard);
        }
        else
        {
            CurrentNote = _queue[_index];
        }
    }

    [RelayCommand]
    private void Quit()
    {
        _dashboard.LoadModules();
        _main.Navigate(_dashboard);
    }
}
