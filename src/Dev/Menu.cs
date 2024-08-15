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

namespace Rythmify.UI;

public class Menu {
	private readonly GraphicsDevice _graphics;
	private KeyboardKey _volumeUp;
	private KeyboardKey _volumeDown;

	private KeyboardKey _numPad8;

	private KeyboardKey _F3;
	private KeyboardKey _F4;

	private UIElementContainer _mainContainer;
	private UIElementContainer _sideMenu;
	private ReplaySelector _replaySelector;
	private BeatmapSelector _beatmapSelector;
	private InputBox _searchBar;
	private string _query;

	private bool _play;
	private bool _isPlaying;

	private Texture2D _testBG;
	private Texture2D _testBG2;

	private int _beatmapsPageIndex = 0;
	private BeatmapDB _beatmapsDB;
	private ScoreDB _scoresDB;
	private List<BeatmapWithScores> _beatmapsList;
	private List<BeatmapWithScores> _sortedBeatmapsList;

	private List<Visuals> _visualsList = new();

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
		Visuals beatmapDisplay = new(_graphics, 600, 100, Color.DarkGray) {
			BlinkOnMouseClick = true,
			BlinkOnMouseOver = true
		};
		_visualsList.Add(beatmapDisplay);

		// 6
		Visuals scoreDropdown = new(_graphics, 600, 800, Color.White) {
			BlinkOnMouseClick = false,
			BlinkOnMouseOver = false
		};
		scoreDropdown.SetGradientAsColor(gradientListsList[1], 100);
		_visualsList.Add(scoreDropdown);

		// 7
		Visuals scoreDisplay = new(_graphics, 600, 100, Color.DarkGray) {
			BlinkOnMouseClick = true,
			BlinkOnMouseOver = true
		};
		_visualsList.Add(scoreDisplay);
	}

	public void Init() {
		List<GradientList> gradientListsList = new();

		SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
		// Logger.LogDebug($"Sync context: {SynchronizationContext.Current}");

		InitGradientListsList(gradientListsList);
		InitVisualsList(_visualsList, gradientListsList);

		_mainContainer = new UIElementContainer(_graphics, _graphics.Viewport.Bounds.Width, _graphics.Viewport.Bounds.Height, new Vector2(0, 0), "mainContainer", Color.Transparent);

		_sideMenu = new UIElementContainer(_graphics, _graphics.Viewport.Bounds.Width - 1000, 1000, new(1000, 0), "sideMenu", new Color(100, 100, 100));
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

		_beatmapsDB = BeatmapDBParser.Parse("C:/Users/shiro/AppData/Local/osu!/osu!.db");
		_scoresDB = ScoreDBParser.Parse("C:/Users/shiro/AppData/Local/osu!/scores.db", _beatmapsDB);

		_replaySelector = new ReplaySelector(_graphics, new(0, 50 * 3 + 10 * 3), "replaySelector", 10, _visualsList[6], _visualsList[7]);
		_replaySelector.SetColor(Color.Transparent);
		_replaySelector.Hide = true;
		_mainContainer.Add(_replaySelector);

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

		// string texturePath2 = "C:/Users/shiro/AppData/Local/osu!/Songs/2112649 Camellia - Kisaragi/61163969_p0.jpg";
		// _testBG = Texture2D.FromFile(_graphics, texturePath2);
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

	public void アップデート(ref BeatmapPlayer beatmapPlayer, ref InputsPlayer inputsPlayer, ref AudioPlayer audioPlayer, ReplayData replay, Skin skin) {
		MouseManager.Update();

		if (_replaySelector.NeedToUpdatePlayers) {

			if (_beatmapSelector.SelectedBeatmap != null && _replaySelector.SelectedReplay != null) {
				beatmapPlayer = new BeatmapPlayer(_beatmapSelector.SelectedBeatmap.Beatmap, skin);
				inputsPlayer = new InputsPlayer(_replaySelector.SelectedReplay);
				Logger.LogDebug($"AudioPath: {_beatmapSelector.SelectedBeatmap.AudioPath}");
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

	public void レンダー(SpriteBatch spriteBatch) {
		_mainContainer.Render(spriteBatch);
	}
}
