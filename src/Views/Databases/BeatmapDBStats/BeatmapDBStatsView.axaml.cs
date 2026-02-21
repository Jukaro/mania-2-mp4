using Avalonia.Controls;
using Mania2mp4.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Mania2mp4.Views;

public partial class BeatmapDBStatsView : UserControl {
	public BeatmapDBStatsView() {
		InitializeComponent();
		DataContext = Services.ServiceProvider.GetRequiredService<BeatmapDBStatsViewModel>();
	}
}
