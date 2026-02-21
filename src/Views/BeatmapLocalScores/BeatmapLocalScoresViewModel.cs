using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Mania2mp4.Displays;
using Mania2mp4.Models;

namespace Mania2mp4.ViewModels;

public partial class BeatmapLocalScoresViewModel : ViewModelBase {
	private DatabasesService _databases;
	private OsuReplayModel _osuReplayModel;

	[ObservableProperty]
	private BeatmapDisplay? _selectedBeatmapDisplay;

	[ObservableProperty]
	private ScoreDisplay? _selectedScoreDisplay;

	[ObservableProperty]
	private int _localScoresCount;

	public ScoreDisplayManager ScoreDisplayManager { get; }

	public BeatmapLocalScoresViewModel(DatabasesService databases, OsuReplayModel osuReplayModel) {
		_databases = databases;
		_osuReplayModel = osuReplayModel;
		ScoreDisplayManager = new(_databases);

		PropertyChanged += OnPropertyChanged;
	}

	private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) {
		if (e.PropertyName == nameof(SelectedBeatmapDisplay)) {
			if (SelectedBeatmapDisplay == null
				|| !_databases.ScoreDB.Beatmaps.ContainsKey(SelectedBeatmapDisplay.BeatmapMD5))
				return;

			var beatmap = _databases.BeatmapDB.Beatmaps[SelectedBeatmapDisplay.BeatmapMD5];
			_osuReplayModel.Beatmap = _databases.ScoreDB.Beatmaps[beatmap.BeatmapMD5];
			var scores = _databases.ScoreDB.Beatmaps[SelectedBeatmapDisplay.BeatmapMD5].Replays;
			ScoreDisplayManager.Scores = scores;
			LocalScoresCount = scores.Count;
		} else if (e.PropertyName == nameof(SelectedScoreDisplay)) {
			if (SelectedScoreDisplay == null) return;
			_osuReplayModel.Score = SelectedScoreDisplay.Replay;
		}
	}
}
