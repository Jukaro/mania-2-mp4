using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml.Templates;
using CommunityToolkit.Mvvm.ComponentModel;
using Mania2mp4.Displays;
using Mania2mp4.Models;

namespace Mania2mp4.ViewModels;

public partial class SessionViewerViewModel : ViewModelBase {
	private DatabasesService _databases;
	private OsuReplayModel _osuReplay;

	[ObservableProperty]
	private List<DateTime> _sessionDates;

	[ObservableProperty]
	private SessionStatsDisplay _sessionStats = new();

	public SessionScoreListRenderer SessionScoreListRenderer { get; } = new();

	[ObservableProperty]
	private SessionScoreDisplay? _selectedSessionScoreDisplay;

	private Session _selectedSession = new(new());

	private DataTemplate _sessionScoreTemplate;
	private DataTemplate _extendedSessionScoreTemplate;

	[ObservableProperty]
	private DataTemplate _selectedTemplate;

	[ObservableProperty]
	private bool _isExtendedStatsEnabled = false;

	public SessionScoreDisplayManager SessionScoreDisplayManager { get; }

	public SessionViewerViewModel(DatabasesService databasesModel, OsuReplayModel osuReplayModel) {
		_databases = databasesModel;
		_osuReplay = osuReplayModel;
		_databases.DatabasesInitialized += OnDatabasesInitialized;
		PropertyChanged += OnPropertyChanged;

		_sessionScoreTemplate = (DataTemplate)Application.Current.Resources["SessionScoreTemplate"];
		_extendedSessionScoreTemplate = (DataTemplate)Application.Current.Resources["ExtendedSessionScoreTemplate"];
		SelectedTemplate = _sessionScoreTemplate;

		SessionScoreDisplayManager = new(_databases);
	}

	private void OnDatabasesInitialized(object? sender, EventArgs e) {
		Task.Run(async () => {
			await UpdateScores(_databases.SessionList.SessionsOrderedList.Last().Value);
			SessionDates = _databases.SessionList.SessionsOrderedList.Select(o => o.Key).ToList();
		});
	}

	private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) {
		if (e.PropertyName == nameof(IsExtendedStatsEnabled)) {
			Task.Run(() => UpdateScores(_selectedSession));
		} else if (e.PropertyName == nameof(SelectedSessionScoreDisplay)) {
			if (SelectedSessionScoreDisplay == null
				|| !_databases.ScoreDB.Beatmaps.ContainsKey(SelectedSessionScoreDisplay.BeatmapDisplay.BeatmapMD5))
				return;

			var display = SelectedSessionScoreDisplay;

			var beatmap = _databases.BeatmapDB.Beatmaps[display.BeatmapDisplay.BeatmapMD5];
			_osuReplay.Beatmap = _databases.ScoreDB.Beatmaps[beatmap.BeatmapMD5];
			_osuReplay.Score = display.Replay;
		}
	}

	private async Task UpdateScores(Session session) {
		_selectedSession = session;

		SessionScoreDisplayManager.Scores = session.Replays;
		SessionStats = new SessionStatsDisplay(_databases, session);

		if (IsExtendedStatsEnabled) SelectedTemplate = _extendedSessionScoreTemplate;
		else SelectedTemplate = _sessionScoreTemplate;
	}

	public void OnDateChanged(object? sender, SelectionChangedEventArgs e) {
		if (e.AddedItems[0] is DateTime date && _databases.SessionList.SessionsDict.TryGetValue(date, out Session session)) {
			Task.Run(() => UpdateScores(session));
		}
	}

	public void RenderScoresList() {
		SessionScoreListRenderer.RenderScoresList(SessionScoreDisplayManager.Displays, SelectedTemplate, _selectedSession.DateTime);
	}
}
