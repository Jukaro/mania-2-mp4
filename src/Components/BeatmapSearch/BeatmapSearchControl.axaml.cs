using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Markup.Xaml;
using Mania2mp4.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Mania2mp4.Controls;

public partial class BeatmapSearchControl : UserControl {
	private BeatmapSearchViewModel _vm;

	public static readonly DirectProperty<BeatmapSearchControl, object?> SelectedItemProperty;

	private object? _selectedItem;

	public object? SelectedItem {
		get { return _selectedItem; }
		set { SetAndRaise(SelectedItemProperty, ref _selectedItem, value); }
	}

	static BeatmapSearchControl() {
		SelectedItemProperty = AvaloniaProperty.RegisterDirect<BeatmapSearchControl, object?>(nameof(SelectedItem), o => o.SelectedItem, (o, v) => o.SelectedItem = v);
	}

	public BeatmapSearchControl() {
		InitializeComponent();
		_vm = Services.ServiceProvider.GetRequiredService<BeatmapSearchViewModel>();

		searchBarTextBox.Bind(TextBox.TextProperty, new Binding("Query") {
			Source = _vm
		});

		listBox.Bind(ListBox.ItemsSourceProperty, new Binding("BeatmapDisplayManager.Displays") {
			Source = _vm
		});

		listBox.Bind(ListBox.SelectedItemProperty, new Binding("SelectedItem") {
			Source = this,
			Mode = BindingMode.OneWayToSource
		});

		pageInfo.Bind(TextBlock.TextProperty, new MultiBinding {
			Bindings = {
				new Binding("CurrentPage") { Source = _vm },
				new Binding("PageCount") { Source = _vm }
			},
			StringFormat = "Page {0} / {1}"
		});

		searchInfo.Bind(TextBlock.TextProperty, new Binding("SearchResultsCount") {
			Source = _vm,
			StringFormat = "{0} results"
		});
	}

	public void FirstPage() { _vm.FirstPage(); }
	public void PreviousPage() { _vm.PreviousPage(); }
	public void NextPage() { _vm.NextPage(); }
	public void LastPage() { _vm.LastPage(); }
}
