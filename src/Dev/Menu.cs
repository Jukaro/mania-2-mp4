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
using System;
using FontStashSharp;
using System.IO;
using Rythmify.Core.Shared;

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

	private List<Visuals> _visualsList = new();

	private FontSystem _fontSystem;

	Dictionary<DateTime, Session> _sessions = new();
	IOrderedEnumerable<KeyValuePair<DateTime, Session>> _sessionsList;

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

	public void InitGradientListsList(List<GradientList> gradientListsList) {
		GradientList RGBGradient = new();
		RGBGradient.Add(new(new(255, 0, 0), new(255, 255, 0)));
		RGBGradient.Add(new(new(255, 255, 0), new(0, 255, 0)));
		RGBGradient.Add(new(new(0, 255, 0), new(0, 255, 255)));
		RGBGradient.Add(new(new(0, 255, 255), new(0, 0, 255)));
		RGBGradient.Add(new(new(0, 0, 255), new(255, 0, 255)));
		RGBGradient.Add(new(new(255, 0, 255), new(255, 0, 0)));
		gradientListsList.Add(RGBGradient);

		GradientList BlackRedGradient = new();
		BlackRedGradient.Add(new(Color.Black, Color.Red));
		BlackRedGradient.Add(new(Color.Red, Color.Black));
		gradientListsList.Add(BlackRedGradient);
	}

	public void InitVisualsList(List<Visuals> _visualsList, List<GradientList> gradientListsList) {
		// 0
		Visuals basicBlackButton = new(_graphics, 100, 50, Color.Black) {
			BlinkOnMouseClick = true,
			BlinkOnMouseOver = true
		};
		_visualsList.Add(basicBlackButton);

		// 1
		Visuals basicRGBButton = new(_graphics, 300, 50, Color.White) {
			BlinkOnMouseClick = true,
			BlinkOnMouseOver = true
		};
		basicRGBButton.SetGradientAsColor(gradientListsList[0], 10);
		_visualsList.Add(basicRGBButton);

		// 2
		Visuals basicBlackRedButton = new(_graphics, 100, 50, Color.White) {
			BlinkOnMouseClick = true,
			BlinkOnMouseOver = true
		};
		basicBlackRedButton.SetGradientAsColor(gradientListsList[1], 100);
		_visualsList.Add(basicBlackRedButton);

		// 3
		Visuals basicThinBlackRedButton = new(_graphics, 100, 10, Color.White) {
			BlinkOnMouseClick = true,
			BlinkOnMouseOver = true
		};
		basicThinBlackRedButton.SetGradientAsColor(gradientListsList[1], 100);
		_visualsList.Add(basicThinBlackRedButton);

		// 4
		Visuals beatmapDropdown = new(_graphics, 600, 800, Color.White) {
			BlinkOnMouseClick = false,
			BlinkOnMouseOver = false
		};
		beatmapDropdown.SetGradientAsColor(gradientListsList[1], 100);
		_visualsList.Add(beatmapDropdown);

		// 5
		Visuals beatmapDisplay = new(_graphics, 600, 100, Color.White) {
			BlinkOnMouseClick = true,
			BlinkOnMouseOver = true
		};
		_visualsList.Add(beatmapDisplay);

		// 6
		Visuals scoreDropdown = new(_graphics, 600, 800, Color.Transparent) {
			BlinkOnMouseClick = false,
			BlinkOnMouseOver = false
		};
		// scoreDropdown.SetGradientAsColor(gradientListsList[1], 100);
		_visualsList.Add(scoreDropdown);

		// 7
		Visuals scoreDisplay = new(_graphics, 600, 100, Color.White) {
			BlinkOnMouseClick = true,
			BlinkOnMouseOver = true
		};
		_visualsList.Add(scoreDisplay);

		// 8
		Visuals sessionDropdown = new(_graphics, 300, 800, Color.Transparent) {
			BlinkOnMouseClick = false,
			BlinkOnMouseOver = false
		};
		// scoreDropdown.SetGradientAsColor(gradientListsList[1], 100);
		_visualsList.Add(sessionDropdown);

		// 9
		Visuals sessionDisplay = new(_graphics, 300, 20, Color.Black) {
			BlinkOnMouseClick = true,
			BlinkOnMouseOver = true
		};
		_visualsList.Add(sessionDisplay);
	}

	public void Init() {
		List<GradientList> gradientListsList = new();

		SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());

		InitGradientListsList(gradientListsList);
		InitVisualsList(_visualsList, gradientListsList);

		_mainContainer = new UIElementContainer(_graphics, _graphics.Viewport.Bounds.Width, _graphics.Viewport.Bounds.Height, new Vector2(0, 0), "mainContainer", Color.Transparent);

		_sideMenu = new ScrollableUIElementContainer(_graphics, _graphics.Viewport.Bounds.Width - 1000, 1000, new(1000, 0), "sideMenu", new Color(100, 100, 100));
		_mainContainer.Add(_sideMenu);

		_sideMenu.Add(new ToggleButton(_graphics, new(0, 50 * 2 + 10 * 2), "Play", Play, Pause, _visualsList[0]));
		_sideMenu.Add(new Button(_graphics, new(200, 50 * 2 + 10 * 2), "NextPage", _visualsList[0]));
		_sideMenu.Add(new Button(_graphics, new(400, 50 * 2 + 10 * 2), "PreviousPage", _visualsList[0]));
		_sideMenu.Add(new Button(_graphics, new(600, 50 * 2 + 10 * 2), "DisplayScores", _visualsList[0]));
		if (_sideMenu["DisplayScores"] is Button displayScores)
			displayScores.SetOnClick(() => _replaySelector.Hide = !_replaySelector.Hide);

		if (_sideMenu["NextPage"] is Button button)
			button.SetOnClick(NextPage);
		if (_sideMenu["PreviousPage"] is Button button2)
			button2.SetOnClick(PreviousPage);

		_beatmapsDB = BeatmapDBParser.Parse("G:/Jeux/osssu/osu!.db");
		_scoresDB = ScoreDBParser.Parse("G:/Jeux/osssu/scores.db", _beatmapsDB);

		_replaySelector = new ReplaySelector(_graphics, new(0, 50 * 3 + 10 * 3), "replaySelector", 10, _visualsList[6], _visualsList[7]);
		_replaySelector.Hide = true;
		_mainContainer.Add(_replaySelector);

		// GradientList gdList = new();
		// gdList.Add(new Gradient(new Color(100, 0, 0, 1), new Color(0, 0, 100, 1)));
		// gdList.Add(new Gradient(new Color(0, 0, 100, 1), new Color(100, 0, 0, 1)));

		// _visualsList[5].InitGradientTexture(gdList, 100);

		_searchBar = new InputBox(_graphics, new (0, 0), "inputBox", _visualsList[0]);
		_searchBar.Visuals.Resize(600, _visualsList[0].Height);
		_sideMenu.Add(_searchBar);

		_query = "";
		_beatmapsList = _scoresDB.Beatmaps.Values.ToList();

		_sortedBeatmapsList = FilterSongs(_query);

		_beatmapSelector = new BeatmapSelector(_graphics, new(0, 50 * 3 + 10 * 3), "beatmapSelector", 10, _visualsList[4], _visualsList[5]);
		_beatmapSelector.SetColor(Color.Transparent);
		_sideMenu.Add(_beatmapSelector);
		_beatmapSelector.Init(_sortedBeatmapsList, 0, _replaySelector);

