using System;
using System.ComponentModel;
using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mania2mp4.Models;
using Rythmify.Core;
using Rythmify.Core.Beatmap;
using Rythmify.Core.Replay;
using Rythmify.Dev;

namespace Mania2mp4.ViewModels;

public partial class OsuReplayViewModel : ViewModelBase {
	[ObservableProperty]
	private OsuReplay? _osuReplay;

	private OsuReplayModel _osuReplayModel;

	private SkinData? _skin => _osuReplayModel?.Skin;
	private BeatmapWithScores? _beatmap => _osuReplayModel?.Beatmap;
	private ReplayData? _score => _osuReplayModel?.Score;

	private bool _isPlaying = false;

	private readonly Rect _bounds = new Rect(0, 0, 1280, 720);

	private double _backgroundOpacity = 0.5f;

	public double BackgroundOpacity {
		get { return _backgroundOpacity; }
		set {
			_backgroundOpacity = value;
			OsuReplay?.ChangeBackgroundOpacity(_backgroundOpacity);
		}
	}

	private int _scrollSpeed = 28;

	public int ScrollSpeed {
		get { return _scrollSpeed; }
		set {
			_scrollSpeed = value;
			OsuReplay.ChangeScrollSpeed(_scrollSpeed);
		}
	}

	public OsuReplayViewModel(OsuReplayModel osuReplayModel) {
		_osuReplay = null;
		_osuReplayModel = osuReplayModel;
		_osuReplayModel.PropertyChanged += OsuReplayModelPropertyChanged;
	}

	private void OsuReplayModelPropertyChanged(object? sender, PropertyChangedEventArgs e) {
		if (e.PropertyName == nameof(_osuReplayModel.Beatmap))
			ChangeBeatmap(_osuReplayModel.Beatmap);
		if (e.PropertyName == nameof(_osuReplayModel.Score))
			ChangeScore(_osuReplayModel.Score);
		if (e.PropertyName == nameof(_osuReplayModel.Skin))
			ChangeSkin(_osuReplayModel.Skin);
	}

	public void ChangeSkin(SkinData skin) {
		if (OsuReplay != null)
			OsuReplay.UpdateSkin(new ReplaySkinData(_skin, (int)_beatmap.BeatmapDBInfo.CircleSize));
		else
			TryInstanciateReplay();
	}

	public void ChangeBeatmap(BeatmapWithScores beatmap) {
		if (beatmap.Beatmap == null)
			beatmap.LoadBeatmap();
		_osuReplayModel.Beatmap = beatmap;
		TryInstanciateReplay();
	}

	public void ChangeScore(ReplayData score) {
		if (score == null) return;
		if (score.Inputs == null) {
			try {
				_osuReplayModel.Score = ReplayParser.Parse(score.FilePath, 4, false);
			} catch (Exception e) {
				Logger.LogDebug($"No replay for this score: {e.Message}. Stacktrace:\n{e.StackTrace}");
				_osuReplayModel.Score = null;
				return;
			}
		}
		TryInstanciateReplay();
	}

	private void TryInstanciateReplay() {
		// bool canBeInstanciated = _skin != null && _beatmap != null && _score != null && _beatmap.BeatmapDBInfo.BeatmapMD5 == _score.BeatmapMD5;
		bool canBeInstanciated = _skin != null && _beatmap != null && _beatmap.Beatmap != null && _score != null;
		if (canBeInstanciated) {
			OsuReplay?.Pause();
			OsuReplay?.Dispose();
			OsuReplay = null;
			try {
				OsuReplay = new(_bounds, new ReplaySkinData(_skin, (int)_beatmap.BeatmapDBInfo.CircleSize), _beatmap, _score);
				OsuReplay.ChangeBackgroundOpacity(BackgroundOpacity);
				OsuReplay.ChangeScrollSpeed(ScrollSpeed);
			} catch (Exception e) {
				Logger.LogDebug($"Couldn't create replay: {e.Message}\nStacktrace: {e.StackTrace}");
				return;
			}
		}

		Logger.LogDebug($"canBeInstanciated: {canBeInstanciated}");

		if (_isPlaying)
			OsuReplay?.Play();
	}

	[RelayCommand]
	public void PlayPause() {
		_isPlaying = !_isPlaying;

		if (_isPlaying)
			OsuReplay?.Play();
		else
			OsuReplay?.Pause();
		// Logger.LogDebug("playing");
	}
}
