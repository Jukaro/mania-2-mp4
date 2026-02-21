using Avalonia.Controls;
using Mania2mp4.ViewModels;

namespace Mania2mp4.Views;

public partial class FilesStatsView : UserControl {
	public FilesStatsView() {
		InitializeComponent();
		DataContext = new FilesStatsViewModel();
	}
}
