using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData;
using Mania2mp4.Displays;
using Mania2mp4.Models;
using Rythmify.Core.Replay;
using Rythmify.Core.Shared;
using Rythmify.Dev;

namespace Mania2mp4.ViewModels;

public class TotalWeightedPPDetails {
	public double TotalWeightedPP { get; set; }
	public double TotalPP { get; set; }
	public double BonusPP { get; set; }

	public TotalWeightedPPDetails(double totalWeightedPP, double bonusPP) {
		TotalWeightedPP = totalWeightedPP;
		BonusPP = bonusPP;
		TotalPP = TotalWeightedPP + BonusPP;
	}
}

public partial class ScoreList : ObservableObject {
	private DatabasesService _databases;

	[ObservableProperty]
	private FilterSet _filterSet;

	[ObservableProperty]
	private TotalWeightedPPDetails _totalWeightedPPDetails = new(0, 0);

	public List<ReplayData> Scores = new();
	public Top100ScoreDisplayManager DisplayManager { get; set; }

	private ManagedTaskPool _managedTaskPool = new(1);

	public ScoreList(DatabasesService databases) {
		_databases = databases;
		DisplayManager = new(_databases);
		PropertyChanged += OnPropertyChanged;
	}

	private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) {
		if (e.PropertyName == nameof(FilterSet)) {
			FilterSet.PropertyChanged += OnFilterSetChanged;
			_managedTaskPool.AddTaskToPool(UpdateScores);
		}
	}

	private void OnFilterSetChanged(object? sender, PropertyChangedEventArgs e) {
		_managedTaskPool.AddTaskToPool(UpdateScores);
	}

	public async Task UpdateScores() {
		Scores = FilterSet.GetFilteredBestScoresPerBeatmap(_databases, 100);
		DisplayManager.Scores = Scores;
		TotalWeightedPPDetails = new(ScoreListHelper.GetWeightedPerfomancePoints(Scores.ToList()), GetBonusPP(FilterSet));
	}

	private double GetBonusPP(FilterSet filterSet) {
		int nbRankedBeatmapsPlayed = _databases.ScoreDB.Beatmaps.Values.Count(b => filterSet.BeatmapFilters.All(f => f.Apply(b)
			&& b.BeatmapDBInfo.ManiaGrade != (byte)ScoreGrade.None));
		double bonusPP = 416.6667 * (1 - Math.Pow(0.995, Math.Min(nbRankedBeatmapsPlayed, 1000)));
		return bonusPP;
	}
}

public class ScoreTabViewModel {
	public string Title { get; }
	public ScoreList ScoreList { get; }

	public ScoreTabViewModel(string title, ScoreList scoreList) {
		Title = title;
		ScoreList = scoreList;
	}
}

public partial class Top100ViewModel : ViewModelBase {
	private DatabasesService _databases;
	private OsuReplayModel _osuReplay;
	private FilterSetsService _filterSetsService;

	public ObservableCollection<ScoreTabViewModel> Tabs { get; } = new();

	[ObservableProperty]
	private Top100ScoreDisplay? _selectedTop100ScoreDisplay;

	public Top100ViewModel(DatabasesService databasesModel, OsuReplayModel osuReplay, FilterSetsService filterSetsService) {
		_databases = databasesModel;
		_osuReplay = osuReplay;
		_filterSetsService = filterSetsService;

		_filterSetsService.FiltersSets.CollectionChanged += OnFilterSetsChanged;
		PropertyChanged += OnPropertyChanged;
	}

	private async void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) {
		if (e.PropertyName == nameof(SelectedTop100ScoreDisplay)) {
			if (SelectedTop100ScoreDisplay == null
				|| !_databases.ScoreDB.Beatmaps.ContainsKey(SelectedTop100ScoreDisplay.BeatmapDisplay.BeatmapMD5))
				return;

			var display = SelectedTop100ScoreDisplay;

			var beatmap = _databases.BeatmapDB.Beatmaps[display.BeatmapDisplay.BeatmapMD5];
			_osuReplay.Beatmap = _databases.ScoreDB.Beatmaps[beatmap.BeatmapMD5];
			_osuReplay.Score = display.Replay;
		}
	}

	private async void OnFilterSetsChanged(object? sender, NotifyCollectionChangedEventArgs e) {
		if (e.OldItems != null) {
			foreach (FilterSet filterSet in e.OldItems) {
				var toRemove = Tabs.Where(t => t.Title == filterSet.Name).ToList();
				Tabs.Remove(toRemove);
			}
		}

		if (e.NewItems != null) {
			foreach (FilterSet filterSet in e.NewItems) {
				var scoreList = new ScoreList(_databases);
				scoreList.FilterSet = filterSet;
				var scoreTabViewModel = new ScoreTabViewModel(filterSet.Name, scoreList);
				Tabs.Add(scoreTabViewModel);
			}
		}
	}
}
