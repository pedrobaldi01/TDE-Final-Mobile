using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Workoutly.Mobile.Data;
using Workoutly.Mobile.Services;

internal static class OnlineScenario
{
    // Porta exclusiva da cópia. Execute apenas contra o helper de demonstração.
    private static readonly Uri Demo = new("http://localhost:5186/");
    public static async Task RunAsync(string directory)
    {
        using var teacher = Client();
        using var student = Client();
        using var stranger = Client();
        var teacherId = await SignInAsync(teacher, "teacher");
        var studentId = await SignInAsync(student, "student");
        await SignInAsync(stranger, "teacher");
        var invitation = await Json(await teacher.PostAsync("teaching/invitation", null));
        await Json(await student.PostAsJsonAsync("enrollment", new { code = invitation.GetProperty("code").GetString() }));
        var path = $"teaching/students/{studentId}/plans";
        var dao = new TemplateDao(directory, Demo.ToString(), teacherId);
        var exercise = dao.Catalog().First() with { SuggestedWeightKg = 20m };
        dao.Save(new(Guid.NewGuid().ToString(), "", "Modelo de aula", "", 0, [exercise], true));
        var model = dao.List().Single();
        var id = Guid.NewGuid();
        var plan = await Json(await teacher.PostAsJsonAsync(path, new { id, model.Name, model.Notes,
            exercises = model.Exercises.Select(x => new { x.ExerciseId, x.Sets, x.Repetitions, x.SuggestedWeightKg }) }));
        var received = await student.GetFromJsonAsync<JsonElement>($"plans/{id}");
        if (received.GetProperty("name").GetString() != model.Name ||
            received.GetProperty("exercises")[0].GetProperty("suggestedWeightKg").GetDecimal() != 20m)
            throw new Exception("Ficha recebida difere do modelo.");
        await Json(await student.PostAsJsonAsync("sessions", new { id = Guid.NewGuid(), planId = id,
            planVersion = plan.GetProperty("version").GetInt32(), startedAt = DateTimeOffset.UtcNow.AddMinutes(-5),
            durationSeconds = 60, notes = "", exercises = new[] { new { exercise.ExerciseId, sets = new[] { new { repetitions = 10, weightKg = 20m } } } } }));
        var date = DateTime.UtcNow;
        var query = $"history?from={date.AddDays(-1):yyyy-MM-dd}&to={date:yyyy-MM-dd}&offsetMinutes=0";
        var prefix = $"teaching/students/{studentId}/";
        foreach (var result in new[] { await student.GetFromJsonAsync<JsonElement>(query), await teacher.GetFromJsonAsync<JsonElement>(prefix + query) })
            if (result.GetProperty("volumeKg").GetDecimal() != 200m) throw new Exception("Volume do histórico incorreto.");
        foreach (var route in new[] { path, prefix + query })
        {
            using var forbidden = await stranger.GetAsync(route);
            if (forbidden.StatusCode != HttpStatusCode.NotFound) throw new Exception("Falha no isolamento do professor.");
        }
        Console.WriteLine("OK • modelo SQLite → ficha compartilhada → série → histórico dos dois perfis e proteção do vínculo");
    }

    private static HttpClient Client() => new(new HttpClientHandler { AllowAutoRedirect = false }) { BaseAddress = Demo, Timeout = TimeSpan.FromSeconds(12) };
    private static async Task<string> SignInAsync(HttpClient client, string role)
    {
        var email = $"{Guid.NewGuid():N}@example.test";
        var password = Guid.NewGuid().ToString("N");
        await Json(await client.PostAsJsonAsync("auth/register", new { name = "Teste Mobile", email, password, role }));
        var session = await Json(await client.PostAsJsonAsync("auth/login", new { email, password }));
        client.DefaultRequestHeaders.Authorization = new("Bearer", session.GetProperty("accessToken").GetString());
        return session.GetProperty("user").GetProperty("id").GetString()!;
    }
    private static async Task<JsonElement> Json(HttpResponseMessage response)
    {
        using (response) { response.EnsureSuccessStatusCode(); return await response.Content.ReadFromJsonAsync<JsonElement>(); }
    }
}
