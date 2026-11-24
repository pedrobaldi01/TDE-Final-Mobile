namespace Workoutly.Mobile.Models;

internal static class ExerciseImages
{
    // O ID do catálogo continua igual mesmo se o nome exibido mudar.
    private static readonly Dictionary<string, string> ById = new()
    {
        ["e1000000-0000-0000-0000-000000000001"] = "barbell_squat.png",
        ["e1000000-0000-0000-0000-000000000002"] = "leg_press.png",
        ["e1000000-0000-0000-0000-000000000003"] = "leg_extension.png",
        ["e1000000-0000-0000-0000-000000000004"] = "bench_press.png",
        ["e1000000-0000-0000-0000-000000000005"] = "bent_over_row.png",
        ["e1000000-0000-0000-0000-000000000006"] = "lat_pulldown.png",
        ["e1000000-0000-0000-0000-000000000007"] = "dumbbell_shoulder_press.png",
        ["e1000000-0000-0000-0000-000000000008"] = "barbell_curl.png",
        ["e1000000-0000-0000-0000-000000000009"] = "triceps_pushdown.png",
        ["e1000000-0000-0000-0000-000000000010"] = "calf_raise.png",
        ["e1000000-0000-0000-0000-000000000011"] = "dumbbell_fly.png",
        ["e1000000-0000-0000-0000-000000000012"] = "lateral_raise.png",
        ["e1000000-0000-0000-0000-000000000013"] = "incline_bench_press.png",
        ["e1000000-0000-0000-0000-000000000014"] = "dumbbell_bench_press.png",
        ["e1000000-0000-0000-0000-000000000015"] = "push_up.png",
        ["e1000000-0000-0000-0000-000000000016"] = "seated_row.png",
        ["e1000000-0000-0000-0000-000000000017"] = "one_arm_dumbbell_row.png",
        ["e1000000-0000-0000-0000-000000000018"] = "pull_up.png",
        ["e1000000-0000-0000-0000-000000000019"] = "deadlift.png",
        ["e1000000-0000-0000-0000-000000000020"] = "romanian_deadlift.png",
        ["e1000000-0000-0000-0000-000000000021"] = "lying_leg_curl.png",
        ["e1000000-0000-0000-0000-000000000022"] = "seated_leg_curl.png",
        ["e1000000-0000-0000-0000-000000000023"] = "walking_lunge.png",
        ["e1000000-0000-0000-0000-000000000024"] = "bulgarian_split_squat.png",
        ["e1000000-0000-0000-0000-000000000025"] = "hip_thrust.png",
        ["e1000000-0000-0000-0000-000000000026"] = "seated_calf_raise.png",
        ["e1000000-0000-0000-0000-000000000027"] = "hammer_curl.png",
        ["e1000000-0000-0000-0000-000000000028"] = "preacher_curl.png",
        ["e1000000-0000-0000-0000-000000000029"] = "skull_crusher.png",
        ["e1000000-0000-0000-0000-000000000030"] = "front_raise.png",
    };

    public static string Find(string id) => ById.GetValueOrDefault(id, "exercise_generic.png");
}
