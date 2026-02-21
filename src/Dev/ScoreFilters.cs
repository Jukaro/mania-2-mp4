using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using Rythmify.Core.Shared;

namespace Rythmify.Dev;

public partial class FilterItem<T> : ObservableObject {
	[ObservableProperty]
	private List<object> _itemsSource = new();

	public ObservableCollection<object> SelectedItems { get; set; } = new();

	[ObservableProperty]
	private List<T> _items = new();

	public FilterItem() {
		SelectedItems.CollectionChanged += OnSelectedItemsChanged;
	}

	public FilterItem(List<object> itemsSource) {
		ItemsSource = itemsSource;
		SelectedItems.CollectionChanged += OnSelectedItemsChanged;
	}

	private void OnSelectedItemsChanged(object? sender, NotifyCollectionChangedEventArgs e) {
		Items = SelectedItems.Cast<T>().ToList();
	}

	public override string ToString() {
		return $"Filter<{typeof(T)}> (source items count: {ItemsSource.Count}, items count: {Items.Count}, first 5 items: {string.Join(",", ItemsSource.Take(5))})";
	}
}

public partial class ScoreFilter : ObservableObject {
	[ObservableProperty]
	private FilterItem<string> _players = new();

	[ObservableProperty]
	private FilterItem<int> _keyModes = new();

	[ObservableProperty]
	private FilterItem<RankedStatus> _rankedStatuses = new();

	[ObservableProperty]
	private DateTime _upperLimitDate = DateTime.Now.AddDays(1);

	public ScoreFilter() {
		SubscribeToFieldsPropertyChangedEvents();
	}

	public ScoreFilter(ScoreFilter scoreFilter) {
		Players = scoreFilter.Players;
		KeyModes = scoreFilter.KeyModes;
		RankedStatuses = scoreFilter.RankedStatuses;
		SubscribeToFieldsPropertyChangedEvents();
	}

	public ScoreFilter(FilterItem<string> players, FilterItem<int> keyModes, FilterItem<RankedStatus> rankedStatuses) {
		Players = players;
		KeyModes = keyModes;
		RankedStatuses = rankedStatuses;
		SubscribeToFieldsPropertyChangedEvents();
	}

	private void SubscribeToFieldsPropertyChangedEvents() {
		Players.PropertyChanged += (sender, e) => { OnPropertyChanged(nameof(Players)); };
		KeyModes.PropertyChanged += (sender, e) => { OnPropertyChanged(nameof(KeyModes)); };
		RankedStatuses.PropertyChanged += (sender, e) => { OnPropertyChanged(nameof(RankedStatuses)); };
	}
}
