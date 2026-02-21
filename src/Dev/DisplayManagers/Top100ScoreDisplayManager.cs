using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Mania2mp4.Displays;
using Mania2mp4.Models;
using Rythmify.Core.Replay;
using Rythmify.Dev;

public partial class Top100ScoreDisplayManager : ObservableObject {
	private DatabasesService _databases;

	[ObservableProperty]
	private List<ReplayData> _scores;

	[ObservableProperty]
	private List<Top100ScoreDisplay> _displays = new();

	public Top100ScoreDisplayManager(DatabasesService databases) {
		_databases = databases;
		PropertyChanged += OnPropertyChanged;
	}

	private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) {
		if (e.PropertyName == nameof(Scores)) {
			Task.Run(UpdateDisplays);
		}
	}

	private async Task UpdateDisplays() {
		foreach (ReplayData score in Scores)
			score.Accuracy = ScoreMetrics.ComputeV1Accuracy(score);

		List<Top100ScoreDisplay> displays = new();
		List<Task> tasks = new();

		foreach (var score in Scores) {
			var beatmap = _databases.ScoreDB.Beatmaps[score.BeatmapMD5];
			var display = new Top100ScoreDisplay(score);
			display.BeatmapDisplay = new(beatmap);
			var task = Task.Run(() => display.BeatmapDisplay.Background = _databases.GetThumbnailFromDB(beatmap));
			tasks.Add(task);
			displays.Add(display);
		}

		List<WeightedPP> weightedPPs = ScoreListHelper.GetWeightPercentageAndPP(Scores);

		if (weightedPPs != null) {
			for (int i = 0; i < displays.Count; i++) {
				displays[i].Weight = weightedPPs[i].Weight;
				displays[i].WeightedPP = weightedPPs[i].PerformancePoints;
			}
		}

		var oldDisplays = Displays;
		Displays = displays;

		Task.WhenAll(tasks).Wait();
		foreach (var display in oldDisplays)
			_databases.ThumbnailsDB.ReleaseThumbnail(display.BeatmapDisplay.BeatmapMD5);
	}
}
