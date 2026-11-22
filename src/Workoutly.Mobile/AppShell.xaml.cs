using Workoutly.Mobile.Services;

namespace Workoutly.Mobile;

public partial class AppShell : Shell
{
	private bool _confirmingLeave;
	public AppShell()
	{
		InitializeComponent();
	}

    private AccountClient? _session;
    public void OpenWorkspace(AccountClient accounts, AccountProfile profile)
    {
        if (_session == accounts) return;
        _session = accounts;
        var models = new Tab { Title = "Modelos", Route = "modelos" };
        models.Items.Add(new ShellContent { Title = "Modelos", ContentTemplate = new DataTemplate(() => new PlansPage(accounts, local: true)) });
        WorkspaceItem.Items.Add(models);
        var catalog = new FlyoutItem { Title = "Catálogo de exercícios", Route = "catalogo" };
        catalog.Items.Add(new ShellContent { Title = "Exercícios", ContentTemplate = new DataTemplate(() => new ExercisesPage(accounts)) });
        Items.Add(catalog);
        FlyoutBehavior = FlyoutBehavior.Flyout;
        accounts.SessionEnded += () =>
        {
            // Descarta também as pilhas e páginas em cache das outras abas.
            Dispatcher.Dispatch(() => { if (Window is { } window && window.Page == this) window.Page = new AppShell(); });
        };
    }

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
