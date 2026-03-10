using Avalonia;
using Avalonia.Media;
using Rythmify.Core;
using Rythmify.Core.Beatmap;
using Rythmify.Core.Replay;

public class OsuReplayController {
	private OsuReplay _osuReplay = null;

	private readonly Rect _bounds;

	private ReplaySkinData _skin = null;
	private BeatmapWithScores _beatmap = null;
	private ReplayData _score = null;

	private bool _isPlaying = false;

	public OsuReplayController(Rect bounds) {
		_bounds = bounds;
	}

	public void ChangeSkin(ReplaySkinData skin) {
		_skin = skin;
		TryInstanciateReplay();
	}

	public void ChangeBeatmap(BeatmapWithScores beatmap) {
		_beatmap = beatmap;
		TryInstanciateReplay();
	}

	public void ChangeReplay(ReplayData score) {
		_score = score;
		TryInstanciateReplay();
	}

	private void TryInstanciateReplay() {
		// bool canBeInstanciated = _skin != null && _beatmap != null && _score != null && _beatmap.BeatmapDBInfo.BeatmapMD5 == _score.BeatmapMD5;
		bool canBeInstanciated = _skin != null && _beatmap != null && _score != null;
		if (canBeInstanciated) {
			_osuReplay?.Dispose();
			_osuReplay = new(_bounds, _skin, _beatmap, _score);
		}

		Logger.LogInfo($"[OsuReplayController] canBeInstanciated: {canBeInstanciated}");

		if (_isPlaying)
			Play();
	}

	public void Play() {
		// if (_osuReplay == null)
		// 	return;

		_osuReplay?.Play();
		_isPlaying = true;
	}

	public void Pause() {
		if (_osuReplay == null)
			return;

		_osuReplay.Pause();
		_isPlaying = false;
	}

	public void Update(double deltaTime, double speedMultiplier) => _osuReplay?.Update(deltaTime, speedMultiplier);

	public void Render(DrawingContext drawingContext) => _osuReplay?.Render(drawingContext);
}