/* -------------------------------------------------------------------------- */
/*                                  Sessions                                  */
/* -------------------------------------------------------------------------- */


		DateTime oldest = DateTime.MaxValue;
		DateTime limit = new DateTime(2017, 1, 1);

		foreach (BeatmapWithScores beatmap2 in _scoresDB.Beatmaps.Values) {
			// Logger.LogDebug($"beatmap name: {beatmap2.BeatmapDBInfo.SongTitle}");
			foreach (ReplayData replay in beatmap2.Replays) {
				// Logger.LogDebug($"timestamp: {replay.TimeStamp}");
				if (!_sessions.ContainsKey(replay.TimeStamp.Date))
					_sessions[replay.TimeStamp.Date] = new();
				_sessions[replay.TimeStamp.Date].Replays.Add(replay);
				if (replay.TimeStamp < oldest && replay.TimeStamp > limit)
					oldest = replay.TimeStamp;
			}
		}

		foreach (Session session in _sessions.Values) {
			// session.Replays.Sort((a, b) => (int)(a.TimeStamp.ToBinary() - b.TimeStamp.ToBinary()));
			session.Replays = session.Replays.OrderBy(a => a.TimeStamp).ToList();
		}

		_sessionsList = _sessions.OrderBy(a => a.Key);

		Logger.LogDebug($"oldest timestamp: {oldest.Date}");
		Logger.LogDebug($"number of sessions: {_sessions.Count}");
		// List<ReplayData> replays = _sessions[new DateTime(2024, 03, 06)].Replays;

		// Logger.LogDebug($"number of replays: {replays.Count}");

		// foreach (ReplayData replay in replays) {
		// 	DateTime timestamp = replay.TimeStamp;
		// 	string songTitle = _scoresDB.Beatmaps[replay.BeatmapMD5].BeatmapDBInfo.SongTitle;
		// 	string difficulty = _scoresDB.Beatmaps[replay.BeatmapMD5].BeatmapDBInfo.Difficulty;
		// 	int score = replay.Score;
		// 	Logger.LogDebug($"timestamp: {timestamp} / map: {songTitle} [{difficulty}] / score: {score}");
		// }

		_sideMenu.Add(new ReplaySelector(_graphics, new(0, 2000), "SessionReplays", 10, _visualsList[6], _visualsList[7]));

		_sideMenu.Add(new Dropdown(_graphics, new(0, 1000), 0, "Sessions", _visualsList[8]));
		if (_sideMenu["Sessions"] is Dropdown sessionsButton) {
			for (int i = 0; i < _sessionsList.Count(); i++) {
			// for (int i = 0; i < 100; i++) {
				sessionsButton.Add(new Button(_graphics, new(0, 0), "Session" + i, _visualsList[9]));
				sessionsButton[i].Visuals.Resize(sessionsButton.UsableWidth, _visualsList[9].Height);
				sessionsButton[i].Visuals.Texts.Add(new Text(_sessionsList.ElementAt(i).Key.ToString(), new Vector2(0, 0)));

				int index = i;
				if (sessionsButton[i] is Button button1) {
					button1.SetOnClick(() => { UpdateSession(index); });
				}
			}
		}

		_sideMenu.Add(new Button(_graphics, new(0, 10000), "testaaaaaa", _visualsList[7]));
		_sideMenu.LastElement().Visuals.SetColor(Color.SeaGreen);

