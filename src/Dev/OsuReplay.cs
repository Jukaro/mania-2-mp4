using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rythmify.Core;
using Rythmify.Core.Game;
using Rythmify.Core.Replay;
using Rythmify.UI;

public class OsuReplay {
	private BeatmapPlayer _beatmapPlayer = null;
	private InputsPlayer _inputsPlayer = null;
	private AudioPlayer _audioPlayer = null;

	private BeatmapRenderer _beatmapRenderer;
	private InputsRenderer _inputsRenderer;
	private SkinRenderer _skinRenderer;

	public OsuReplay(GraphicsDeviceManager graphicsDeviceManager, GraphicsDevice graphicsDevice, ReplaySkinData skin, BeatmapWithScores beatmap, ReplayData replay) {
		_skinRenderer = new(skin, graphicsDevice);
		_beatmapRenderer = new(graphicsDeviceManager, _skinRenderer);
		_inputsRenderer = new(graphicsDeviceManager, _skinRenderer);

		_beatmapPlayer = new(beatmap.Beatmap, _skinRenderer.GetSkin().ManiaSection.HitPosition);
		_audioPlayer = new(beatmap.AudioPath);
		_inputsPlayer = new(replay);
	}

	public void Dispose() => _audioPlayer.Dispose();

	public void Play() {
		_beatmapPlayer.Play();
		_inputsPlayer.Play();
		_audioPlayer.Play();
	}

	public void Pause() {
		_beatmapPlayer.Pause();
		_inputsPlayer.Pause();
		_audioPlayer.Pause();
	}

	public void Update(double deltaTime, double speedMultiplier) {
		_beatmapPlayer.Update(deltaTime * speedMultiplier);
		_inputsPlayer.Update(deltaTime * speedMultiplier);
		_audioPlayer.Update();
	}

	public void Render(SpriteBatch spriteBatch) {
		_beatmapRenderer.Render(_beatmapPlayer, spriteBatch);
		_inputsRenderer.Render(_inputsPlayer, spriteBatch);
	}
}
