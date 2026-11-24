using System.Collections.ObjectModel;
using System.Globalization;
using Workoutly.Mobile.Data;
using Workoutly.Mobile.Models;

namespace Workoutly.Mobile.ViewModels;

public sealed class ExerciseFields(PlanExerciseItem exercise)
{
    public PlanExerciseItem Exercise { get; } = exercise;
    public string Sets { get; set; } = exercise.Sets.ToString();
    public string Repetitions { get; set; } = exercise.Repetitions.ToString();
    public string Weight { get; set; } = exercise.SuggestedWeightKg?.ToString("0.##") ?? "";

    public PlanExerciseItem Read()
    {
        if (!int.TryParse(Sets, out var sets) || sets is < 1 or > 20 ||
            !int.TryParse(Repetitions, out var reps) || reps is < 1 or > 200)
            throw new ArgumentException($"Revise {Exercise.Name}: use 1 a 20 séries e 1 a 200 repetições.");
        decimal? kg = null;
        if (!string.IsNullOrWhiteSpace(Weight))
        {
            if (!decimal.TryParse(Weight.Trim().Replace(',', '.'), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var weight)
                || weight is < 0 or > 2000 || decimal.Round(weight, 2) != weight)
                throw new ArgumentException($"Revise a carga de {Exercise.Name}: 0 a 2000 kg com até duas casas decimais.");
            kg = weight;
        }
        return Exercise with { Sets = sets, Repetitions = reps, SuggestedWeightKg = kg };
    }
}

public sealed class PlanEditorViewModel(TemplateDao dao)
{
    private string _id = Guid.NewGuid().ToString();
    private int _version;
    private string[] _baseline = [];
    public string Name { get; set; } = "";
    public string Notes { get; set; } = "";
    public bool Favorite { get; set; }
    public ObservableCollection<ExerciseFields> Items { get; } = [];
    public bool SavedBefore => _version > 0;
    public bool Changed => !_baseline.SequenceEqual(Values());
    public void Remember() => _baseline = Values();
    private string[] Values() => new[] { Name, Notes, Favorite.ToString() }.Concat(Items.SelectMany(i =>
        new[] { i.Exercise.ExerciseId, i.Sets, i.Repetitions, i.Weight })).ToArray();

    public void Fill(PlanItem plan)
    {
        _id = plan.Id; _version = plan.Version;
        Name = plan.Name;
        Notes = plan.Notes;
        Favorite = plan.Favorite;
        Items.Clear();
        foreach (var item in plan.Exercises) Items.Add(new(item));
    }

    public async Task SaveAsync()
    {
        var name = Name.Trim();
        if (name.Length is < 2 or > 120) throw new ArgumentException("Informe um nome entre 2 e 120 caracteres.");
        if (Items.Count is < 1 or > 50) throw new ArgumentException("Inclua entre 1 e 50 exercícios.");
        var items = Items.Select(i => i.Read()).ToArray();
        dao.Save(new(_id, name, Notes.Trim(), _version, items, Favorite));
        await Task.CompletedTask;
    }

    public Task<PlanItem> ReloadAsync() => Task.FromResult(dao.List().FirstOrDefault(x => x.Id == _id)
        ?? throw new InvalidOperationException("A ficha foi excluída."));

    public Task DeleteAsync()
    {
        dao.Delete(new(_id, Name, Notes, _version, []));
        return Task.CompletedTask;
    }
}
