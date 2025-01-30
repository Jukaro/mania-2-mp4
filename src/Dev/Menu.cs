using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Rythmify.Core;
using Rythmify.Core.Game;
using Rythmify.Core.Replay;
using Rythmify.Core.Databases;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using Rythmify.Dev;
using System.IO;
using System;

namespace Rythmify.UI;

public class Menu {
	private readonly GraphicsDevice _graphics;
	private KeyboardKey _volumeUp;
	private KeyboardKey _volumeDown;

	private KeyboardKey _numPad8;

	private KeyboardKey _F3;
	private KeyboardKey _F4;

	private UIElementContainer _mainContainer;
	private ScrollableUIElementContainer _sideMenu;
	private ReplaySelector _replaySelector;
	private BeatmapSelector _beatmapSelector;
	private InputBox _searchBar;
	private string _query;

	private bool _play;
	private bool _isPlaying;

	private int _beatmapsPageIndex = 0;
	private BeatmapDB _beatmapsDB;
	private ScoreDB _scoresDB;
	private List<BeatmapWithScores> _beatmapsList;
	private List<BeatmapWithScores> _sortedBeatmapsList;

	SessionList _sessionList;

	List<ReplayData> _allScores = new();
	List<ReplayData> _top100Scores = new();

	public Menu(GraphicsDevice graphics) {
		_graphics = graphics;
		_volumeUp = new(Keys.NumPad1);
		_volumeDown = new(Keys.NumPad2);
		_numPad8 = new(Keys.NumPad8);
		_F3 = new(Keys.F3);
		_F4 = new(Keys.F4);

		_play = false;
		_isPlaying = false;
	}

	public void Init() {
		SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());

		FontsStore.InitFonts();
		GradientStore.InitGradients();
		VisualsStore.InitVisuals(_graphics, GradientStore.gradients);

		_mainContainer = new UIElementContainer(_graphics, _graphics.Viewport.Bounds.Width, _graphics.Viewport.Bounds.Height, new Vector2(0, 0), "mainContainer", Color.Transparent);

		_sideMenu = new ScrollableUIElementContainer(_graphics, _graphics.Viewport.Bounds.Width - 1000, 1000, new(1000, 0), "sideMenu", new Color(100, 100, 100));
		_mainContainer.Add(_sideMenu);

		_sideMenu.Add(new ToggleButton(_graphics, new(0, 50 * 2 + 10 * 2), "Play", Play, Pause, VisualsStore.visuals[0]));
		_sideMenu.Add(new Button(_graphics, new(200, 50 * 2 + 10 * 2), "NextPage", VisualsStore.visuals[0]));
		_sideMenu.Add(new Button(_graphics, new(400, 50 * 2 + 10 * 2), "PreviousPage", VisualsStore.visuals[0]));
		_sideMenu.Add(new Button(_graphics, new(600, 50 * 2 + 10 * 2), "DisplayScores", VisualsStore.visuals[0]));
		if (_sideMenu["DisplayScores"] is Button displayScores)
			displayScores.SetOnClick(() => _replaySelector.Hide = !_replaySelector.Hide);

		if (_sideMenu["NextPage"] is Button button)
			button.SetOnClick(NextPage);
		if (_sideMenu["PreviousPage"] is Button button2)
			button2.SetOnClick(PreviousPage);

		_beatmapsDB = BeatmapDBParser.Parse(Path.Combine(Paths.OsuDirectoryPath, "osu!.db"));
		_scoresDB = ScoreDBParser.Parse(Path.Combine(Paths.OsuDirectoryPath, "scores.db"), _beatmapsDB);

		_replaySelector = new ReplaySelector(_graphics, new(0, 50 * 3 + 10 * 3), "replaySelector", 10, VisualsStore.visuals[6], VisualsStore.visuals[7]);
		_replaySelector.Hide = true;
		_mainContainer.Add(_replaySelector);

