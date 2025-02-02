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
	private SkinData _rawSkin;
	private ReplaySkinData _replaySkin;
	private ReplayData _replay;

	private Button _test;

	private double _speedMultiplier = 1.0f;

	public void Init(GraphicsDeviceManager graphics, GraphicsDevice graphicsDevice)
	{
		// dynamic testCase = Datasets.TestCases.Jukaro.Stronger;
		dynamic skinPath = Datasets.TestCases.ShiroW.Skins.KizunaAkari;
		dynamic testCase = Datasets.TestCases.ShiroW.Polyriddim;
		// testCase = Datasets.TestCases.Jukaro.Polyriddim;

		_beatmap = BeatmapParser.Parse(testCase.BeatmapPath);

		_rawSkin = SkinParser.Parse(skinPath);
		_replaySkin = new ReplaySkinData(_rawSkin, _beatmap.DifficultyData.LaneCount);

		_replay = ReplayParser.Parse(testCase.ReplayPath, _beatmap.DifficultyData.LaneCount, false);

		_beatmapPlayer = new(_beatmap, _replaySkin.ManiaSection.HitPosition);
		_inputsPlayer = new(_replay);

		SkinRenderer skinRenderer = new(_replaySkin, graphicsDevice);
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

		_menu.Update(ref _beatmapPlayer, ref _inputsPlayer, ref _audioPlayer, _replay, _replaySkin);
		_test.Update();
	}

	public void Render(SpriteBatch spriteBatch)
	{
		_beatmapRenderer.Render(_beatmapPlayer, spriteBatch);
		_inputsRenderer.Render(_inputsPlayer, spriteBatch);

		_menu.Render(spriteBatch);
		spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
		_test.Render(spriteBatch);
		spriteBatch.End();
	}

	private void UpdateSpeedMultiplier(double value) {
		_speedMultiplier = value;
	}
}
