using System;
using System.IO;
using FFMpegCore;
using FFMpegCore.Enums;
using FFMpegCore.Pipes;
using Rythmify.Core;

public static class FFMpegAudioManipulation {
	public static void FFmpegTrim(string inputFile, Stream outputStream, int startMs, int? endMs) {
		IMediaAnalysis inputFileProbe = FFProbe.Analyse(inputFile);

		TimeSpan? duration = endMs != null ? TimeSpan.FromMilliseconds((int)endMs - startMs) : null;

		var customArgs = FFMpegArguments
			.FromFileInput(inputFile, false, options => options
				.Seek(TimeSpan.FromMilliseconds(startMs)))
			.OutputToPipe(new StreamPipeSink(outputStream), options => options
				.WithAudioCodec(AudioCodec.LibVorbis)
				.WithAudioBitrate((int)inputFileProbe.Format.BitRate / 1000)
				.WithDuration(duration)
				.ForceFormat("ogg"));

		Logger.LogDebug(customArgs.Arguments);

		customArgs.ProcessSynchronously();
	}
}
