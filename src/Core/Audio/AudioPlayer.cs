using System;
using System.IO;
using System.Linq;
using NAudio.Mixer;
using NAudio.Wave;
using Rythmify.Core;
using SoundFlow.Abstracts.Devices;
using SoundFlow.Backends.MiniAudio;
using SoundFlow.Codecs.FFMpeg;
using SoundFlow.Components;
using SoundFlow.Enums;
using SoundFlow.Metadata;
using SoundFlow.Metadata.Models;
using SoundFlow.Providers;
using SoundFlow.Structs;

namespace Rythmify.UI;

public class AudioPlayer {
	private string _songPath;
	private bool _needToPlayAudio;

	private MiniAudioEngine _engine = new();
	private FFmpegCodecFactory _ffmpegCodecFactory = new();
	private AudioFormat _audioFormat;
	private StreamDataProvider _streamDataProvider;
	private AudioPlaybackDevice _playbackDevice;
	private SoundPlayer _player;

	public AudioPlayer(string songPath) {
		// _engine.RegisterCodecFactory(_ffmpegCodecFactory);

		_audioFormat = AudioFormat.DvdHq;
		_engine.UpdateAudioDevicesInfo();

		var defaultDevice = _engine.PlaybackDevices.FirstOrDefault(x => x.IsDefault);
    	_playbackDevice = _engine.InitializePlaybackDevice(defaultDevice, _audioFormat);

		_songPath = songPath;

		_streamDataProvider = new StreamDataProvider(_engine, _audioFormat, File.OpenRead(_songPath));

		_player = new SoundPlayer(_engine, _audioFormat, _streamDataProvider);
		_playbackDevice.MasterMixer.AddComponent(_player);

		_playbackDevice.Start();

		_player.Volume = 0.5f;
	}

	public void Dispose() {
		_playbackDevice.Stop();
		_playbackDevice.Dispose();
		_streamDataProvider.Dispose();
		_engine.Dispose();
	}

	public void Pause() {
		_player.Pause();
		// _outputDevice.Pause();
		_needToPlayAudio = false;
	}

	public void Play() {
		_needToPlayAudio = true;
	}

	public void Reset() {
		// _outputDevice.Stop();
		// _song = new(_songPath);
		// _outputDevice.Init(_song.GetSampleProvider());
		_player.Stop();
		_streamDataProvider = new StreamDataProvider(_engine, _audioFormat, File.OpenRead(_songPath));
	}

	public void Update() {
		if (_needToPlayAudio && _player.State != SoundFlow.Enums.PlaybackState.Playing) {
			_player.Play();
			Logger.LogDebug("is playing");
		}
	}

	public void VolumeUp() {
		// _outputDevice.Volume = Math.Clamp(_outputDevice.Volume + 0.01f, 0, 1);
	}

	public void VolumeDown() {
		// _outputDevice.Volume = Math.Clamp(_outputDevice.Volume - 0.01f, 0, 1);
	}
}
