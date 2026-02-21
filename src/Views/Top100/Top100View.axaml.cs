using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Mania2mp4.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Mania2mp4.Views;

public partial class Top100View : UserControl {
	public Top100View() {
		InitializeComponent();
		DataContext = Services.ServiceProvider.GetRequiredService<Top100ViewModel>();
	}
}
