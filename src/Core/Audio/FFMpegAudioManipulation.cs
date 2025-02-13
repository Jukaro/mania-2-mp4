using System;
using System.IO;
using FFMpegCore;
using FFMpegCore.Enums;
using FFMpegCore.Pipes;
using Rythmify.Core;

public static class FFMpegAudioManipulation {
	public static void FFmpegTrim(string inputFile, Stream outputStream, int start, int end) {
		IMediaAnalysis inputFileProbe = FFProbe.Analyse(inputFile);

		var customArgs = FFMpegArguments
			.FromFileInput(inputFile, false, options => options
				.Seek(TimeSpan.FromMilliseconds(start)))
			.OutputToPipe(new StreamPipeSink(outputStream), options => options
				.WithAudioCodec(AudioCodec.LibVorbis)
				.WithAudioBitrate((int)inputFileProbe.Format.BitRate / 1000)
				.WithDuration(TimeSpan.FromMilliseconds(end - start))
				.ForceFormat("ogg"));

		Logger.LogDebug(customArgs.Arguments);

		customArgs.ProcessSynchronously();
	}
}
