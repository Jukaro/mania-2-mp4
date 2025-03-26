using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rythmify.Core;
using Rythmify.Core.Replay;

public class OsuReplayController {
	private OsuReplay _osuReplay = null;

	private GraphicsDevice _graphicsDevice;
	private GraphicsDeviceManager _graphicsDeviceManager;

	private ReplaySkinData _skin = null;
	private BeatmapWithScores _beatmap = null;
	private ReplayData _score = null;

	private bool _isPlaying = false;

	public OsuReplayController(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphicsDeviceManager) {
		_graphicsDevice = graphicsDevice;
		_graphicsDeviceManager = graphicsDeviceManager;
	}

	public void ChangeSkin(ReplaySkinData skin) {
		_skin = skin;
	}

	public void ChangeBeatmap(BeatmapWithScores beatmap) {
		_beatmap = beatmap;
	}

	public void ChangeReplay(ReplayData score) {
		_score = score;

		bool canBeInstanciated = _skin != null && _beatmap != null && _score != null;
		if (canBeInstanciated) {
			_osuReplay?.Dispose();
			_osuReplay = new(_graphicsDeviceManager, _graphicsDevice, _skin, _beatmap, _score);
		}

		if (_isPlaying)
			Play();
	}

	public void Play() {
		if (_osuReplay == null)
			return;

		_osuReplay.Play();
		_isPlaying = true;
	}

	public void Pause() {
		if (_osuReplay == null)
			return;

		_osuReplay.Pause();
		_isPlaying = false;
	}

	public void Update(double deltaTime, double speedMultiplier) => _osuReplay?.Update(deltaTime, speedMultiplier);

	public void Render(SpriteBatch spriteBatch) => _osuReplay?.Render(spriteBatch);
}
