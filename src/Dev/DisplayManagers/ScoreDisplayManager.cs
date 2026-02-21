using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Mania2mp4.Displays;
using Mania2mp4.Models;
using Rythmify.Core.Replay;

public partial class ScoreDisplayManager : ObservableObject {
	private DatabasesService _databases;

	[ObservableProperty]
	private List<ReplayData> _scores;

	[ObservableProperty]
	private List<ScoreDisplay> _displays = new();

	public ScoreDisplayManager(DatabasesService databases) {
		_databases = databases;
		PropertyChanged += OnPropertyChanged;
	}

	private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) {
		if (e.PropertyName == nameof(Scores)) {
			Task.Run(UpdateDisplays);
		}
	}

	private async Task UpdateDisplays() {
		ReplayData lastScore = null;

		foreach (ReplayData score in Scores.AsEnumerable().Reverse()) {
			score.Accuracy = ScoreMetrics.ComputeV1Accuracy(score);
			if (lastScore != null)
				score.ScoreDifference = score.Score - lastScore.Score;
			lastScore = score;
		}

		List<ScoreDisplay> displays = new();

		foreach (var score in Scores) {
			var beatmap = _databases.ScoreDB.Beatmaps[score.BeatmapMD5];
			var display = new ScoreDisplay(score);
			display.BeatmapDisplay = new(beatmap);
			displays.Add(display);
		}

		Displays = displays;
	}
}
