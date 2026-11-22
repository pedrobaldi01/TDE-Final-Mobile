using Workoutly.Mobile.ViewModels;
using Workoutly.Mobile.Services;

namespace Workoutly.Mobile;

public partial class ExercisesPage : ContentPage
{
    private readonly AccountClient _accounts;
    private readonly CatalogViewModel _model;
    private bool _busy;
    private bool _opening;

    public ExercisesPage(AccountClient accounts)
    {
        InitializeComponent();
        _accounts = accounts; BindingContext = _model = new(accounts);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadAsync();
    }

    private async Task LoadAsync()
    {
        if (_busy) return;
        SetBusy(true);
        ErrorText.IsVisible = false;
        ExerciseList.IsVisible = false;
        StatusText.Text = "Carregando exercícios…";
        try
        {
            await _model.LoadAsync();
            StatusText.Text = "Toque em um exercício para ver os detalhes.";
            ExerciseList.IsVisible = true; RefreshButton.Text = "Atualizar";
        }
        catch (SessionExpiredException exception)
        {

            await DisplayAlertAsync("Acesso", exception.Message, "OK");
            await Navigation.PopToRootAsync();
        }
        catch (Exception exception) when (exception is AccountException or SQLite.SQLiteException or IOException)
        {
            ErrorText.Text = exception.Message;
            ErrorText.IsVisible = true;
            StatusText.Text = "Lista não carregada."; RefreshButton.Text = "Tentar novamente";
        }
        finally { SetBusy(false); }
    }

    private async void OnRefresh(object? sender, EventArgs args) => await LoadAsync();

    private async void OnCreate(object? sender, EventArgs args)
    {
        if (_busy || _opening) return;
        _opening = true;
        try { await Navigation.PushAsync(new ExercisePage(_accounts)); }
        finally { _opening = false; }
    }

    private async void OnDetails(object? sender, EventArgs args)
    {
        if (sender is Button { CommandParameter: ExerciseItem item }) await OpenDetails(item);
    }
    private async void OnSelected(object? sender, SelectionChangedEventArgs args)
    {
        if (args.CurrentSelection.FirstOrDefault() is ExerciseItem item) await OpenDetails(item);
    }

    private async Task OpenDetails(ExerciseItem item)
    {
        if (_busy || _opening) return;
        _opening = true;
        ExerciseList.SelectedItem = null;
        try { await Navigation.PushAsync(new ExercisePage(_accounts, item)); }
        finally { _opening = false; }
    }

    private void SetBusy(bool busy)
    {
        _busy = busy;
        Loading.IsVisible = Loading.IsRunning = busy;
        CreateButton.IsEnabled = RefreshButton.IsEnabled = ExerciseList.IsEnabled = !busy;
        Search.IsEnabled = OnlyMine.IsEnabled = !busy && ExerciseList.IsVisible;
    }
}
