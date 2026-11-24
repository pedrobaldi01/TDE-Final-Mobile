using Workoutly.Mobile.Models;
using Workoutly.Mobile.Data;
using Workoutly.Mobile.ViewModels;

namespace Workoutly.Mobile;

public partial class PlanPage : ContentPage, IConfirmLeave
{
    private readonly PlanDao _dao;
    private readonly PlanEditorViewModel _editor;
    private bool _busy, _allowLeave;

    public PlanPage(PlanDao dao, PlanItem? plan)
    {
        InitializeComponent();
        _dao = dao;
        _editor = new(dao);
        if (plan is not null) _editor.Fill(plan);
        _editor.Remember();
        BindEditor();
        Title = "Ficha";
        Heading.Text = plan is null ? "Nova ficha" : "Editar ficha";
        DeleteButton.IsVisible = ReloadButton.IsVisible = plan is not null;
        FormFeedback.FollowError(PageScroll, ErrorText);
    }

    private void BindEditor() { BindingContext = null; BindingContext = _editor; }
    public async Task<bool> CanLeaveAsync() => _allowLeave || (!_busy && (!_editor.Changed ||
        await DisplayAlertAsync("Descartar alterações?", "O conteúdo em edição ainda não foi salvo.", "Descartar", "Continuar editando")));

    private async void OnAdd(object? sender, EventArgs args)
    {
        if (_editor.Items.Count >= 50) { ShowError("Use até 50 exercícios."); return; }
        await ExecuteAsync(() => Navigation.PushModalAsync(new PlanExercisePickerPage(_dao,
            _editor.Items.Select(r => r.Exercise.ExerciseId).ToHashSet(), item =>
            _editor.Items.Add(new(new(item.Id, item.Name, item.MuscleGroup, item.Equipment, 3, 10))))));
    }

    private async void OnItemAction(object? sender, EventArgs args)
    {
        if (_busy || sender is not Button { BindingContext: ExerciseFields item } button) return;
        var index = _editor.Items.IndexOf(item);
        if (index < 0) return;
        if (button.Text == "Remover")
        {
            if (await DisplayAlertAsync("Remover exercício?", item.Exercise.Name, "Remover", "Cancelar")) _editor.Items.Remove(item);
            return;
        }
        var next = index + (button.Text == "Subir" ? -1 : 1);
        if (next >= 0 && next < _editor.Items.Count) _editor.Items.Move(index, next);
    }

    private async void OnSave(object? sender, EventArgs args) => await ExecuteAsync(async () =>
    {
        _editor.Save();
        _allowLeave = true;
        await Navigation.PopAsync();
        if (Shell.Current.CurrentPage is PlansPage list) list.ShowMessage("Ficha salva neste aparelho");
    });
    private async void OnReload(object? sender, EventArgs args) => await ExecuteAsync(async () =>
    {
        if (!await DisplayAlertAsync("Recarregar?", "Substituir os campos pela versão salva?", "Recarregar", "Cancelar")) return;
        _editor.Fill(_editor.Reload());
        BindEditor();
        _editor.Remember();
    });
    private async void OnDelete(object? sender, EventArgs args) => await ExecuteAsync(async () =>
    {
        var question = "Excluir ficha deste aparelho?";
        if (!await DisplayAlertAsync(question, _editor.Name, "Excluir", "Cancelar")) return;
        _editor.Delete();
        _allowLeave = true;
        await Navigation.PopAsync();
    });

    private async Task ExecuteAsync(Func<Task> action)
    {
        if (_busy) return;
        _busy = true;
        Form.IsEnabled = false;
        Loading.IsVisible = Loading.IsRunning = true;
        ErrorText.IsVisible = false;
        try { await action(); }
        catch (Exception e) when (e is ArgumentException or SQLite.SQLiteException or InvalidOperationException or IOException)
        { ShowError(e.Message); }
        finally { _busy = false; Form.IsEnabled = true; Loading.IsVisible = Loading.IsRunning = false; }
    }
    private void ShowError(string message) { ErrorText.Text = message; ErrorText.IsVisible = true; }
}