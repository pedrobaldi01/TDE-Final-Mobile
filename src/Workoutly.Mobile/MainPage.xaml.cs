namespace Workoutly.Mobile;
public partial class MainPage : ContentPage
{
    public MainPage() => InitializeComponent();
    private async void OnPlans(object? sender, EventArgs args) => await Shell.Current.GoToAsync("//area/fichas/lista");
}
