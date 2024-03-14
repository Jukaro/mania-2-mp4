using System.IO;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NAudio.Wave;
using Rythmify.Core;
using Rythmify.Core.Beatmap;
using Rythmify.Core.Game;

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

		// var filePath = "C:/Users/shiro/AppData/Local/osu!/Songs/400078 Kurokotei - Galaxy Collapse/Kurokotei - Galaxy Collapse (Mat) [Cataclysmic Hypernova].osu";
		// var filePath = "C:/Users/shiro/AppData/Local/osu!/Songs/24313 Team Nekokan - Can't Defeat Airman/Team Nekokan - Can't Defeat Airman (Blue Dragon) [Holy Shit! It's Airman!!].osu";
		// var filePath = "C:/Users/shiro/AppData/Local/osu!/Songs/574811 odaxelagnia - The Shortest Mashcore Ever/odaxelagnia - The Shortest Mashcore Ever (Hydria) [Insane].osu";
		var filePath = "C:/Users/shiro/AppData/Local/osu!/Songs/1981845 Gram - Odin/Gram - Odin (cai_ji_ccc) [GOD].osu";
		// var filePath = "C:/Users/shiro/AppData/Local/osu!/Songs/1147471 toby fox - sans/toby fox - sans. (Leniane) [mimi's easy].osu";
		// var filePath = "C:/Users/shiro/AppData/Local/osu!/Songs/1035988 The Koxx - A FOOL MOON NIGHT/The Koxx - A FOOL MOON NIGHT (motoroko) [A Fool].osu";

		var beatmap = BeatmapParser.Parse(filePath);
		Logger.LogDebug(Path.Combine(Path.GetDirectoryName(filePath), beatmap.GeneralData.AudioFilename));
		_song = new AudioFileReader(Path.Combine(Path.GetDirectoryName(filePath), beatmap.GeneralData.AudioFilename));

		Skin skin = new() { HitPosition = 384 };

		_beatmapPlayer = new(beatmap, skin);

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
