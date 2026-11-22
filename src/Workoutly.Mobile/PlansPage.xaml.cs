using Workoutly.Mobile.Services;
using Workoutly.Mobile.ViewModels;
namespace Workoutly.Mobile;

public partial class PlansPage : ContentPage
{
    private readonly AccountClient _accounts;
    private readonly string? _studentId;
    private bool _busy;
    private readonly bool _local;
    private readonly Action<PlanItem>? _select;
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
    public bool CanStartWorkout => _studentId is null && !_local;
    public PlansPage(AccountClient accounts, string? studentId = null, string? studentName = null, bool local = false, Action<PlanItem>? select = null)
    {
        InitializeComponent();
        _accounts = accounts;
        _studentId = studentId;
        _local = local;
        _select = select;
        BindingContext = _model = new(accounts, studentId, local);
        if (local)
        {
            Title = Heading.Text = "Modelos no aparelho";
            Subtitle.Text = select is null ? "Modelos desta conta. Favoritos aparecem primeiro." : "Escolha um modelo para preencher a ficha.";
            CreateButton.Text = select is null ? "Novo modelo" : "Cancelar";
        }
        OnPropertyChanged(nameof(CanStartWorkout));
        if (studentId is not null)
        {
            Heading.Text = "Fichas do aluno";
            Subtitle.Text = studentName;
        }
    }
    protected override async void OnAppearing() { base.OnAppearing(); await LoadAsync(); }
    private async Task LoadAsync()
    {
        if (_busy) return;
        SetBusy(true);
        ErrorText.IsVisible = false;
        // Não manter dados de um aluno visíveis após perder o vínculo.

        PlansList.IsVisible = false;
        try
        {
            await _model.LoadAsync();
            PlansList.IsVisible = true;
            ListStatus.Text = _select is null ? "Abra um registro para editar." : "Toque para usar.";
            RefreshButton.Text = "Atualizar";
        }
        catch (SessionExpiredException exception)
        {
            await DisplayAlertAsync("Acesso", exception.Message, "OK");
            await Navigation.PopToRootAsync();
        }
        catch (Exception exception) when (exception is AccountException or SQLite.SQLiteException or IOException)
        {
            ErrorText.Text = exception.Message; ErrorText.IsVisible = true;
            ListStatus.Text = "Lista não carregada."; RefreshButton.Text = "Tentar novamente";
        }
        finally { SetBusy(false); }
    }
    private async void OnRefresh(object? sender, EventArgs args) => await LoadAsync();
    private async void OnCreate(object? sender, EventArgs args) {
        if (_select is not null) await Navigation.PopModalAsync();
        else await OpenAsync(null);
    }
    private async void OnOpen(object? sender, EventArgs args)
    {
        if (_busy || sender is not Button { CommandParameter: PlanItem plan }) return;
        if (_select is null) await OpenAsync(plan);
        else
        {
            _busy = true;
            try { _select(plan); await Navigation.PopModalAsync(); }
            finally { _busy = false; }
        }
    }
    private async Task OpenAsync(PlanItem? plan)
    {
        if (_busy) return;
        SetBusy(true);
        try { await Navigation.PushAsync(new PlanPage(_accounts, _studentId, plan, _local)); }
        finally { SetBusy(false); }
    }
    private async void OnStartWorkout(object? sender, EventArgs args)
    {
        if (_busy || _local || _studentId is not null || sender is not Button { CommandParameter: PlanItem plan }) return;
        SetBusy(true); ErrorText.IsVisible = false;
        try
        {
            var current = await _accounts.GetPlanAsync(null, plan.Id);
            await Navigation.PushAsync(new WorkoutPage(_accounts, current));
        }
        catch (SessionExpiredException exception)
        {
            await DisplayAlertAsync("Acesso", exception.Message, "OK"); await Navigation.PopToRootAsync();
        }
        catch (Exception exception) when (exception is AccountException or SQLite.SQLiteException or IOException) { ErrorText.Text = exception.Message; ErrorText.IsVisible = true; }
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
