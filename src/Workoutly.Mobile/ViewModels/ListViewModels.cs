using System.ComponentModel;
using System.Globalization;
using Workoutly.Mobile.Services;

namespace Workoutly.Mobile.ViewModels;

public abstract class SearchList : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    private string _query = "";
    public string Query
    {
        get => _query;
        set { _query = value ?? ""; Refresh(); }
    }
    protected bool Matches(string value) => CultureInfo.CurrentCulture.CompareInfo.IndexOf(value, Query.Trim(),
        CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace) >= 0;
    protected void Refresh() => PropertyChanged?.Invoke(this, new(null));
}

public sealed class PlansViewModel(AccountClient accounts, string? student, bool local) : SearchList
{
    private PlanItem[] _plans = [];
    public PlanItem[] Visible => _plans.Where(p => Matches(p.Name)).ToArray();
    public async Task LoadAsync()
    {
        _plans = [];
        Refresh();
        _plans = local ? accounts.Templates.List() : await accounts.GetPlansAsync(student);
        Refresh();
    }
}

public sealed class CatalogViewModel(AccountClient accounts, HashSet<string>? excluded = null, bool local = false) : SearchList
{
    private ExerciseItem[] _catalog = [];
    private bool _onlyMine;
    public bool OnlyMine
    {
        get => _onlyMine;
        set { _onlyMine = value; Refresh(); }
    }
    public ExerciseItem[] Visible => _catalog.Where(e => (excluded is null || !excluded.Contains(e.Id)) &&
        (!OnlyMine || e.IsMine) && Matches($"{e.Name} {e.MuscleGroup} {e.Equipment}")).ToArray();
    public async Task LoadAsync()
    {
        _catalog = [];
        Refresh();
        _catalog = local ? accounts.Templates.Catalog().Select(e => new ExerciseItem(e.ExerciseId, e.Name, e.MuscleGroup, e.Equipment, e.Description, true)).ToArray()
            : await accounts.GetExercisesAsync();
        Refresh();
    }
}