		_searchBar = new InputBox(_graphics, new (0, 0), "inputBox", VisualsStore.visuals[0]);
		_searchBar.Visuals.Resize(600, VisualsStore.visuals[0].Height);
		_sideMenu.Add(_searchBar);

		_query = "";
		_beatmapsList = _scoresDB.Beatmaps.Values.ToList();

		_sortedBeatmapsList = FilterSongs(_query);

		_beatmapSelector = new BeatmapSelector(_graphics, new(0, 50 * 3 + 10 * 3), "beatmapSelector", 10, VisualsStore.visuals[4], VisualsStore.visuals[5]);
		_beatmapSelector.SetColor(Color.Transparent);
		_sideMenu.Add(_beatmapSelector);
		_beatmapSelector.Init(_sortedBeatmapsList, 0, _replaySelector);

/* -------------------------------------------------------------------------- */
/*                                  Sessions                                  */
/* -------------------------------------------------------------------------- */

		_sessionList = new SessionList(_scoresDB);

		Logger.LogInfo($"SessionList: {_sessionList}");

		var watch = new System.Diagnostics.Stopwatch();
		watch.Start();

		_sideMenu.Add(new ReplaySelector(_graphics, new(0, 2000), "SessionReplays", 10, VisualsStore.visuals[6], VisualsStore.visuals[7]));

		_sideMenu.Add(new Dropdown(_graphics, new(0, 1000), 0, "Sessions", VisualsStore.visuals[8]));
		if (_sideMenu["Sessions"] is Dropdown sessionsButton) {
			// for (int i = 0; i < _sessionList.SessionsOrderedList.Count(); i++) {
			for (int i = 0; i < 100; i++) {
				sessionsButton.Add(new Button(_graphics, new(0, 0), "Session" + i, VisualsStore.visuals[9]));
				sessionsButton[i].Visuals.Resize(sessionsButton.UsableWidth, VisualsStore.visuals[9].Height);
				sessionsButton[i].Visuals.Texts.Add(new Text(_sessionList.SessionsOrderedList.ElementAt(i).Key.ToString(), new Vector2(0, 0)));

				int index = i;
				if (sessionsButton[i] is Button button1) {
					button1.SetOnClick(() => { UpdateSession(index); });
				}
			}
		}

		watch.Stop();
		Logger.LogDebug($"Sessions: {watch.ElapsedMilliseconds}ms");

/* -------------------------------------------------------------------------- */
/*                               Scores Top 100                               */
/* -------------------------------------------------------------------------- */

		DateTime limit = new DateTime(2021, 01, 01);

		Logger.LogDebug($"limit: {limit}");

		List<ReplayData> filteredScores = new();

		foreach (BeatmapWithScores beatmap in _scoresDB.Beatmaps.Values) {
			beatmap.Replays = beatmap.Replays
				.OrderByDescending(r => r.PerformancePoints)
				.ToList();
			filteredScores = beatmap.Replays
				.Where(r => r.TimeStamp < limit)
				.ToList();
			if (filteredScores.Count > 0) {
				_allScores.Add(filteredScores[0]);
				// Logger.LogDebug($"Adding {filteredScores[0].TimeStamp}");
			}
		}

		_top100Scores =	_allScores
							.OrderByDescending(r => r.PerformancePoints)
							.Where(r => r.PlayerName == "Jukaro"
								&& r.LaneCount == 4
								&& _beatmapsDB.Beatmaps[r.BeatmapMD5].RankedStatus == 4)
							.Take(100)
							.ToList();

		watch.Restart();

		_sideMenu.Add(new ReplaySelector(_graphics, new(0, 3000), "Top 100", 10, VisualsStore.visuals[6], VisualsStore.visuals[7]));
		if (_sideMenu["Top 100"] is ReplaySelector top100Dropdown) {
			top100Dropdown.UpdateScores(_top100Scores, _beatmapsDB);
		}

