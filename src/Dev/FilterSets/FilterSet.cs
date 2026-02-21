using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using Mania2mp4.Models;
using Rythmify.Core.Replay;

public class FilterSet : ObservableObject {
	public string Name { get; set; }
	public ObservableCollection<IFilter> BeatmapFilters { get; } = new();
	public ObservableCollection<IFilter> ScoreFilters { get; } = new();

	public FilterSet(string name) {
		Name = name;
		BeatmapFilters.CollectionChanged += OnFiltersCollectionChanged;
		ScoreFilters.CollectionChanged += OnFiltersCollectionChanged;
	}

	private void OnFiltersCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) {
		if (e.OldItems != null) {
			foreach (IFilter filter in e.OldItems)
				filter.PropertyChanged -= OnFilterPropertyChanged;
		}

		if (e.NewItems != null) {
			foreach (IFilter filter in e.NewItems)
				filter.PropertyChanged += OnFilterPropertyChanged;
		}

		OnPropertyChanged("FiltersCollectionChanged");
	}

	private void OnFilterPropertyChanged(object? sender, PropertyChangedEventArgs e) {
		OnPropertyChanged("FilterPropertyChanged");
	}

	public List<ReplayData> GetFilteredBestScoresPerBeatmap(DatabasesService databases, int nbScores) {
		Func<ReplayData, double> scoreOrdering = x => x.PerformancePoints;
		List<ReplayData> topScoresFromFilteredBeatmaps = new();

		foreach (var beatmap in databases.ScoreDB.Beatmaps.Values) {
			if (!BeatmapFilters.All(f => f.Apply(beatmap))) continue;

			foreach (var score in beatmap.Replays.OrderByDescending(scoreOrdering)) {
				if (!ScoreFilters.All(f => f.Apply(score))) continue;
				topScoresFromFilteredBeatmaps.Add(score);
				break;
			}
		}

		topScoresFromFilteredBeatmaps = topScoresFromFilteredBeatmaps.OrderByDescending(scoreOrdering).Take(nbScores).ToList();
		return topScoresFromFilteredBeatmaps;
	}
}
