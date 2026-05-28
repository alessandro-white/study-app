using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StudyApp.Models;
using StudyApp.Services;

namespace StudyApp.ViewModels;

public partial class ModuleDetailViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _main;
    private readonly DashboardViewModel _dashboard;

    public Module Module { get; }
    [ObservableProperty] private ObservableCollection<Note> _notes = new();

    public ModuleDetailViewModel(MainWindowViewModel main, DashboardViewModel dashboard, Module module)
    {
        _main = main;
        _dashboard = dashboard;
        Module = module;
        LoadNotes();
    }

    public void LoadNotes()
    {
        var notes = DatabaseService.Instance.GetNotes(Module.Id);
        Notes = new ObservableCollection<Note>(notes);
    }

    [RelayCommand]
    private void AddNote() => _main.Navigate(new CreateEditNoteViewModel(_main, this, Module.Id, null));

    [RelayCommand]
    private void EditNote(Note n) => _main.Navigate(new CreateEditNoteViewModel(_main, this, Module.Id, n));

    [RelayCommand]
    private void DeleteNote(Note n)
    {
        DatabaseService.Instance.DeleteNote(n.Id);
        LoadNotes();
    }

    [RelayCommand]
    private void Back()
    {
        _dashboard.LoadModules();
        _main.Navigate(_dashboard);
    }

    [RelayCommand]
    private void Study()
    {
        var due = DatabaseService.Instance.GetDueNotes(Module.Id);
        if (due.Count == 0) return;
        _main.Navigate(new ReviewViewModel(_main, _dashboard, Module, due));
    }
}
