using Microsoft.Extensions.DependencyInjection;

namespace MauiApp1;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
		
		// Only clear preferences ONCE to fix type conflicts
if (!Preferences.ContainsKey("PreferencesCleared_v2"))
{
    Preferences.Clear();
    Preferences.Set("PreferencesCleared_v2", "true");
}
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new AppShell());
	}
}