using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Rythmify.Core;
using Rythmify.Core.Beatmap;
using Rythmify.Core.Game;
using Rythmify.Core.Replay;
using Rythmify.Dev;

namespace Rythmify.UI;

public class OsuReplay
{
	private BeatmapPlayer _beatmapPlayer;
	private BeatmapRenderer _beatmapRenderer;
	private InputsPlayer _inputsPlayer;
	private InputsRenderer _inputsRenderer;
	private AudioPlayer _audioPlayer;
	private Menu _menu;

	private BeatmapData _beatmap;
	private Skin _skin;
	private ReplayData _replay;

	private dynamic testCase;

	private Button _test;

	private double _speedMultiplier = 1.0f;

	public void Init(GraphicsDeviceManager graphics, GraphicsDevice graphicsDevice)
	{
		dynamic testCase = Datasets.TestCases.ShiroW.Stronger;
		// dynamic testCase = Datasets.TestCases.Jukaro.Polyriddim;
		// testCase = Datasets.TestCases.Jukaro.Polyriddim;

		_beatmap = BeatmapParser.Parse(testCase.BeatmapPath);

		_skin = new() { HitPosition = 384 };

		_replay = ReplayParser.Parse(testCase.ReplayPath, _beatmap.DifficultyData.LaneCount, false);

		_beatmapPlayer = new(_beatmap, _skin);
		_inputsPlayer = new(_replay);

		SkinRenderer skinRenderer = new(_skin, graphicsDevice);
		_beatmapRenderer = new(graphics, skinRenderer);
		_inputsRenderer = new(graphics, skinRenderer);

		string songPath = Path.Combine(Path.GetDirectoryName(testCase.BeatmapPath), _beatmap.GeneralData.AudioFilename);
		_audioPlayer = new(songPath);

		_menu = new(graphicsDevice);
		_menu.Init();

		Visuals visuals = new(graphicsDevice, 300, 50, Color.Red) {
			BlinkOnMouseClick = true,
			BlinkOnMouseOver = true
		};

		_test = new SliderButton(graphicsDevice, new(700, 950), "Slider", 0, 10, 10, UpdateSpeedMultiplier, visuals);
	}

	public void Update(GameTime gameTime)
	{
		_beatmapPlayer.Update(gameTime.ElapsedGameTime.TotalMilliseconds * _speedMultiplier);
		_inputsPlayer.Update(gameTime.ElapsedGameTime.TotalMilliseconds * _speedMultiplier);
		_audioPlayer.Update(_beatmapPlayer.AudioStarted);

		_menu.Update(ref _beatmapPlayer, ref _inputsPlayer, ref _audioPlayer, _replay, _skin);
		_test.Update();
	}

	public void Render(SpriteBatch spriteBatch)
	{
		_beatmapRenderer.Render(_beatmapPlayer, spriteBatch);
		_inputsRenderer.Render(_inputsPlayer, spriteBatch);

		_menu.Render(spriteBatch);
		_test.Render(spriteBatch);
	}

	private void UpdateSpeedMultiplier(double value) {
		_speedMultiplier = value;
	}
}
