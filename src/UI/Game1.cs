using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Rythmify.Core.Beatmap;
using Rythmify.Core.Game;

namespace Rythmify.UI;

public class Game1 : Game
{
	private GraphicsDeviceManager _graphics;
	private SpriteBatch _spriteBatch;
	private ReplayPlayer ReplayPlayer;

	private Texture2D NoteTexture;
	private Texture2D HitLineTexture;

	public Game1()
	{
		_graphics = new GraphicsDeviceManager(this);
		Content.RootDirectory = "Content";
		IsMouseVisible = true;

		// var filePath = "C:/Users/shiro/AppData/Local/osu!/Songs/400078 Kurokotei - Galaxy Collapse/Kurokotei - Galaxy Collapse (Mat) [Cataclysmic Hypernova].osu";
		// var filePath = "C:/Users/shiro/AppData/Local/osu!/Songs/24313 Team Nekokan - Can't Defeat Airman/Team Nekokan - Can't Defeat Airman (Blue Dragon) [Holy Shit! It's Airman!!].osu";
		var filePath = "C:/Users/shiro/AppData/Local/osu!/Songs/574811 odaxelagnia - The Shortest Mashcore Ever/odaxelagnia - The Shortest Mashcore Ever (Hydria) [Insane].osu";
		var beatmap = BeatmapParser.Parse(filePath);
		Logger.LogSuccess(beatmap.ToString());
		ReplayPlayer = new(beatmap);
	}

	protected override void Initialize()
	{
		_graphics.PreferredBackBufferWidth = 1920;
		_graphics.PreferredBackBufferHeight = 1080;
		_graphics.ApplyChanges();

		ReplayPlayer.Play();
		base.Initialize();
	}

	protected override void LoadContent()
	{
		_spriteBatch = new SpriteBatch(GraphicsDevice);

		NoteTexture = new(GraphicsDevice, 150, 150);
		var colors = new Color[150 * 150];
		Array.Fill(colors, Color.White);
		NoteTexture.SetData(colors);

		HitLineTexture = new(GraphicsDevice, _graphics.PreferredBackBufferWidth, 5);
		var colors1 = new Color[_graphics.PreferredBackBufferWidth * 5];
		Array.Fill(colors1, Color.Red);
		HitLineTexture.SetData(colors1);
	}

	protected override void Update(GameTime gameTime)
	{
		if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
			Exit();

		ReplayPlayer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);

		base.Update(gameTime);
	}

	protected override void Draw(GameTime gameTime)
	{
		GraphicsDevice.Clear(Color.Black);

		_spriteBatch.Begin();
		foreach (var note in ReplayPlayer.RenderedNotes)
		{
			_spriteBatch.Draw(NoteTexture, new Vector2(note.Lane * NoteTexture.Width + 30 * note.Lane, (int)(1080 * note.Y / 480f)), Color.White);
		}
		var hitPosition = 384;
		var screenSpaceHitPosition = hitPosition * _graphics.PreferredBackBufferHeight / 480;
		_spriteBatch.Draw(HitLineTexture, new Vector2(0, screenSpaceHitPosition), Color.White);
		_spriteBatch.End();

		base.Draw(gameTime);
	}
}
