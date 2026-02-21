using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Mania2mp4.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Rythmify.UI;

namespace Mania2mp4.Views;

public partial class BeatmapMarathonView : UserControl {
	public BeatmapMarathonView() {
		if (Design.IsDesignMode){
			Paths.Init();
			Paths.TryLoad();
			BeatmapDisplayHelper.CreateThumbnailsFolder();
			Services.Init();
			Design.SetDataContext(this, Services.ServiceProvider.GetRequiredService<BeatmapMarathonViewModel>());
		} else {
			DataContext = Services.ServiceProvider.GetRequiredService<BeatmapMarathonViewModel>();
		}
		InitializeComponent();
	}
}
