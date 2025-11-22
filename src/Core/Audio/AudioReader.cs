using System.IO;
using NAudio.Vorbis;
using NAudio.Wave;
using NLayer.NAudioSupport;
using SoundFlow.Providers;

namespace Rythmify.Core;

public class AudioReader {
	private readonly VorbisWaveReader _oggAudio;
	private readonly AudioFileReader _audio;
	private readonly Mp3FileReaderBase _mp3Audio;
	private readonly bool _isOgg;

	public AudioReader(string filePath) {
		if (filePath.EndsWith(".ogg")) {
			_oggAudio = new VorbisWaveReader(filePath);
			_isOgg = true;
		}
		else {
			// _audio = new AudioFileReader(filePath);
			var builder = new Mp3FileReader.FrameDecompressorBuilder(wf => new Mp3FrameDecompressor(wf));
			_mp3Audio = new Mp3FileReaderBase(filePath, builder);
			_isOgg = false;
			// var reader = new StreamDataProvider(File.OpenRead(filePath));
		}
	}

	public ISampleProvider GetSampleProvider() => _isOgg ? _oggAudio : _audio;
	public WaveStream GetWaveStream() => _isOgg ? _oggAudio : _audio;

	public Mp3FileReaderBase GetMp3FileReaderBase() => _mp3Audio;
}
