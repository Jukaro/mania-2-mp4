using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Markup.Xaml;
using DynamicData;
using Mania2mp4.Controls;
using Mania2mp4.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Mania2mp4.Views;

public partial class SessionViewerView : UserControl {
	public SessionViewerView() {
		InitializeComponent();
		var vm = Services.ServiceProvider.GetRequiredService<SessionViewerViewModel>();
		sessionCalendar.SelectedDatesChanged += vm.OnDateChanged;
		DataContext = vm;
	}
}
