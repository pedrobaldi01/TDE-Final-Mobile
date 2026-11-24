using Workoutly.Mobile.Data;
using Workoutly.Mobile.ViewModels;
using Workoutly.Mobile.Models;

var temp = Directory.CreateTempSubdirectory("workoutly-modelos-");
try
{
    var dao = new TemplateDao(temp.FullName);
    var exercise = dao.Catalog().First();
    var template = new PlanItem("ficha-a", "Ficha A", "Revisar", 0, [exercise with { Sets = 3, Repetitions = 8 }], true);
    dao.Save(template);
    var reopened = new TemplateDao(temp.FullName);
    var saved = reopened.List().Single();
    Check(saved.Name == "Ficha A" && saved.Favorite && saved.Exercises.Single().Repetitions == 8, "criar e reabrir");
    reopened.Save(saved with { Exercises = [exercise with { Sets = 4, Repetitions = 12 }] });
    var edited = dao.List().Single();
    Check(edited.Version == 2 && edited.Exercises.Single().Sets == 4, "alterar");
    try { dao.Save(edited with { Name = "Incompleta", Exercises = [exercise with { ExerciseId = "desconhecido" }] }); }
    catch (SQLite.SQLiteException) { }
    Check(dao.List().Single().Name == "Ficha A" && dao.List().Single().Exercises.Length == 1, "rollback");
    try { dao.Delete(saved); } catch (InvalidOperationException) { }
    Check(dao.List().Single().Version == 2, "conflito");
    dao.Save(new("ficha-b", "Ficha B", "", 0, [exercise], false));
    var list = new PlansViewModel(dao);
    await list.LoadAsync();
    Check(list.Visible.Length == 2, "Todas mostra as duas fichas");
    list.OnlyFavorites = true;
    Check(list.Visible.Length == 1 && list.Visible[0].Name == "Ficha A", "Só favoritas");
    list.Query = "Ficha B";
    Check(list.Visible.Length == 0, "pesquisa respeita favoritas");
    list.OnlyFavorites = false;
    Check(list.Visible.Single().Name == "Ficha B", "Todas preserva pesquisa");
    list.Query = "";
    dao.Delete(dao.List().Single(x => x.Id == "ficha-b"));
    dao.Delete(edited);
    Check(dao.List().Length == 0, "excluir");

}
finally { temp.Delete(true); }

static void Check(bool condition, string step)
{
    if (!condition) throw new Exception(step);
    Console.WriteLine($"OK • {step}");
}
