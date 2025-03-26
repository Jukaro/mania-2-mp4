using System;
using NAudio.Wave;
using Rythmify.Core;

namespace Rythmify.UI;

public class AudioPlayer {
	private WaveOutEvent _outputDevice;
	private AudioReader _song;
	private string _songPath;
	private bool _needToPlayAudio;

	public AudioPlayer(string songPath) {
		_outputDevice = new WaveOutEvent();
		_songPath = songPath;
		_song = new(_songPath);
		_outputDevice.Init(_song.GetSampleProvider());
		_outputDevice.Volume = 0.01f;
		_needToPlayAudio = false;
	}

	public void Dispose() => _outputDevice.Dispose();

	public void Pause() {
		_outputDevice.Pause();
		_needToPlayAudio = false;
	}

	public void Play() {
		_needToPlayAudio = true;
	}

	public void Reset() {
		_outputDevice.Stop();
		_song = new(_songPath);
		_outputDevice.Init(_song.GetSampleProvider());
	}

	public void Update() {
		if (_needToPlayAudio && _outputDevice.PlaybackState != PlaybackState.Playing)
			_outputDevice.Play();
	}

	public void VolumeUp() {
		_outputDevice.Volume = Math.Clamp(_outputDevice.Volume + 0.01f, 0, 1);
	}

	public void VolumeDown() {
		_outputDevice.Volume = Math.Clamp(_outputDevice.Volume - 0.01f, 0, 1);
	}
}
