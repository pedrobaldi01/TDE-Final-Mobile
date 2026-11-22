using System.Collections.ObjectModel;
using System.Globalization;
using Workoutly.Mobile.Data;
using Workoutly.Mobile.Services;

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
            throw new AccountException($"Revise {Exercise.Name}: use 1 a 20 séries e 1 a 200 repetições.");
        decimal? kg = null;
        if (!string.IsNullOrWhiteSpace(Weight))
        {
            if (!decimal.TryParse(Weight.Trim().Replace(',', '.'), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var weight)
                || weight is < 0 or > 2000 || decimal.Round(weight, 2) != weight)
                throw new AccountException($"Revise a carga de {Exercise.Name}: 0 a 2000 kg com até duas casas decimais.");
            kg = weight;
        }
        return Exercise with { Sets = sets, Repetitions = reps, SuggestedWeightKg = kg };
    }
}

public sealed class PlanEditorViewModel(AccountClient accounts, string? studentId, bool local)
{
    private string _id = Guid.NewGuid().ToString();
    private int _version;
    private string[] _baseline = [];
    public bool Local => local;
    public string Name { get; set; } = "";
    public string Notes { get; set; } = "";
    public bool Favorite { get; set; }
    public ObservableCollection<ExerciseFields> Items { get; } = [];
    public bool SavedBefore => _version > 0;
    public bool Changed => !_baseline.SequenceEqual(Values());
    public void Remember() => _baseline = Values();
    private string[] Values() => new[] { Name, Notes, Favorite.ToString() }.Concat(Items.SelectMany(i =>
        new[] { i.Exercise.ExerciseId, i.Sets, i.Repetitions, i.Weight })).ToArray();

    public void Fill(PlanItem plan, bool keepIdentity)
    {
        if (!keepIdentity) { _id = plan.Id; _version = plan.Version; }
        Name = plan.Name;
        Notes = plan.Notes;
        Favorite = local && plan.Favorite;
        Items.Clear();
        foreach (var item in plan.Exercises) Items.Add(new(item));
    }

    public async Task SaveAsync()
    {
        var name = Name.Trim();
        if (name.Length is < 2 or > 120) throw new AccountException("Informe um nome entre 2 e 120 caracteres.");
        if (Items.Count is < 1 or > 50) throw new AccountException("Inclua entre 1 e 50 exercícios.");
        var items = Items.Select(i => i.Read()).ToArray();
        if (local)
            accounts.Templates.Save(new(_id, "", name, Notes.Trim(), _version, items, Favorite));
        else
            await accounts.SavePlanAsync(studentId, _id, _version, name, Notes.Trim(),
                items.Select(i => new PlanExercisePayload(i.ExerciseId, i.Sets, i.Repetitions, i.SuggestedWeightKg)).ToArray());
    }

    public Task<PlanItem> ReloadAsync() => local
        ? Task.FromResult(accounts.Templates.List().FirstOrDefault(x => x.Id == _id) ?? throw new AccountException("O modelo foi excluído."))
        : accounts.GetPlanAsync(studentId, _id);

    public async Task DeleteAsync()
    {
        if (local) accounts.Templates.Delete(new(_id, "", Name, Notes, _version, []));
        else await accounts.DeletePlanAsync(studentId, _id, _version);
    }
}
