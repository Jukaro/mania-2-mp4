using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Mania2mp4.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Mania2mp4.Views;

public partial class BeatmapLocalScoresView : UserControl {
	public BeatmapLocalScoresView() {
		InitializeComponent();
		DataContext = Services.ServiceProvider.GetRequiredService<BeatmapLocalScoresViewModel>();
	}
}
