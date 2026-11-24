using Workoutly.Mobile.ViewModels;
using Workoutly.Mobile.Models;
using Workoutly.Mobile.Data;
namespace Workoutly.Mobile;

public partial class PlanExercisePickerPage : ContentPage
{
    private readonly Action<ExerciseItem> _onSelect;
    private readonly CatalogViewModel _model;
    private bool _busy;
    public PlanExercisePickerPage(PlanDao dao, HashSet<string> selected, Action<ExerciseItem> onSelect)
    {
        InitializeComponent(); BindingContext = _model = new(dao, selected); _onSelect = onSelect;
    }
    protected override async void OnAppearing() { base.OnAppearing(); await LoadAsync(); }
    private async Task LoadAsync()
    {
        if (_busy) return;
        SetBusy(true); ErrorText.IsVisible = false;
        ExerciseList.IsVisible = false;
        ListStatus.Text = "Carregando exercícios…";
        try
        {
            await _model.LoadAsync(); ExerciseList.IsVisible = true;
            RefreshButton.Text = "Atualizar"; ListStatus.Text = "Toque para incluir. Exercícios já adicionados ficam ocultos.";
        }
        catch (Exception exception) when (exception is SQLite.SQLiteException or IOException)
        {
            ErrorText.Text = exception.Message; ErrorText.IsVisible = true;
            ListStatus.Text = "Lista não carregada."; RefreshButton.Text = "Tentar novamente";
        }
        finally { SetBusy(false); }
    }
    private async void OnRefresh(object? sender, EventArgs args) => await LoadAsync();
    private async void OnCancel(object? sender, EventArgs args) => await Navigation.PopModalAsync();
    private async void OnInclude(object? sender, EventArgs args)
    {
        if (_busy || sender is not Button { CommandParameter: ExerciseItem item }) return;
        SetBusy(true);
        try { _onSelect(item); await Navigation.PopModalAsync(); }
        finally { SetBusy(false); }
    }
    private void SetBusy(bool busy)
    {
        _busy = busy; Loading.IsVisible = Loading.IsRunning = busy;
        ExerciseList.IsEnabled = CreateButton.IsEnabled = RefreshButton.IsEnabled = !busy;
        Search.IsEnabled = !busy && ExerciseList.IsVisible;
    }
}
