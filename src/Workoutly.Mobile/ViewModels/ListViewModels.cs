using System.ComponentModel;
using System.Globalization;
using Workoutly.Mobile.Models;
using Workoutly.Mobile.Data;

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

public sealed class PlansViewModel(TemplateDao dao) : SearchList
{
    private PlanItem[] _plans = [];
    private bool _onlyFavorites;
    public bool OnlyFavorites
    {
        get => _onlyFavorites;
        set { _onlyFavorites = value; Refresh(); }
    }
    public PlanItem[] Visible => _plans.Where(p => (!OnlyFavorites || p.Favorite) && Matches(p.Name)).ToArray();
    public Task LoadAsync()
    {
        _plans = dao.List();
        Refresh();
        return Task.CompletedTask;
    }
}

public sealed class CatalogViewModel(TemplateDao dao, HashSet<string> excluded) : SearchList
{
    private ExerciseItem[] _catalog = [];
    public ExerciseItem[] Visible => _catalog.Where(e => !excluded.Contains(e.Id) &&
        Matches($"{e.Name} {e.MuscleGroup} {e.Equipment}")).ToArray();
    public Task LoadAsync()
    {
        _catalog = dao.Catalog().Select(e => new ExerciseItem(e.ExerciseId, e.Name, e.MuscleGroup, e.Equipment, e.Description)).ToArray();
        Refresh();
        return Task.CompletedTask;
    }
}
