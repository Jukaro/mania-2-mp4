using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Mania2mp4.Models;
using Rythmify.Core.Databases;
using Rythmify.Core.Shared;
using SkiaSharp;

namespace Mania2mp4.ViewModels;

public partial class BeatmapDBStatsViewModel : ViewModelBase {
	private DatabasesService _databases;

	public List<string> GameModes { get; set; }

	[ObservableProperty]
	private string _selectedGameMode = "All";

	[ObservableProperty]
	private ObservableCollection<PieSeries<int>> _gameModePieChart = new();

	[ObservableProperty]
	private ObservableCollection<PieSeries<int>> _rankedStatusPieChart = new();

	[ObservableProperty]
	private ObservableCollection<PieSeries<int>> _gradesPieChart = new();

	[ObservableProperty]
	private ObservableCollection<PieSeries<int>> _playStatusPieChart = new();

	public SolidColorPaint LegendTextPaint { get; } = new SolidColorPaint(SKColors.White);

	public BeatmapDBStatsViewModel(DatabasesService databasesModel) {
		_databases = databasesModel;
		_databases.DatabasesInitialized += OnDatabasesInitialized;

		GameModes = Enum.GetNames(typeof(GameMode)).ToList();
		GameModes.Insert(0, "All");

		PropertyChanged += OnPropertyChanged;
	}

	private void OnDatabasesInitialized(object? sender, EventArgs e) {
		UpdateCharts();
	}

	private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) {
		if (e.PropertyName == nameof(SelectedGameMode)) UpdateCharts();
	}

	private void UpdateCharts() {
		UpdateBeatmapDBGameModesPieChart();
		UpdateBeatmapDBRankedStatusPieChart();
		UpdateBeatmapDBGradesPieChart();
		UpdateBeatmapDBPlayStatusPieChart();
	}

	private void UpdateBeatmapDBGameModesPieChart() {
		Dictionary<string, int> values = new() {
			{ "Standard", 0 },
			{ "Mania", 0 },
			{ "Taiko", 0 },
			{ "Catch The Beat", 0 }
		};

		foreach (var beatmap in _databases.BeatmapDB.Beatmaps.Values) {
			if (beatmap.Mode == GameMode.Standard) values["Standard"]++;
			else if (beatmap.Mode == GameMode.Mania) values["Mania"]++;
			else if (beatmap.Mode == GameMode.Taiko) values["Taiko"]++;
			else if (beatmap.Mode == GameMode.CatchTheBeat) values["Catch The Beat"]++;
		}

		if (GameModePieChart.Count == 0) ChartsHelper.CreatePieChart(GameModePieChart, values, 8);
		else ChartsHelper.UpdatePieChart(GameModePieChart, values);
	}

	private void UpdateBeatmapDBRankedStatusPieChart() {
		Dictionary<string, int> values = new() {
			{ "Ranked", 0 },
			{ "Loved", 0 },
			{ "Unranked", 0 }
		};

		GameMode? gameMode = null;
		if (SelectedGameMode != "All") gameMode = Enum.Parse<GameMode>(SelectedGameMode);

		IEnumerable<BeatmapDataFromDB> beatmaps = _databases.BeatmapDB.Beatmaps.Values;
		if (SelectedGameMode != "All") beatmaps = beatmaps.Where(b => b.Mode == gameMode);

		foreach (var beatmap in beatmaps) {
			if ((RankedStatus)beatmap.RankedStatus == RankedStatus.Ranked) values["Ranked"]++;
			else if ((RankedStatus)beatmap.RankedStatus == RankedStatus.Loved) values["Loved"]++;
			else values["Unranked"]++;
		}

		if (RankedStatusPieChart.Count == 0) ChartsHelper.CreatePieChart(RankedStatusPieChart, values, 8);
		else ChartsHelper.UpdatePieChart(RankedStatusPieChart, values);
	}

	private void UpdateBeatmapDBGradesPieChart() {
		Dictionary<string, int> values = new() {
			{ "SS", 0 },
			{ "S", 0 },
			{ "A", 0 },
			{ "B", 0 },
		};

		Dictionary<GameMode, Func<BeatmapDataFromDB, byte>> beatmapDataToGameModeGrade = new() {
			{ GameMode.Standard, b => b.StandardGrade },
			{ GameMode.Mania, b => b.ManiaGrade },
			{ GameMode.Taiko, b => b.TaikoGrade },
			{ GameMode.CatchTheBeat, b => b.CatchTheBeatGrade }
		};

		GameMode? gameMode = null;
		if (SelectedGameMode != "All") gameMode = Enum.Parse<GameMode>(SelectedGameMode);

		IEnumerable<BeatmapDataFromDB> beatmaps = _databases.BeatmapDB.Beatmaps.Values;
		if (SelectedGameMode != "All") beatmaps = beatmaps.Where(b => b.Mode == gameMode);

		List<Func<BeatmapDataFromDB, byte>> gameModeGradeGetters = beatmapDataToGameModeGrade.Values.ToList();
		if (SelectedGameMode != "All" && gameMode != null)
			gameModeGradeGetters = [beatmapDataToGameModeGrade[(GameMode)gameMode]];

		foreach (var beatmap in beatmaps) {
			foreach (var gameModeGradeGetter in gameModeGradeGetters) {
				ScoreGrade grade = (ScoreGrade)gameModeGradeGetter(beatmap);
				string gradeStr = Enum.GetName(typeof(ScoreGrade), grade);
				if (grade == ScoreGrade.SilverSS) grade = ScoreGrade.SS;
				if (grade == ScoreGrade.SilverS) grade = ScoreGrade.S;
				if (values.ContainsKey(gradeStr)) values[gradeStr]++;
			}
		}

		if (GradesPieChart.Count == 0) ChartsHelper.CreatePieChart(GradesPieChart, values, 8);
		else ChartsHelper.UpdatePieChart(GradesPieChart, values);
	}

	private void UpdateBeatmapDBPlayStatusPieChart() {
		Dictionary<string, int> values = new() {
			{ "Played", 0 },
			{ "Unplayed", 0 }
		};

		GameMode? gameMode = null;
		if (SelectedGameMode != "All") gameMode = Enum.Parse<GameMode>(SelectedGameMode);

		IEnumerable<BeatmapDataFromDB> beatmaps = _databases.BeatmapDB.Beatmaps.Values;
		if (SelectedGameMode != "All") beatmaps = beatmaps.Where(b => b.Mode == gameMode);

		foreach (var beatmap in beatmaps) {
			if (beatmap.IsUnplayed == true) values["Unplayed"]++;
			else values["Played"]++;
		}

		if (PlayStatusPieChart.Count == 0) ChartsHelper.CreatePieChart(PlayStatusPieChart, values, 8);
		else ChartsHelper.UpdatePieChart(PlayStatusPieChart, values);
	}
}
