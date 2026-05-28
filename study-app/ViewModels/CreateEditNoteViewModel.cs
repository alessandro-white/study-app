using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StudyApp.Models;
using StudyApp.Services;

namespace StudyApp.ViewModels;

public partial class CreateEditNoteViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _main;
    private readonly ModuleDetailViewModel _moduleDetail;
    private readonly int _moduleId;
    private readonly Note? _existing;

    [ObservableProperty] private string _front = "";
    [ObservableProperty] private string _back = "";
    [ObservableProperty] private string _errorMessage = "";
    public string Title => _existing == null ? "New Card" : "Edit Card";

    public CreateEditNoteViewModel(MainWindowViewModel main, ModuleDetailViewModel moduleDetail, int moduleId, Note? existing)
    {
        _main = main;
        _moduleDetail = moduleDetail;
        _moduleId = moduleId;
        _existing = existing;
        if (existing != null) { Front = existing.Front; Back = existing.Back; }
    }

    [RelayCommand]
    private void Save()
    {
        ErrorMessage = "";
        if (string.IsNullOrWhiteSpace(Front)) { ErrorMessage = "Front text is required."; return; }
        if (string.IsNullOrWhiteSpace(Back)) { ErrorMessage = "Back text is required."; return; }
        if (_existing == null)
            DatabaseService.Instance.CreateNote(_moduleId, Front, Back);
        else
            DatabaseService.Instance.UpdateNote(_existing.Id, Front, Back);
        _moduleDetail.LoadNotes();
        _main.Navigate(_moduleDetail);
    }

    [RelayCommand]
    private void Cancel() => _main.Navigate(_moduleDetail);
}
