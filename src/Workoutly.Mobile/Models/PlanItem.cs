namespace Workoutly.Mobile.Services;

public sealed record PlanExerciseItem(string ExerciseId, string Name, string MuscleGroup, string Equipment, string Description, int Sets, int Repetitions,
    decimal? SuggestedWeightKg = null, decimal? LastUsedWeightKg = null);
public sealed record PlanItem(string Id, string StudentId, string Name, string Notes, int Version, PlanExerciseItem[] Exercises, bool Favorite = false)
{
    public string Summary => $"{Exercises.Length} exercício(s) • {Exercises.Sum(item => item.Sets)} séries previstas";
}
