using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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

	private static MiniAudioEngine _engine = new();
	private static FFmpegCodecFactory _ffmpegCodecFactory = new();
	private AudioFormat _audioFormat;
	private StreamDataProvider? _streamDataProvider;
	private AudioPlaybackDevice _playbackDevice;
	private SoundPlayer _player;

	static AudioPlayer() {
		// unregister default codec factory because it's too strict with audio files headers
		_engine.UnregisterCodecFactory("SoundFlow.MiniAudio.Default");

		_engine.RegisterCodecFactory(_ffmpegCodecFactory);
		_engine.UpdateAudioDevicesInfo();
	}

	public AudioPlayer(string songPath) {
		var defaultDevice = _engine.PlaybackDevices.FirstOrDefault(x => x.IsDefault);

		_songPath = songPath;
		Task.Run(() => _audioFormat = GetAudioFormat(songPath)).Wait();
		_playbackDevice = _engine.InitializePlaybackDevice(defaultDevice, _audioFormat);
		Task.Run(() => _streamDataProvider = new StreamDataProvider(_engine, File.OpenRead(_songPath))).Wait();

		_player = new SoundPlayer(_engine, _audioFormat, _streamDataProvider);
		_playbackDevice.MasterMixer.AddComponent(_player);

		_playbackDevice.Start();

		_player.Volume = 0.5f;
	}

	private AudioFormat GetAudioFormat(string songPath) {
		using var stream = File.OpenRead(songPath);
		AudioFormat? format = AudioFormat.GetFormatFromStream(stream);

		if (format != null)
			return (AudioFormat)format;

		return GetAudioFormatManually();
	}

	private AudioFormat GetAudioFormatManually() {
		var readOptions = new ReadOptions();
		readOptions.ReadTags = false;

		Dictionary<int, SampleFormat> intToSampleFormat = new() {
			{ 0, SampleFormat.S16 },
			{ 8, SampleFormat.U8 },
			{ 16, SampleFormat.S16 },
			{ 24, SampleFormat.S24 },
			{ 32, SampleFormat.S32 }
		};

		var result = SoundMetadataReader.Read(_songPath, readOptions);

		if (!result.IsSuccess) {
			string directory = Path.GetFileName(Path.GetDirectoryName(_songPath));
			string filename = Path.GetFileName(_songPath);
			throw new ArgumentException($"Couldn't read {Path.Combine(directory, filename)}: {result.Error.Message}");
		}

		SoundFormatInfo info = result.Value;

		var audioFormat = new AudioFormat();
		audioFormat.Format = intToSampleFormat[info.BitsPerSample];
		audioFormat.Channels = info.ChannelCount;
		audioFormat.SampleRate = info.SampleRate;
		audioFormat.Layout = AudioFormat.GetLayoutFromChannels(info.ChannelCount);

		return audioFormat;
	}

	public void Dispose() {
		_playbackDevice.Stop();
		_playbackDevice.Dispose();
		_streamDataProvider?.Dispose();
		_player.Dispose();
	}

	public void Pause() {
		_player.Pause();
		_needToPlayAudio = false;
	}

	public void Play() {
		_needToPlayAudio = true;
	}

	public void Reset() {
		_player.Stop();
		_streamDataProvider = new StreamDataProvider(_engine, _audioFormat, File.OpenRead(_songPath));
	}

	public void Update() {
		if (_needToPlayAudio && _player.State != PlaybackState.Playing) {
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
