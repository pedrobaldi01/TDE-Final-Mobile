using System.Security.Claims;
using Workoutly.Api.Data;

namespace Workoutly.Api.Auth;

internal static class StudentProfileEndpoints
{
    internal sealed record Input(string? FirstName, string? LastName, decimal? BodyWeightKg, int? HeightCm);

    public static void Map(RouteGroupBuilder auth)
    {
        var profile = auth.MapGroup("/me/student-profile").RequireAuthorization("student");
        profile.MapGet("", async (ClaimsPrincipal user, MySqlDatabase database, CancellationToken ct) =>
        {
            await using var db = await database.OpenConnectionAsync(ct);
            await using var cmd = db.CreateCommand();
            cmd.CommandText = """
                SELECT u.name, m.first_name, m.last_name, m.body_weight_kg, m.height_cm
                FROM users u LEFT JOIN student_measurements m ON m.student_id=u.id WHERE u.id=@id
                """;
            cmd.Parameters.AddWithValue("@id", user.FindFirstValue(ClaimTypes.NameIdentifier));
            await using var row = await cmd.ExecuteReaderAsync(ct);
            if (!await row.ReadAsync(ct)) return Results.NotFound();
            var name = row.GetString(0);
            var first = row.IsDBNull(1) ? name : row.GetString(1);
            var last = row.IsDBNull(2) ? null : row.GetString(2);
            if (string.Join(" ", new[] { first, last }.Where(s => s is not null)) != name) (first, last) = (name, null);
            return Results.Ok(new { name, firstName = first, lastName = last,
                bodyWeightKg = row.IsDBNull(3) ? (decimal?)null : row.GetDecimal(3),
                heightCm = row.IsDBNull(4) ? (int?)null : row.GetInt32(4) });
        });
        profile.MapPut("", async (Input input, ClaimsPrincipal user, MySqlDatabase database, CancellationToken ct) =>
        {
            var first = input.FirstName?.Trim();
            var last = string.IsNullOrWhiteSpace(input.LastName) ? null : input.LastName.Trim();
            var name = last is null ? first : $"{first} {last}";
            if (string.IsNullOrEmpty(first) || name!.Length is < 2 or > 120 ||
                input.BodyWeightKg is decimal weight && (weight is <= 0 or > 1000 || decimal.Round(weight, 2) != weight) ||
                input.HeightCm is < 1 or > 300)
                return Results.ValidationProblem(new Dictionary<string, string[]> { ["perfil"] = ["Confira o nome, o peso e a altura."] });
            await using var db = await database.OpenConnectionAsync(ct);
            await using var tx = await db.BeginTransactionAsync(ct);
            await using var cmd = db.CreateCommand();
            cmd.Transaction = tx;
            cmd.CommandText = """
                UPDATE users SET name=@name WHERE id=@id;
                INSERT INTO student_measurements(student_id,first_name,last_name,body_weight_kg,height_cm)
                VALUES(@id,@first,@last,@weight,@height) ON DUPLICATE KEY UPDATE first_name=@first,last_name=@last,
                body_weight_kg=@weight,height_cm=@height;
                """;
            cmd.Parameters.AddWithValue("@id", user.FindFirstValue(ClaimTypes.NameIdentifier));
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@first", first);
            cmd.Parameters.AddWithValue("@last", (object?)last ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@weight", (object?)input.BodyWeightKg ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@height", (object?)input.HeightCm ?? DBNull.Value);
            await cmd.ExecuteNonQueryAsync(ct);
            await tx.CommitAsync(ct);
            return Results.Ok(new { name, firstName = first, lastName = last, input.BodyWeightKg, input.HeightCm });
        });
    }
}
