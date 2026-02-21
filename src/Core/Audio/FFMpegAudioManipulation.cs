using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.Marshalling;
using FFMpegCore;
using FFMpegCore.Arguments;
using FFMpegCore.Enums;
using FFMpegCore.Pipes;
using Rythmify.Core;

public class ConcatenateWithCrossfade : IArgument {
	private string _text;
	string IArgument.Text => _text;

	public ConcatenateWithCrossfade(List<Stream> streams) {
		int i = 0;
		string str = "-filter_complex \"";
		string input1 = $"{i}:a";
		string output = "";

		int crossfadeDurationMs = 500;
		string overlapMode = "0";
		string curveType = "tri";

		while (i < streams.Count - 1) {
			string input2 = $"{i + 1}:a";
			output = $"out{i + 1}";

			str += $"[{input1}][{input2}]acrossfade=d={crossfadeDurationMs}ms:o={overlapMode}:c1={curveType}:c2={curveType}[{output}]";
			if (i < streams.Count - 2)
				str += "; ";
			input1 = output;
			i++;
		}
		str += $"\" -map \"[{output}]\"";

		_text = str;
	}
}

public class AdjustAudio : IArgument {
	private string _text;
	string IArgument.Text => _text;

	public AdjustAudio(int startChange, int endChange, int audioDuration) {
		int i = 0;
		string str = "-filter_complex \"";
		string input1 = $"{i}:a";
		string output = "";

		List<string> filters = new();

		if (startChange < 0 || endChange < 0) {
			string atrim = "atrim=";
			if (startChange < 0 && endChange < 0)
				atrim += $"start={Math.Abs(startChange)}ms:end={audioDuration + endChange}ms";
			else if (startChange < 0)
				atrim += $"start={Math.Abs(startChange)}ms";
			else if (endChange < 0)
				atrim += $"end={audioDuration + endChange}ms";
			filters.Add(atrim);
		}

		if (startChange > 0) {
			string adelay = $"adelay=delays={startChange}ms:all=1";
			filters.Add(adelay);
		}

		if (endChange > 0) {
			// string apad = $"apad=pad_dur={endChange}ms";
			string apad = $"apad=whole_dur={startChange + audioDuration + endChange}ms";
			filters.Add(apad);
		}

		while (i < filters.Count) {
			output = $"out{i + 1}";

			str += $"[{input1}]{filters[i]}[{output}]";
			if (i < filters.Count - 1)
				str += "; ";
			input1 = output;
			i++;
		}
		str += $"\" -map \"[{output}]\"";

		_text = str;
	}
}

public static class FFMpegAudioManipulation {
	public static void AdjustAudio(string inputFile, Stream outputStream, int startChange, int endChange) {
		IMediaAnalysis inputFileProbe = FFProbe.Analyse(inputFile);

		int audioDuration = (int)inputFileProbe.Duration.TotalMilliseconds;

		var customArgs = FFMpegArguments
			.FromFileInput(inputFile, false)
			.OutputToPipe(new StreamPipeSink(outputStream), options => options
				.WithArgument(new AdjustAudio(startChange, endChange, audioDuration))
				.WithAudioCodec(AudioCodec.LibVorbis)
				.WithAudioBitrate((int)inputFileProbe.Format.BitRate / 1000)
				.ForceFormat("ogg"));

		// Logger.LogDebug(customArgs.Arguments);

		customArgs.ProcessSynchronously();
	}

	public static void Concatenate(List<Stream> streams, Stream outputStream) {
		var inputArgs = FFMpegArguments.FromPipeInput(new StreamPipeSource(streams[0]));

		for (int i = 1; i < streams.Count; i++)
			inputArgs.AddPipeInput(new StreamPipeSource(streams[i]));

		var finalArgs = inputArgs.OutputToPipe(new StreamPipeSink(outputStream), options => options
			.WithArgument(new ConcatenateWithCrossfade(streams))
			.WithAudioCodec(AudioCodec.LibVorbis)
			.WithAudioBitrate(192)
			.ForceFormat("ogg"));

		// Logger.LogDebug(finalArgs.Arguments);

		foreach (Stream stream in streams)
			stream.Position = 0;

		finalArgs.ProcessAsynchronously().Wait();
	}
}
