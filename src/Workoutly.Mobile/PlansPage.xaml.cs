using Workoutly.Mobile.Models;
using Workoutly.Mobile.Data;
using Workoutly.Mobile.ViewModels;
namespace Workoutly.Mobile;

public partial class PlansPage : ContentPage
{
    private readonly PlanDao _dao;
    private bool _busy;
    private readonly PlansViewModel _model;
    private int _messageNumber;
    public async void ShowMessage(string text)
    {
        var number = ++_messageNumber;
        SnackbarText.Text = text;
        Snackbar.IsVisible = true;
        SemanticScreenReader.Default.Announce(text);
        await Task.Delay(3000);
        if (number == _messageNumber) Snackbar.IsVisible = false;
    }
    public PlansPage(PlanDao dao)
    {
        InitializeComponent();
        _dao = dao;
        BindingContext = _model = new(dao);
    }
    protected override async void OnAppearing() { base.OnAppearing(); await LoadAsync(); }
    private async Task LoadAsync()
    {
        if (_busy) return;
        SetBusy(true);
        ErrorText.IsVisible = false;

        PlansList.IsVisible = false;
        try
        {
            await _model.LoadAsync();
            PlansList.IsVisible = true;
            ListStatus.Text = "Abra uma ficha para editar.";
            RefreshButton.Text = "Atualizar";
        }
        catch (Exception exception) when (exception is SQLite.SQLiteException or IOException)
        {
            ErrorText.Text = exception.Message; ErrorText.IsVisible = true;
            ListStatus.Text = "Lista não carregada."; RefreshButton.Text = "Tentar novamente";
        }
        finally { SetBusy(false); }
    }
    private async void OnRefresh(object? sender, EventArgs args) => await LoadAsync();
    private async void OnCreate(object? sender, EventArgs args) => await OpenAsync(null);
    private async void OnOpen(object? sender, EventArgs args)
    {
        if (sender is Button { CommandParameter: PlanItem plan }) await OpenAsync(plan);
    }
    private async Task OpenAsync(PlanItem? plan)
    {
        if (_busy) return;
        SetBusy(true);
        try { await Navigation.PushAsync(new PlanPage(_dao, plan)); }
        finally { SetBusy(false); }
    }

    private void SetBusy(bool busy)
    {
        _busy = busy;
        Loading.IsVisible = Loading.IsRunning = busy;
        if (busy) ListStatus.Text = "Carregando fichas…";
        CreateButton.IsEnabled = RefreshButton.IsEnabled = PlansList.IsEnabled = !busy;
    }
}
