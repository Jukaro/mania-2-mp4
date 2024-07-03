using System.IO;
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

public class OsuReplay
{
	private BeatmapPlayer _beatmapPlayer;
	private BeatmapRenderer _beatmapRenderer;
	private InputsPlayer _inputsPlayer;
	private InputsRenderer _inputsRenderer;

	private AudioFileReader _song;

	public void LoadContent(GraphicsDeviceManager graphics, GraphicsDevice graphicsDevice, WaveOutEvent outputDevice)
	{
		// dynamic testCase = Datasets.TestCases.ShiroW.Stronger;
		dynamic testCase = Datasets.TestCases.Jukaro.Polyriddim;

		BeatmapData beatmap = BeatmapParser.Parse(testCase.BeatmapPath);
		_song = new AudioFileReader(Path.Combine(Path.GetDirectoryName(testCase.BeatmapPath), beatmap.GeneralData.AudioFilename));

		Skin skin = new() { HitPosition = 384 };

		ReplayData replay = ReplayParser.Parse(testCase.ReplayPath, beatmap.DifficultyData.LaneCount);

		_beatmapPlayer = new(beatmap, skin, replay);
		_inputsPlayer = new(replay);

		SkinRenderer skinRenderer = new(skin, graphicsDevice);
		_beatmapRenderer = new(graphics, skinRenderer);
		_inputsRenderer = new(graphics, skinRenderer);

		outputDevice.Init(_song);
		outputDevice.Volume = 0.01f;

		_beatmapPlayer.Play();
		_inputsPlayer.Play();
	}

	public void Update(GameTime gameTime, WaveOutEvent outputDevice)
	{
		if (_beatmapPlayer.AudioStarted && outputDevice.PlaybackState != PlaybackState.Playing)
			outputDevice.Play();

		_beatmapPlayer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);
		_inputsPlayer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);
	}

	public void Render(SpriteBatch spriteBatch)
	{
		_beatmapRenderer.Render(_beatmapPlayer, spriteBatch);
		_inputsRenderer.Render(_inputsPlayer, spriteBatch);
	}
}
