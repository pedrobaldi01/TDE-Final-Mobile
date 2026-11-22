using Workoutly.Mobile.Services;
using Workoutly.Mobile.ViewModels;

namespace Workoutly.Mobile;

public partial class PlanPage : ContentPage, IConfirmLeave
{
    private readonly AccountClient _accounts;
    private readonly PlanEditorViewModel _editor;
    private bool _busy, _allowLeave;

    public PlanPage(AccountClient accounts, string? studentId, PlanItem? plan, bool local = false)
    {
        InitializeComponent();
        _accounts = accounts;
        _editor = new(accounts, studentId, local);
        if (plan is not null) _editor.Fill(plan, false);
        _editor.Remember();
        BindEditor();
        Title = local ? "Modelo no aparelho" : "Ficha de treino";
        Heading.Text = local ? (plan is null ? "Novo modelo" : "Editar modelo") : (plan is null ? "Nova ficha" : "Editar ficha");
        UseTemplate.IsVisible = !local;
        FavoritePanel.IsVisible = local;
        DeleteButton.IsVisible = ReloadButton.IsVisible = plan is not null;
        if (local) { ReloadButton.Text = "Recarregar modelo"; DeleteButton.Text = "Excluir modelo"; }
        SaveButton.Text = local ? "Salvar modelo" : "Salvar ficha";
        FormFeedback.FollowError(PageScroll, ErrorText);
    }

    private void BindEditor() { BindingContext = null; BindingContext = _editor; }
    public async Task<bool> CanLeaveAsync() => _allowLeave || (!_busy && (!_editor.Changed ||
        await DisplayAlertAsync("Descartar alterações?", "O conteúdo em edição ainda não foi salvo.", "Descartar", "Continuar editando")));

    private async void OnAdd(object? sender, EventArgs args)
    {
        if (_editor.Items.Count >= 50) { ShowError("Use até 50 exercícios."); return; }
        await ExecuteAsync(() => Navigation.PushModalAsync(new PlanExercisePickerPage(_accounts,
            _editor.Items.Select(r => r.Exercise.ExerciseId).ToHashSet(), item =>
            _editor.Items.Add(new(new(item.Id, item.Name, item.MuscleGroup, item.Equipment, item.Description, 3, 10))), _editor.Local)));
    }

    private async void OnUseTemplate(object? sender, EventArgs args) => await ExecuteAsync(async () =>
    {
        if ((_editor.Items.Count > 0 || !string.IsNullOrWhiteSpace(_editor.Name) || !string.IsNullOrWhiteSpace(_editor.Notes)) &&
            !await DisplayAlertAsync("Preencher com modelo?", "Ao escolher, os campos serão substituídos. A ficha só muda ao salvar.", "Escolher", "Cancelar")) return;
        await Navigation.PushModalAsync(new PlansPage(_accounts, local: true, select: template =>
        {
            _editor.Fill(template, true);
            BindEditor();
        }));
    });

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
        await _editor.SaveAsync();
        _allowLeave = true;
        await Navigation.PopAsync();
        if (Shell.Current.CurrentPage is PlansPage list) list.ShowMessage(_editor.Local ? "Modelo salvo neste aparelho" : "Ficha salva para o aluno");
    });
    private async void OnReload(object? sender, EventArgs args) => await ExecuteAsync(async () =>
    {
        if (!await DisplayAlertAsync("Recarregar?", "Substituir os campos pela versão salva?", "Recarregar", "Cancelar")) return;
        _editor.Fill(await _editor.ReloadAsync(), false);
        BindEditor();
        _editor.Remember();
    });
    private async void OnDelete(object? sender, EventArgs args) => await ExecuteAsync(async () =>
    {
        var question = _editor.Local ? "Excluir modelo deste aparelho?" : "Excluir ficha e preservar os treinos já feitos?";
        if (!await DisplayAlertAsync(question, _editor.Name, "Excluir", "Cancelar")) return;
        await _editor.DeleteAsync();
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
        catch (SessionExpiredException e) { await DisplayAlertAsync("Acesso", e.Message, "OK"); _allowLeave = true; await Navigation.PopToRootAsync(); }
        catch (Exception e) when (e is AccountException or SQLite.SQLiteException or InvalidOperationException or IOException)
        { ShowError(e.Message); }
        finally { _busy = false; Form.IsEnabled = true; Loading.IsVisible = Loading.IsRunning = false; }
    }
    private void ShowError(string message) { ErrorText.Text = message; ErrorText.IsVisible = true; }
}
