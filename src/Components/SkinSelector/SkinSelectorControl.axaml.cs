using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Markup.Xaml;
using Mania2mp4.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Mania2mp4.Controls;

public partial class SkinSelectorControl : UserControl {
	public SkinSelectorControl() {
		InitializeComponent();
		var vm = Services.ServiceProvider.GetRequiredService<SkinSelectorViewModel>();
		DataContext = vm;

		comboBox.Bind(ComboBox.ItemsSourceProperty, new Binding("SkinPathList") {
			Source = vm
		});

		// AvaloniaXamlLoader.Load(this);
	}
}