		watch.Stop();
		Logger.LogDebug($"Top 100: {watch.ElapsedMilliseconds}ms");
	}

	private void UpdateSession(int i) {
		if (_sideMenu["SessionReplays"] is ReplaySelector rs) {
			rs.UpdateScores(_sessionList.SessionsOrderedList.ElementAt(i).Value.Replays, _beatmapsDB);
			Logger.LogDebug($"Number of scores: {_sessionList.SessionsOrderedList.ElementAt(i).Value.Replays.Count}");
		}
	}

	private void NextPage() {
		_beatmapsPageIndex++;
		_beatmapSelector.UpdateBeatmapsDropdown(_sortedBeatmapsList, _beatmapsPageIndex * _beatmapSelector.DisplayedBeatmapsCount, _replaySelector);
	}

	private void PreviousPage() {
		if (_beatmapsPageIndex == 0)
			return;
		_beatmapsPageIndex--;
		_beatmapSelector.UpdateBeatmapsDropdown(_sortedBeatmapsList, _beatmapsPageIndex * _beatmapSelector.DisplayedBeatmapsCount, _replaySelector);
	}

	private void Play() {
		_play = true;
		_sideMenu["Play"].SetColor(new Color(0, 255, 0));
	}

	private void Pause() {
		_play = false;
		_sideMenu["Play"].SetColor(new Color(255, 0, 0));
	}

	public void Update(ref BeatmapPlayer beatmapPlayer, ref InputsPlayer inputsPlayer, ref AudioPlayer audioPlayer, ReplayData replay, ReplaySkinData skin) {
		MouseManager.Update();

		if (_replaySelector.NeedToUpdatePlayers) {

			if (_beatmapSelector.SelectedBeatmap != null && _replaySelector.SelectedReplay != null) {
				beatmapPlayer = new BeatmapPlayer(_beatmapSelector.SelectedBeatmap.Beatmap, skin.ManiaSection.HitPosition);
				inputsPlayer = new InputsPlayer(_replaySelector.SelectedReplay);
				audioPlayer = new AudioPlayer(_beatmapSelector.SelectedBeatmap.AudioPath);
			}
			_replaySelector.NeedToUpdatePlayers = false;
		}

		if (_searchBar.Input != null && _searchBar.Input != _query) {
			_query = _searchBar.Input;

			_sortedBeatmapsList = FilterSongs(_query);

			_beatmapSelector.UpdateBeatmapsDropdown(_sortedBeatmapsList, _beatmapsPageIndex * _beatmapSelector.DisplayedBeatmapsCount, _replaySelector);
		}

		if (_play && !_isPlaying) {
			beatmapPlayer.Play();
			inputsPlayer.Play();
			audioPlayer.Play();
			_isPlaying = true;
		}
		else if (!_play && _isPlaying) {
			beatmapPlayer.Pause();
			inputsPlayer.Pause();
			audioPlayer.Pause();
			_isPlaying = false;
		}

		if (_numPad8.IsPressed()) {
			beatmapPlayer.Reset(replay);
			inputsPlayer.Init(replay);
			audioPlayer.Reset();
			_isPlaying = false;

			// beatmapPlayer.Play();
			// inputsPlayer.Play();
			// audioPlayer.Play();
		}

		if (_volumeUp.IsPressed())
			audioPlayer.VolumeUp();
		if (_volumeDown.IsPressed())
			audioPlayer.VolumeDown();

		if (_F3.IsPressed())
			beatmapPlayer.ScrollSpeedDown();

		if (_F4.IsPressed())
			beatmapPlayer.ScrollSpeedUp();

		_mainContainer.Update();
	}

	private List<BeatmapWithScores> FilterSongs(string query) {
		return _beatmapsList.Where(beatmap => beatmap.BeatmapDBInfo.SongTitle.Contains(query, System.StringComparison.OrdinalIgnoreCase)).ToList();
	}

	public void Render(SpriteBatch spriteBatch) {
		_mainContainer.Render(spriteBatch);
	}
}
