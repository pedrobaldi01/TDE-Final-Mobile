using Workoutly.Mobile.Data;

namespace Workoutly.Mobile;

public partial class AppShell : Shell
{
	private bool _confirmingLeave;
	public AppShell()
	{
        InitializeComponent();
        var dao = new TemplateDao(FileSystem.AppDataDirectory);
        HomeContent.ContentTemplate = new DataTemplate(() => new MainPage());
        PlansContent.ContentTemplate = new DataTemplate(() => new PlansPage(dao));
    }
    private async void OnHome(object? sender, EventArgs args) { FlyoutIsPresented = false; await GoToAsync("//area/inicio/home"); }
    private async void OnPlans(object? sender, EventArgs args) { FlyoutIsPresented = false; await GoToAsync("//area/fichas/lista"); }

	protected override async void OnNavigating(ShellNavigatingEventArgs args)
	{
		base.OnNavigating(args);
		if (args.Cancelled || !args.CanCancel || args.Source is not (ShellNavigationSource.Pop or ShellNavigationSource.PopToRoot or ShellNavigationSource.ShellSectionChanged or ShellNavigationSource.ShellItemChanged or ShellNavigationSource.ShellContentChanged) || CurrentPage is not IConfirmLeave form) return;
		if (_confirmingLeave) { args.Cancel(); return; }
		var deferral = args.GetDeferral();
		if (deferral is null) return;
		_confirmingLeave = true;
		try { if (!await form.CanLeaveAsync()) args.Cancel(); }
		finally { _confirmingLeave = false; deferral.Complete(); }
	}
}

// Confirmação única para os retornos canceláveis do Shell.
internal interface IConfirmLeave
{
	Task<bool> CanLeaveAsync();
}
