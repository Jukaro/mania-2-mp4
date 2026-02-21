using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Mania2mp4.Displays;
using Mania2mp4.Models;
using Rythmify.Core.Replay;

public partial class SessionScoreDisplayManager : ObservableObject {
	private DatabasesService _databases;

	[ObservableProperty]
	private List<ReplayData> _scores;

	[ObservableProperty]
	private List<SessionScoreDisplay> _displays = new();

	public SessionScoreDisplayManager(DatabasesService databases) {
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

		List<SessionScoreDisplay> displays = new();
		List<Task> tasks = new();

		foreach (var score in Scores) {
			var beatmap = _databases.ScoreDB.Beatmaps[score.BeatmapMD5];
			var sessionScore = new SessionScore(score, beatmap);
			var display = new SessionScoreDisplay(sessionScore);
			display.BeatmapDisplay = new(beatmap);
			var task = Task.Run(() => display.BeatmapDisplay.Background = _databases.GetThumbnailFromDB(beatmap));
			tasks.Add(task);
			displays.Add(display);
		}

		var oldDisplays = Displays;
		Displays = displays;

		Task.WhenAll(tasks).Wait();
		foreach (var display in oldDisplays)
			_databases.ThumbnailsDB.ReleaseThumbnail(display.BeatmapDisplay.BeatmapMD5);
	}
}
