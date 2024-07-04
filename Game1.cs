using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NAudio.Wave;

namespace Rythmify.UI;

public class Game1 : Game
{
	private readonly GraphicsDeviceManager _graphics;
	private SpriteBatch _spriteBatch;

	private OsuReplay _osuReplay;

	public Game1()
	{
		_graphics = new GraphicsDeviceManager(this);
		Content.RootDirectory = "Content";
		IsMouseVisible = true;
		_osuReplay = new OsuReplay();
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

		_osuReplay.Init(_graphics, GraphicsDevice);
	}

	protected override void Update(GameTime gameTime)
	{
		if (Keyboard.GetState().IsKeyDown(Keys.Escape))
			Exit();

		_osuReplay.Update(gameTime);

		base.Update(gameTime);
	}

	protected override void Draw(GameTime gameTime)
	{
		GraphicsDevice.Clear(Color.Black);

		_spriteBatch.Begin();
		_osuReplay.Render(_spriteBatch);
		_spriteBatch.End();

		base.Draw(gameTime);
	}
}