/* -------------------------------------------------------------------------- */
/*                                 Collections                                */
/* -------------------------------------------------------------------------- */

		// CollectionDB collectionDB = CollectionDBParser.Parse("G:/Jeux/osssu/collection.db");

		// for (int i = 0; i < collectionDB.CollectionCount; i++) {
		// 	Logger.LogDebug($"Collection name: {collectionDB.Collections[i].Name} ({collectionDB.Collections[i].BeatmapCount} maps)");
		// }

		// for (int i = 0; i < collectionDB.CollectionCount; i++) {
		// 	if (collectionDB.Collections[i].Name == "3* 4K 999k") {
		// 		collectionDB.Collections.RemoveAt(i);
		// 		collectionDB.CollectionCount--;
		// 		break;
		// 	}
		// }


		// List<string> list = _beatmapsDB.Beatmaps.Values.Where(beatmap =>
		// 	beatmap.Mode == GameMode.Mania
		// 	&& (int)beatmap.CircleSize == 4
		// 	&& beatmap.ManiaStarRating[(int)Mods.None] >= 1
		// 	&& beatmap.ManiaStarRating[(int)Mods.None] < 2
		// 	&& beatmap.RankedStatus == 4
		// 	&& (!_scoresDB.Beatmaps.ContainsKey(beatmap.BeatmapMD5)
		// 		|| _scoresDB.Beatmaps[beatmap.BeatmapMD5].Replays.Where(replay => replay.Score >= 999000).Count() == 0)
		// ).ToList().Select(beatmap => beatmap.BeatmapMD5).ToList();

		// List<string> list = _beatmapsDB.Beatmaps.Values.Where(beatmap =>
		// 	beatmap.Mode == GameMode.Mania
		// 	&& (int)beatmap.CircleSize == 4
		// 	&& beatmap.ManiaStarRating[(int)Mods.None] >= 4
		// 	// && beatmap.ManiaStarRating[(int)Mods.None] < 4
		// 	&& beatmap.RankedStatus == 4
		// 	&& _scoresDB.Beatmaps.ContainsKey(beatmap.BeatmapMD5)
		// 	&& _scoresDB.Beatmaps[beatmap.BeatmapMD5].Replays.Where(replay => replay.Score >= 995000).Count() > 0
		// ).ToList().Select(beatmap => beatmap.BeatmapMD5).ToList();

		// Collection testCollection = new();
		// testCollection.Name = "4*+ 4K 995k";
		// testCollection.BeatmapCount = list.Count;
		// testCollection.Beatmaps = list;

		// collectionDB.Collections.Add(testCollection);
		// collectionDB.CollectionCount++;

		// CollectionDBWriter.Write(collectionDB, "G:/Jeux/osssu/collection.db");

/* -------------------------------------------------------------------------- */
/*                                 Test Fonts                                 */
/* -------------------------------------------------------------------------- */

		_fontSystem = new FontSystem();
		_fontSystem.AddFont(File.ReadAllBytes(@"D:\Fonts\arial.ttf"));
		Fonts.Arial2 = _fontSystem.GetFont(20);

	}

	private void UpdateSession(int i) {
		if (_sideMenu["SessionReplays"] is ReplaySelector rs) {
			// dd.RemoveAll();

			rs.UpdateScores(_sessionsList.ElementAt(i).Value.Replays, _beatmapsDB);

			Logger.LogDebug($"Number of scores: {_sessionsList.ElementAt(i).Value.Replays.Count}");
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

		SpriteFontBase font30 = _fontSystem.GetFont(20);
		spriteBatch.DrawString(font30, "Jsp lol aled", new Vector2(0, 0), Color.Yellow);
	}
}

public class TextZone { // heriter de element ou de bouton pour la fonction ?
	public Vector2 RelativePos;
	public string Str;
	public Texture2D Texture;
	public SpriteFontBase Font;


}
