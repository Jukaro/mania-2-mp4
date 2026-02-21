using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using Mania2mp4.ViewModels;
using Mania2mp4.Views;
using System;
using Microsoft.Extensions.DependencyInjection;
using Mania2mp4.Models;
using Rythmify.UI;

namespace Mania2mp4;

public static class Services {
	public static IServiceProvider ServiceProvider { get; private set; }

	public static void Init() {
		var serviceCollection = new ServiceCollection();

		serviceCollection.AddSingleton<OsuReplayModel>();
		serviceCollection.AddSingleton<DatabasesService>();

		serviceCollection.AddTransient<MainWindowViewModel>();
		serviceCollection.AddTransient<OsuReplayViewModel>();
		serviceCollection.AddTransient<DatabasesViewModel>();
		serviceCollection.AddTransient<BeatmapDBStatsViewModel>();
		serviceCollection.AddTransient<BeatmapSearchViewModel>();
		serviceCollection.AddTransient<BeatmapLocalScoresViewModel>();
		serviceCollection.AddTransient<SkinSelectorViewModel>();
		serviceCollection.AddTransient<SessionViewerViewModel>();

		ServiceProvider = serviceCollection.BuildServiceProvider();
	}
}

public partial class App : Application
{
	public override void Initialize()
	{
		AvaloniaXamlLoader.Load(this);

		Paths.Init();
		Paths.TryLoad();
		BeatmapDisplayHelper.CreateThumbnailsFolder();
	}

	// public static IServiceProvider Services { get; private set; }

	public override void OnFrameworkInitializationCompleted()
	{
		if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
		{
			// Avoid duplicate validations from both Avalonia and the CommunityToolkit.
			// More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
			DisableAvaloniaDataAnnotationValidation();
			desktop.Exit += OnExit;

			Services.Init();

			desktop.MainWindow = new MainWindow
			{
				DataContext = Services.ServiceProvider.GetRequiredService<MainWindowViewModel>()
			};
		}

		base.OnFrameworkInitializationCompleted();
	}

	private void DisableAvaloniaDataAnnotationValidation()
	{
		// Get an array of plugins to remove
		var dataValidationPluginsToRemove =
			BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

		// remove each entry found
		foreach (var plugin in dataValidationPluginsToRemove)
		{
			BindingPlugins.DataValidators.Remove(plugin);
		}
	}

	private void OnExit(object? sender, ControlledApplicationLifetimeExitEventArgs e) {
		var databases = Services.ServiceProvider.GetRequiredService<DatabasesService>();
		databases.SaveThumbnailDatabase();
	}
}
