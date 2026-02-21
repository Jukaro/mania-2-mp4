using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData;
using Mania2mp4.Models;
using Rythmify.Core.Beatmap;
using Rythmify.Core.Replay;
using Rythmify.Core.Shared;

namespace Mania2mp4.ViewModels;

struct DefaultScoreFilters {
	public AllowedValuesFilter<ReplayData, string> SharedPlayerFilter;

	public DefaultScoreFilters() {
		SharedPlayerFilter = new("Shared Player Filter") {
			ValueGetter = x => x.PlayerName
		};
	}
}

struct DefaultBeatmapFilters {
	public AllowedValuesFilter<BeatmapWithScores, int> _4KFilter;
	public AllowedValuesFilter<BeatmapWithScores, int> _7KFilter;
	public AllowedValuesFilter<BeatmapWithScores, RankedStatus> RankedFilter;

	public AllowedValuesFilter<BeatmapWithScores, int> CustomKeyModesFilter;
	public AllowedValuesFilter<BeatmapWithScores, RankedStatus> CustomRankedStatusesFilter;

	public DefaultBeatmapFilters() {
		_4KFilter = new("4K Filter") {
			ValueGetter = x => (int)x.BeatmapDBInfo.CircleSize,
			AllowedValues = [4]
		};

		_7KFilter = new("7K Filter") {
			ValueGetter = x => (int)x.BeatmapDBInfo.CircleSize,
			AllowedValues = [7]
		};

		RankedFilter = new("Ranked Filter") {
			ValueGetter = x => (RankedStatus)x.BeatmapDBInfo.RankedStatus,
			AllowedValues = [RankedStatus.Ranked]
		};

		CustomKeyModesFilter = new("Custom Key Modes Filter") {
			ValueGetter = x => (int)x.BeatmapDBInfo.CircleSize
		};

		CustomRankedStatusesFilter = new("Custom Ranked Statuses Filter") {
			ValueGetter = x => (RankedStatus)x.BeatmapDBInfo.RankedStatus
		};
	}
}

public partial class FilterSetsViewModel : ViewModelBase {
	private DatabasesService _databases;
	private FilterSetsService _filterSetsService;

	private Dictionary<string, FilterSet> _defaultFilterSets = new();

	private DefaultScoreFilters _defaultScoreFilters = new();
	private DefaultBeatmapFilters _defaultBeatmapFilters = new();

	[ObservableProperty]
	private HashSet<string> _players = new();

	[ObservableProperty]
	private HashSet<int> _keyModes = Enumerable.Range(1, 10).ToHashSet();

	[ObservableProperty]
	private HashSet<RankedStatus> _rankedStatuses = Enum.GetValues<RankedStatus>().ToHashSet();

	public ObservableCollection<string> SelectedPlayers { get; } = new();
	public ObservableCollection<int> SelectedKeyModes { get; } = new();
	public ObservableCollection<RankedStatus> SelectedRankedStatuses { get; } = new();

	public FilterSetsViewModel(DatabasesService databasesService, FilterSetsService filterSetsService) {
		_databases = databasesService;
		_databases.DatabasesInitialized += OnDatabasesInitialized;

		SelectedPlayers.CollectionChanged += OnSelectedPlayersChanged;
		SelectedKeyModes.CollectionChanged += OnSelectedKeyModesChanged;
		SelectedRankedStatuses.CollectionChanged += OnSelectedRankedStatusesChanged;

		_filterSetsService = filterSetsService;
		InitDefaultFilterSets();
	}

	private void OnSelectedPlayersChanged(object? sender, NotifyCollectionChangedEventArgs e) {
		_defaultScoreFilters.SharedPlayerFilter.AllowedValues = [.. SelectedPlayers];
	}

	private void OnSelectedKeyModesChanged(object? sender, NotifyCollectionChangedEventArgs e) {
		_defaultBeatmapFilters.CustomKeyModesFilter.AllowedValues = [.. SelectedKeyModes];
	}

	private void OnSelectedRankedStatusesChanged(object? sender, NotifyCollectionChangedEventArgs e) {
		_defaultBeatmapFilters.CustomRankedStatusesFilter.AllowedValues = [.. SelectedRankedStatuses];
	}

	private void OnDatabasesInitialized(object? sender, EventArgs e) {
		if (!_defaultFilterSets.Values.Any(_filterSetsService.FiltersSets.Contains))
			_filterSetsService.FiltersSets.AddRange(_defaultFilterSets.Values);

		InitPlayerList();
	}

	private void InitDefaultFilterSets() {
		FilterSet Ranked4K = new("4K Ranked");
		Ranked4K.BeatmapFilters.Add(_defaultBeatmapFilters._4KFilter);
		Ranked4K.BeatmapFilters.Add(_defaultBeatmapFilters.RankedFilter);
		Ranked4K.ScoreFilters.Add(_defaultScoreFilters.SharedPlayerFilter);

		FilterSet Ranked7K = new("7K Ranked");
		Ranked7K.BeatmapFilters.Add(_defaultBeatmapFilters._7KFilter);
		Ranked7K.BeatmapFilters.Add(_defaultBeatmapFilters.RankedFilter);
		Ranked7K.ScoreFilters.Add(_defaultScoreFilters.SharedPlayerFilter);

		FilterSet CustomFilter = new("Custom Filter");
		CustomFilter.BeatmapFilters.Add(_defaultBeatmapFilters.CustomKeyModesFilter);
		CustomFilter.BeatmapFilters.Add(_defaultBeatmapFilters.CustomRankedStatusesFilter);
		CustomFilter.ScoreFilters.Add(_defaultScoreFilters.SharedPlayerFilter);

		_defaultFilterSets.Add(Ranked4K.Name, Ranked4K);
		_defaultFilterSets.Add(Ranked7K.Name, Ranked7K);
		_defaultFilterSets.Add(CustomFilter.Name, CustomFilter);
	}

	private void InitPlayerList() {
		HashSet<string> playerList = new();

		foreach (BeatmapWithScores beatmap in _databases.ScoreDB.Beatmaps.Values) {
			foreach (ReplayData r in beatmap.Replays)
				if (!playerList.Contains(r.PlayerName)) playerList.Add(r.PlayerName);
		}

		Players = playerList;
	}
}
