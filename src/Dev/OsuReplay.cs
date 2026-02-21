using System;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Rythmify.Core;
using Rythmify.Core.Beatmap;
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

	private Rect _bounds;
	private Bitmap _background;

	private double _backgroundOpacity = 0.5f;

	public OsuReplay(Rect bounds, ReplaySkinData skin, BeatmapWithScores beatmap, ReplayData replay) {
		if (beatmap.Beatmap == null)
			throw new ArgumentException("BeatmapData is null");

		_skinRenderer = new(skin);
		_beatmapRenderer = new(_skinRenderer, bounds);
		_inputsRenderer = new(_skinRenderer, bounds);

		_bounds = bounds;
		_background = BeatmapDisplayHelper.GetBackground(beatmap);
		if (_background == null)
			_background = BeatmapDisplayHelper.GetSeasonalBackground();

		_beatmapPlayer = new(beatmap.Beatmap, _skinRenderer.GetSkin().ManiaSection.HitPosition);
		_audioPlayer = new(beatmap.AudioPath);
		_inputsPlayer = new(replay, beatmap.Beatmap.GeneralData.AudioLeadIn);
	}

	public void UpdateSkin(ReplaySkinData skin) {
		_skinRenderer = new SkinRenderer(skin);
		_beatmapRenderer.UpdateSkinRenderer(_skinRenderer);
		_inputsRenderer.UpdateSkinRenderer(_skinRenderer);
	}

	public void ChangeBackgroundOpacity(double opacity) {
		_backgroundOpacity = opacity;
	}

	public void ChangeScrollSpeed(int scrollSpeed) {
		_beatmapPlayer.SetScrollSpeed(scrollSpeed);
	}

	public void Dispose() {
		_audioPlayer.Dispose();
		_background.Dispose();
	}

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

	public void Render(DrawingContext drawingContext) {
		using (drawingContext.PushOpacity(_backgroundOpacity))
			drawingContext.DrawImage(_background, new Rect(0, 0, _background.Size.Width * (_bounds.Height / _background.Size.Height), _bounds.Height));
		_beatmapRenderer.Render(_beatmapPlayer, drawingContext);
		_inputsRenderer.Render(_inputsPlayer, drawingContext);
	}
}
