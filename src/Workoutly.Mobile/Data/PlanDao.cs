using SQLite;
using Workoutly.Mobile.Models;

namespace Workoutly.Mobile.Data;

// Um arquivo local por instalação; toda conexão habilita as chaves estrangeiras.
public sealed class PlanDao
{
    private readonly string _path;
    public PlanDao(string directory)
    {
        _path = Path.Combine(directory, "workoutly.db3");
        using var db = Open();
        db.RunInTransaction(() =>
        {
            db.Execute("CREATE TABLE IF NOT EXISTS ModeloFicha (Id TEXT PRIMARY KEY, Name TEXT NOT NULL, Notes TEXT NOT NULL, Favorite INTEGER NOT NULL CHECK(Favorite IN (0,1)), Version INTEGER NOT NULL)");
            db.Execute("CREATE TABLE IF NOT EXISTS ExercicioLocal (Id TEXT PRIMARY KEY, Name TEXT NOT NULL, MuscleGroup TEXT NOT NULL, Equipment TEXT NOT NULL)");
            db.Execute("""
                CREATE TABLE IF NOT EXISTS ItemModelo (
                  ModelId TEXT NOT NULL REFERENCES ModeloFicha(Id) ON DELETE CASCADE,
                  ExerciseId TEXT NOT NULL REFERENCES ExercicioLocal(Id), Position INTEGER NOT NULL,
                  Sets INTEGER NOT NULL CHECK(Sets BETWEEN 1 AND 20), Repetitions INTEGER NOT NULL CHECK(Repetitions BETWEEN 1 AND 200),
                  WeightCents INTEGER CHECK(WeightCents BETWEEN 0 AND 200000),
                  PRIMARY KEY(ModelId, Position), UNIQUE(ModelId, ExerciseId))
                """);
            if (db.ExecuteScalar<int>("SELECT count(*) FROM ExercicioLocal") > 0) return;
            using var source = typeof(PlanDao).Assembly.GetManifestResourceStream("Workoutly.SharedExercises")!;
            using var reader = new StreamReader(source);
            while (reader.ReadLine() is { } line)
                db.Execute("INSERT INTO ExercicioLocal VALUES (?,?,?,?)", line.Split('\t').Cast<object>().ToArray());
        });
    }

    private SQLiteConnection Open()
    {
        var db = new SQLiteConnection(_path);
        db.Execute("PRAGMA foreign_keys=ON");
        return db;
    }

    public ExerciseItem[] Catalog()
    {
        using var db = Open();
        return db.Query<ItemRow>("SELECT Id AS ExerciseId, Name, MuscleGroup, Equipment FROM ExercicioLocal ORDER BY Name")
            .Select(row => new ExerciseItem(row.ExerciseId, row.Name, row.MuscleGroup, row.Equipment)).ToArray();
    }

    public PlanItem[] List()
    {
        using var db = Open();
        PlanItem[] result = [];
        db.RunInTransaction(() =>
        {
            var rows = db.Query<ItemRow>("""
                SELECT i.ModelId, i.ExerciseId, e.Name, e.MuscleGroup, e.Equipment, i.Sets, i.Repetitions, i.WeightCents
                FROM ItemModelo i JOIN ExercicioLocal e ON e.Id=i.ExerciseId ORDER BY i.Position
                """).ToLookup(row => row.ModelId, row => row.ToItem());
            result = db.Query<Header>("SELECT * FROM ModeloFicha ORDER BY Favorite DESC, Name COLLATE NOCASE")
                .Select(h => new PlanItem(h.Id, h.Name, h.Notes, h.Version, rows[h.Id].ToArray(), h.Favorite)).ToArray();
        });
        return result;
    }

    public void Save(PlanItem plan)
    {
        if (plan.Exercises.Length is < 1 or > 50) throw new ArgumentException("Inclua de 1 a 50 exercícios.");
        using var db = Open();
        db.RunInTransaction(() =>
        {
            if (plan.Version == 0)
                db.Execute("INSERT INTO ModeloFicha VALUES (?,?,?,?,1)", plan.Id, plan.Name, plan.Notes, plan.Favorite);
            else
                CheckVersion(db.Execute("UPDATE ModeloFicha SET Name=?, Notes=?, Favorite=?, Version=Version+1 WHERE Id=? AND Version=?",
                    plan.Name, plan.Notes, plan.Favorite, plan.Id, plan.Version));
            db.Execute("DELETE FROM ItemModelo WHERE ModelId=?", plan.Id);
            for (var i = 0; i < plan.Exercises.Length; i++)
            {
                var item = plan.Exercises[i];
                int? cents = item.SuggestedWeightKg is decimal kg ? decimal.ToInt32(kg * 100) : null;
                db.Execute("INSERT INTO ItemModelo VALUES (?,?,?,?,?,?)", plan.Id, item.ExerciseId, i, item.Sets, item.Repetitions, cents);
            }
        });
    }

    public void Delete(PlanItem plan)
    {
        using var db = Open();
        CheckVersion(db.Execute("DELETE FROM ModeloFicha WHERE Id=? AND Version=?", plan.Id, plan.Version));
    }

    private static void CheckVersion(int changed)
    {
        if (changed != 1) throw new InvalidOperationException("A ficha foi alterada ou excluída. Reabra a lista.");
    }

    private sealed class Header
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string Notes { get; set; } = "";
        public bool Favorite { get; set; }
        public int Version { get; set; }
    }

    private sealed class ItemRow
    {
        public string ModelId { get; set; } = "";
        public string ExerciseId { get; set; } = "";
        public string Name { get; set; } = "";
        public string MuscleGroup { get; set; } = "";
        public string Equipment { get; set; } = "";
        public int Sets { get; set; }
        public int Repetitions { get; set; }
        public int? WeightCents { get; set; }
        public PlanExerciseItem ToItem() => new(ExerciseId, Name, MuscleGroup, Equipment, Sets, Repetitions, WeightCents / 100m);
    }
}
