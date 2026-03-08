using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Avalonia.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mania2mp4.DataValidation;
using Mania2mp4.Displays;
using Mania2mp4.Models;
using Rythmify.Core;
using Rythmify.Core.Beatmap;
using Rythmify.Core.Beatmap.Concatenation;
using Rythmify.UI;

namespace Mania2mp4.ViewModels;

public partial class BeatmapMarathonViewModel : ObservableValidator {
	private DatabasesService _databasesModel;

	[ObservableProperty]
	List<BeatmapDisplay> _beatmapDisplays;

	[ObservableProperty]
	ObservableCollection<BeatmapDisplay> _selectedBeatmapsDisplays = new();

	List<BeatmapWithScores> _selectedBeatmaps = new();

	public BeatmapConcatenationParameters Parameters { get; } = new();

	public int SelectedMeasureDivision { get; set; } = 1;
	public int? TimeSpacing { get; set; }

	public bool IsTimeSpacingChecked { get; set; }
	public bool IsMeasureDivisionSpacingChecked { get; set; }

	[ObservableProperty]
	private BeatmapDisplay? _selectedBeatmapDisplay;

	public BeatmapMarathonViewModel(DatabasesService databasesModel) {
		_databasesModel = databasesModel;
		_databasesModel.DatabasesInitialized += OnDatabasesInitialized;
		PropertyChanged += OnPropertyChanged;
	}

	private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) {
		if (e.PropertyName == nameof(SelectedBeatmapDisplay)) {
			if (SelectedBeatmapDisplay == null
				|| !_databasesModel.ScoreDB.Beatmaps.ContainsKey(SelectedBeatmapDisplay.BeatmapMD5))
				return;

			SelectBeatmap(SelectedBeatmapDisplay);
		}
	}

	private void OnDatabasesInitialized(object? sender, EventArgs e) {
		BeatmapDisplays = _databasesModel.ScoreDB.Beatmaps.Values.Select(b => new BeatmapDisplay(b)).ToList();
	}

	public async Task RefreshFolder() {
		if (string.IsNullOrEmpty(Parameters.FolderName)) return;

		string dirPath = Path.Combine(Paths.OsuSongsDirectoryPath, Parameters.FolderName);
		string refreshPath = dirPath + "a";

		try {
			Directory.Move(dirPath, refreshPath);
			Directory.Move(refreshPath, dirPath);
		} catch (Exception e) {
			Logger.LogError($"[BeatmapMarathon] Couldn't refresh folder: {e.Message}");
		}
	}

	public void ConcatenateBeatmaps() {
		if (_selectedBeatmaps.Count == 0) return;

		if (string.IsNullOrEmpty(Parameters.FolderName) || string.IsNullOrEmpty(Parameters.BeatmapFilename))
			throw new DataValidationException("Name cant be unset");

		Parameters.Delay = IsTimeSpacingChecked
			? new MillisecondsDelay(TimeSpacing ?? 1000)
			: new MeasureDivisionDelay(SelectedMeasureDivision);

		string marathonStr = string.Join(", ", _selectedBeatmaps.Select(b => $"{b.BeatmapDBInfo.SongTitle} [{b.BeatmapDBInfo.Difficulty}]"));
		Logger.LogInfo($"\n[BeatmapMarathon] Creating marathon from beatmaps [{marathonStr}]");

		Task.Run(() => {
			try {
				_selectedBeatmaps.ForEach(b => b.LoadBeatmap());
				BeatmapConcatenation.Concatenate(_selectedBeatmaps, Parameters);
				Logger.LogInfo("[BeatmapMarathon] Done !");
			} catch (Exception e) {
				Logger.LogError($"[BeatmapMarathon] Couldn't create the beatmap marathon: {e.Message}", e.StackTrace);
				return;
			}
		});
	}

	public void SelectBeatmap(BeatmapDisplay beatmapDisplay) {
		_selectedBeatmaps.Add(_databasesModel.ScoreDB.Beatmaps[beatmapDisplay.BeatmapMD5]);
		SelectedBeatmapsDisplays.Add(new BeatmapDisplay(beatmapDisplay));
	}

	public void RemoveBeatmap(BeatmapDisplay beatmapDisplay) {
		_selectedBeatmaps.Remove(_databasesModel.ScoreDB.Beatmaps[beatmapDisplay.BeatmapMD5]);
		SelectedBeatmapsDisplays.Remove(beatmapDisplay);
	}

	[RelayCommand]
	public void MoveUp(BeatmapDisplay beatmapDisplay) {
		int index = SelectedBeatmapsDisplays.IndexOf(beatmapDisplay);
		if (index > 0) {
			var beatmapSwap = _selectedBeatmaps[index];
			_selectedBeatmaps.Remove(beatmapSwap);
			_selectedBeatmaps.Insert(index - 1, beatmapSwap);
			SelectedBeatmapsDisplays.Move(index, index - 1);
		}
	}

	[RelayCommand]
	public void MoveDown(BeatmapDisplay beatmapDisplay) {
		int index = SelectedBeatmapsDisplays.IndexOf(beatmapDisplay);
		if (index < SelectedBeatmapsDisplays.Count - 1) {
			var beatmapSwap = _selectedBeatmaps[index];
			_selectedBeatmaps.Remove(beatmapSwap);
			_selectedBeatmaps.Insert(index + 1, beatmapSwap);
			SelectedBeatmapsDisplays.Move(index, index + 1);
		}
	}

	public void FillParametersMetadataWithFirstBeatmap() {
		if (_selectedBeatmaps.Count == 0) return;
		var firstBeatmap = _selectedBeatmaps.First();
		var metadata = Parameters.Metadata.DeepClone();
		metadata.FillMetadataParametersWithBeatmapDataFromDB(firstBeatmap.BeatmapDBInfo);
		Parameters.Metadata = metadata;
	}

	public void FillParametersDifficultyWithFirstBeatmap() {
		if (_selectedBeatmaps.Count == 0) return;
		var firstBeatmap = _selectedBeatmaps.First();
		var difficulty = Parameters.Difficulty.DeepClone();
		difficulty.FillDifficultyParametersWithBeatmapDataFromDB(firstBeatmap.BeatmapDBInfo);
		Parameters.Difficulty = difficulty;
	}

	public ValidationResult ValidateDouble(string strValue) {
			double res;

			if (!double.TryParse(strValue, out res))
				return new (null, "Not a double");

			return new (res, null);
	}

	public ValidationResult ValidateInteger(string strValue) {
			int res;

			if (!int.TryParse(strValue, out res))
				return new (null, "Not an integer");

			return new (res, null);
	}

	public ValidationResult ValidateFilename(string strValue) {
			Regex r = new Regex("[/:*?\"<>|\\\\]");

			if (r.IsMatch(strValue)) {
				return new (null, "A file name can't contain any of the following characters: \\/:*?\"<>|");
			}

			return new (strValue, null);
	}
}
