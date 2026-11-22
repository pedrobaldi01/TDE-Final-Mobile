namespace Workoutly.Mobile.Services;

public static class ApiSettings
{
    // Em aparelhos físicos, use a mesma URL HTTPS da API de demonstração nos dois.
#if ANDROID
    public const string BaseUrl = "http://10.0.2.2:5186/";
#else
    public const string BaseUrl = "http://localhost:5186/";
#endif
}
