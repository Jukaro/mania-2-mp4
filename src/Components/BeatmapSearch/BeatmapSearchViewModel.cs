using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Mania2mp4.Models;
using Rythmify.Core;
using Rythmify.Core.Beatmap;
using Rythmify.Core.Databases;

namespace Mania2mp4.ViewModels;

public partial class BeatmapSearchViewModel : ViewModelBase {
	private DatabasesService _databases;

	private List<BeatmapDataFromDB> _searchResults = new();

	public BeatmapDisplayManager BeatmapDisplayManager { get; set; }

	private string _query = "";

	public string Query {
		get => _query;
		set {
			if (_query == value) return;
			_query = value;
			UpdateSearchResults(_query);
		}
	}

	[ObservableProperty]
	private int _searchResultsCount = 0;

	[ObservableProperty]
	private int _pageCount = 0;

	[ObservableProperty]
	private int _currentPage = 0;

	private int _currentPageIndex = 0;

	public int CurrentPageIndex {
		get { return _currentPageIndex; }
		set {
			_currentPageIndex = value;
			CurrentPage = CurrentPageIndex + 1;
		}
	}

	public int PageSize = 20;

	public BeatmapSearchViewModel(DatabasesService databases) {
		_databases = databases;
		_databases.DatabasesInitialized += OnDatabasesInitialized;

		BeatmapDisplayManager = new(_databases);
	}

	public BeatmapDataFromDB GetBeatmapDataFromMD5(string beatmapMD5) {
		BeatmapDataFromDB beatmapDataFromDB = new();

		if (_databases.BeatmapDB == null || !_databases.BeatmapDB.Beatmaps.ContainsKey(beatmapMD5))
			return beatmapDataFromDB;

		return _databases.BeatmapDB.Beatmaps[beatmapMD5];
	}

	private void OnDatabasesInitialized(object? sender, EventArgs e) {
		UpdateSearchResults("");
	}

	private void UpdateVisibleItems()
	{
		if (_searchResults == null) return;

		var page = _searchResults.Skip(CurrentPageIndex * PageSize).Take(PageSize);
		BeatmapDisplayManager.Beatmaps = page.Select(b => new BeatmapWithScores(b)).ToList();
	}

	public void FirstPage() {
		CurrentPageIndex = 0;
		UpdateVisibleItems();
	}

	public void PreviousPage() {
		if (CurrentPageIndex > 0) {
			CurrentPageIndex--;
			UpdateVisibleItems();
		}
	}

	public void NextPage() {
		if (_searchResults == null) return;
		if ((CurrentPageIndex + 1) * PageSize < _searchResults.Count) {
			CurrentPageIndex++;
			UpdateVisibleItems();
		}
	}

	public void LastPage() {
		if (_searchResults == null) return;
		var totalItems = _searchResults.Count;
		CurrentPageIndex = (totalItems - 1) / PageSize;
		UpdateVisibleItems();
	}

	private void SetPageInfo() {
		PageCount = _searchResults.Count / PageSize + (_searchResults.Count % PageSize > 0 ? 1 : 0);
		CurrentPageIndex = 0;
	}

	public void UpdateSearchResults(string query) {
		if (_databases.BeatmapDB == null) return;

		Task.Run(async () => {
			try {
				await UpdateSearchResultsTask(query);
			} catch (Exception e) {
				Logger.LogError($"{e.Message}\nstacktrace: {e.StackTrace}");
			}
		});
	}

	private async Task UpdateSearchResultsTask(string query) {
		var watch = new Stopwatch();
		watch.Start();

		// var splittedQuery = query.Split(' ');

		var results = _databases.BeatmapDB.Beatmaps.Values.Where(beatmap => {
			string searchString = "";
			searchString += beatmap.SongTitle + " ";
			searchString += beatmap.ArtistName + " ";
			searchString += beatmap.Difficulty + " ";
			searchString += beatmap.SongTags + " ";
			searchString += beatmap.CreatorName;

			// faire un || avec les elements du split
			// foreach (string subQuery in splittedQuery) {
			// 	if (searchString.Contains(subQuery, System.StringComparison.OrdinalIgnoreCase))
			// 		return true;
			// }

			// return false;
			return searchString.Contains(query, System.StringComparison.OrdinalIgnoreCase);
		});

		Dispatcher.UIThread.Post(() => {
			_searchResults = results.ToList();
			watch.Stop();
			// Logger.LogDebug($"query: {Query} ({_searchResults.Count} results in {watch.ElapsedMilliseconds}ms)");
			FirstPage();
			SetPageInfo();
			SearchResultsCount = _searchResults.Count;
		});
	}
}
