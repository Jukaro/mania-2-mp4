using System.IO;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NAudio.Wave;
using Rythmify.Core;
using Rythmify.Core.Beatmap;
using Rythmify.Core.Game;
using Rythmify.Core.Replay;
using Rythmify.Dev;

namespace Rythmify.UI;

public class Game1 : Game
{
	private GraphicsDeviceManager _graphics;
	private SpriteBatch _spriteBatch;
	private BeatmapPlayer _beatmapPlayer;
	private BeatmapRenderer _beatmapRenderer;

	private AudioFileReader _song;
	private WaveOutEvent _outputDevice;
	private Thread _audioThread;

	public Game1()
	{
		_graphics = new GraphicsDeviceManager(this);
		Content.RootDirectory = "Content";
		IsMouseVisible = true;
		_outputDevice = new WaveOutEvent();
	}

	protected override void Initialize()
	{
		_graphics.PreferredBackBufferWidth = (int)(1000 * (16/9f));
		_graphics.PreferredBackBufferHeight = 1000;
		_graphics.SynchronizeWithVerticalRetrace = false;
		_graphics.ApplyChanges();

		// Set update rate to monitor refresh rate
		IsFixedTimeStep = false;

		base.Initialize();
	}

	protected override void LoadContent()
	{
		_spriteBatch = new SpriteBatch(GraphicsDevice);


		dynamic testCase = Datasets.TestCases.ShiroW.Algorithm;

		var beatmap = BeatmapParser.Parse(testCase.BeatmapPath);
		_song = new AudioFileReader(Path.Combine(Path.GetDirectoryName(testCase.BeatmapPath), beatmap.GeneralData.AudioFilename));

		Skin skin = new() { HitPosition = 384 };

		ReplayData replay = ReplayParser.Parse(testCase.ReplayPath, beatmap.DifficultyData.LaneCount);

		_beatmapPlayer = new(beatmap, replay, skin);

		SkinRenderer skinRenderer = new(skin, GraphicsDevice);
		_beatmapRenderer = new(_graphics, skinRenderer);

		_outputDevice.Init(_song);

		_audioThread = new Thread(() => _outputDevice.Play());

		_outputDevice.Volume = 0.1f;
		_beatmapPlayer.Play();
		_audioThread.Start();
	}

	protected override void Update(GameTime gameTime)
	{
		if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
			Exit();

		_beatmapPlayer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);

		base.Update(gameTime);
	}

	protected override void Draw(GameTime gameTime)
	{
		GraphicsDevice.Clear(Color.Black);

		_spriteBatch.Begin();
		_beatmapRenderer.Render(_beatmapPlayer, _spriteBatch);
		_spriteBatch.End();

		base.Draw(gameTime);
	}
}
