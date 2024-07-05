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

	public void Init(GraphicsDeviceManager graphics, GraphicsDevice graphicsDevice)
	{
		// dynamic testCase = Datasets.TestCases.ShiroW.Stronger;
		// dynamic testCase = Datasets.TestCases.Jukaro.Polyriddim;
		testCase = Datasets.TestCases.Jukaro.Polyriddim;

		_beatmap = BeatmapParser.Parse(testCase.BeatmapPath);

		_skin = new() { HitPosition = 384 };

		_replay = ReplayParser.Parse(testCase.ReplayPath, _beatmap.DifficultyData.LaneCount);

		_beatmapPlayer = new(_beatmap, _skin, _replay);
		_inputsPlayer = new(_replay);

		SkinRenderer skinRenderer = new(_skin, graphicsDevice);
		_beatmapRenderer = new(graphics, skinRenderer);
		_inputsRenderer = new(graphics, skinRenderer);

		string songPath = Path.Combine(Path.GetDirectoryName(testCase.BeatmapPath), _beatmap.GeneralData.AudioFilename);
		_audioPlayer = new(songPath);

		_menu = new(graphicsDevice);
		_menu.Init();
	}

	public void Update(GameTime gameTime)
	{
		_menu.Update(_beatmapPlayer, _inputsPlayer, _audioPlayer, _replay);

		_beatmapPlayer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);
		_inputsPlayer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);
		_audioPlayer.Update(_beatmapPlayer.AudioStarted);
	}

	public void Render(SpriteBatch spriteBatch)
	{
		_menu.レンダー(spriteBatch);
		_beatmapRenderer.Render(_beatmapPlayer, spriteBatch);
		_inputsRenderer.Render(_inputsPlayer, spriteBatch);
	}
}
