using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Mania2mp4.Displays;
using Mania2mp4.Models;
using Rythmify.Core;
using Rythmify.Core.Beatmap;

public partial class BeatmapDisplayManager : ObservableObject {
	private DatabasesService _databases;

	[ObservableProperty]
	private List<BeatmapWithScores> _beatmaps;

	[ObservableProperty]
	private List<BeatmapDisplay> _displays = new();

	public BeatmapDisplayManager(DatabasesService databases) {
		_databases = databases;
		PropertyChanged += OnPropertyChanged;
	}

	private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) {
		if (e.PropertyName == nameof(Beatmaps)) {
			Task.Run(UpdateDisplays);
		}
	}

	private async Task UpdateDisplays() {
		List<BeatmapDisplay> displays = new();
		List<Task> tasks = new();

		foreach (var beatmap in Beatmaps) {
			var display = new BeatmapDisplay(beatmap);
			var task = Task.Run(() => display.Background = _databases.GetThumbnailFromDB(beatmap));
			tasks.Add(task);
			displays.Add(display);
		}

		var oldDisplays = Displays;
		Displays = displays;

		Task.WhenAll(tasks).Wait();
		foreach (var display in oldDisplays)
			_databases.ThumbnailsDB.ReleaseThumbnail(display.BeatmapMD5);
	}
}
