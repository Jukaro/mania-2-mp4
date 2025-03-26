using System;
using System.IO;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Rythmify.Core;

namespace Rythmify.UI;

public static class Paths {
	public static string OsuDirectoryPath = "G:/Jeux/osssu";
	public static string OsuSongsDirectoryPath = "G:/Jeux/osu maps de giga ultra mort";
}

public class Game1 : Game
{
	private readonly GraphicsDeviceManager _graphics;
	private SpriteBatch _spriteBatch;

	private Menu _menu;

	public Game1()
	{
		_graphics = new GraphicsDeviceManager(this);
		Content.RootDirectory = "Content";
		IsMouseVisible = true;
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

		FontsStore.Arial = Content.Load<SpriteFont>("Fonts/Arial");
		_menu = new Menu(GraphicsDevice, _graphics);
		_menu.Init();
	}

	protected override void Update(GameTime gameTime)
	{
		if (Keyboard.GetState().IsKeyDown(Keys.Escape))
			Exit();

		TaskManager.Update();
		_menu.Update(gameTime.ElapsedGameTime.TotalMilliseconds);

		base.Update(gameTime);
	}

	protected override void Draw(GameTime gameTime)
	{
		GraphicsDevice.Clear(Color.Black);

		_spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
		_menu.Render(_spriteBatch);
		_spriteBatch.End();

		base.Draw(gameTime);
	}

	protected override void OnExiting(object sender, EventArgs args)
	{
		ManagedGlobalHook.Instance.Hook.Dispose();
		base.OnExiting(sender, args);
	}
}
