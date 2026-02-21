using System.Linq;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using Mania2mp4.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Mania2mp4.Views;

public partial class DatabasesView : UserControl {
	public DatabasesView() {
		InitializeComponent();
		DataContext = Services.ServiceProvider.GetRequiredService<DatabasesViewModel>();
	}
}
