using System;
using System.Threading;
using Avalonia.Controls;
using Avalonia.Input;
using Mania2mp4.Models;
using Mania2mp4.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Mania2mp4.Views;

public partial class MainWindow : Window
{
	public MainWindow()
	{
		if (Design.IsDesignMode){
			Services.Init();
			Design.SetDataContext(this, Services.ServiceProvider.GetRequiredService<MainWindowViewModel>());
		}
		InitializeComponent();

		Focusable = true;
		PointerPressed += MainWindow_PointerPressed;
	}

	private void MainWindow_PointerPressed(object? sender, PointerPressedEventArgs e) {
		Focus();
	}
}
