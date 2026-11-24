namespace Workoutly.Mobile.Models;

public sealed record PlanExerciseItem(string ExerciseId, string Name, string MuscleGroup, string Equipment, string Description, int Sets, int Repetitions,
    decimal? SuggestedWeightKg = null);
public sealed record PlanItem(string Id, string Name, string Notes, int Version, PlanExerciseItem[] Exercises, bool Favorite = false)
{
    public string Summary => $"{Exercises.Length} exercício(s) • {Exercises.Sum(item => item.Sets)} séries previstas";
}

public sealed record ExerciseItem(string Id, string Name, string MuscleGroup, string Equipment, string Description)
{
    public string Summary => $"{MuscleGroup} • {Equipment}";
    public string ImageSource => Services.ExerciseVisuals.Find(Id, Name).ImageSource;
}
