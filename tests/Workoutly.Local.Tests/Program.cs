using Workoutly.Mobile.Data;
using Workoutly.Mobile.Services;

var temp = Directory.CreateTempSubdirectory("workoutly-modelos-");
try
{
    var dao = new TemplateDao(temp.FullName, "demo", "professor-a");
    var exercise = dao.Catalog().First();
    var template = new PlanItem("modelo-a", "", "Ficha A", "Revisar", 0, [exercise with { Sets = 3, Repetitions = 8 }], true);
    dao.Save(template);
    var reopened = new TemplateDao(temp.FullName, "demo", "professor-a");
    var saved = reopened.List().Single();
    Check(saved.Name == "Ficha A" && saved.Favorite && saved.Exercises.Single().Repetitions == 8, "criar e reabrir");
    var other = new TemplateDao(temp.FullName, "demo", "professor-b");
    Check(other.List().Length == 0, "isolamento");
    reopened.Save(saved with { Exercises = [exercise with { Sets = 4, Repetitions = 12 }] });
    var edited = dao.List().Single();
    Check(edited.Version == 2 && edited.Exercises.Single().Sets == 4, "alterar");
    try { dao.Save(edited with { Name = "Incompleta", Exercises = [exercise with { ExerciseId = "desconhecido" }] }); }
    catch (SQLite.SQLiteException) { }
    Check(dao.List().Single().Name == "Ficha A" && dao.List().Single().Exercises.Length == 1, "rollback");
    try { dao.Delete(saved); } catch (InvalidOperationException) { }
    Check(dao.List().Single().Version == 2, "conflito");
    dao.Delete(edited);
    Check(dao.List().Length == 0, "excluir");
    if (args.Contains("--online")) await OnlineScenario.RunAsync(temp.FullName);
}
finally { temp.Delete(true); }

static void Check(bool condition, string step)
{
    if (!condition) throw new Exception(step);
    Console.WriteLine($"OK • {step}");
}
