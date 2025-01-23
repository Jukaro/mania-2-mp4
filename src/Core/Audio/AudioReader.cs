using NAudio.Vorbis;
using NAudio.Wave;

namespace Rythmify.Core;

public class AudioReader {
	private readonly VorbisWaveReader _oggAudio;
	private readonly AudioFileReader _audio;
	private readonly bool _isOgg;

	public AudioReader(string filePath) {
		if (filePath.EndsWith(".ogg")) {
			_oggAudio = new VorbisWaveReader(filePath);
			_isOgg = true;
		}
		else {
			_audio = new AudioFileReader(filePath);
			_isOgg = false;
		}
	}

	public ISampleProvider GetSampleProvider() => _isOgg ? _oggAudio : _audio;
	public WaveStream GetWaveStream() => _isOgg ? _oggAudio : _audio;
}
