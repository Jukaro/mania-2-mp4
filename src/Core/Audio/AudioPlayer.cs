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

	private MiniAudioEngine _engine = new();
	private FFmpegCodecFactory _ffmpegCodecFactory = new();
	private AudioFormat _audioFormat;
	private StreamDataProvider? _streamDataProvider;
	private AudioPlaybackDevice _playbackDevice;
	private SoundPlayer _player;

	public AudioPlayer(string songPath) {
		_engine.RegisterCodecFactory(_ffmpegCodecFactory);

		// unregister default codec factory because it's too strict with audio files headers
		_engine.UnregisterCodecFactory("SoundFlow.MiniAudio.Default");

		_engine.UpdateAudioDevicesInfo();
		var defaultDevice = _engine.PlaybackDevices.FirstOrDefault(x => x.IsDefault);

		_songPath = songPath;
		Task.Run(SetAudioFormat).Wait();
		_playbackDevice = _engine.InitializePlaybackDevice(defaultDevice, _audioFormat);
		Task.Run(SetStreamDataProvider).Wait();

		_player = new SoundPlayer(_engine, _audioFormat, _streamDataProvider);
		_playbackDevice.MasterMixer.AddComponent(_player);

		_playbackDevice.Start();

		_player.Volume = 0.5f;
	}

	private void SetAudioFormat() {
		using var stream = File.OpenRead(_songPath);
		AudioFormat? format = AudioFormat.GetFormatFromStream(stream);
		
		if (format != null) {
			_audioFormat = (AudioFormat)format;
			return;
		}

		SetAudioFormatManually();
	}

	private void SetAudioFormatManually() {
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
			Logger.LogError($"Couldn't read {Path.Combine(directory, filename)}: {result.Error.Message}");
			return;
		}

		SoundFormatInfo info = result.Value;

		_audioFormat = new AudioFormat();
		_audioFormat.Format = intToSampleFormat[info.BitsPerSample];
		_audioFormat.Channels = info.ChannelCount;
		_audioFormat.SampleRate = info.SampleRate;
		_audioFormat.Layout = AudioFormat.GetLayoutFromChannels(info.ChannelCount);
	}

	private void SetStreamDataProvider() {
		_streamDataProvider = new StreamDataProvider(_engine, File.OpenRead(_songPath));
	}

	public void Dispose() {
		_playbackDevice.Stop();
		_playbackDevice.Dispose();
		_streamDataProvider?.Dispose();
		_engine.Dispose();
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
