using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using FFMpegCore;

namespace Rythmify.Core.Beatmap;

public partial class BeatmapConcatenation {
	public static void ConcatenateBeatmapAudio(List<BeatmapWithScores> beatmaps, Stream outputStream, int[] delays) {
		List<Stream> audioStreams = new();
		for (int i = 0; i < beatmaps.Count; i++)
			audioStreams.Add(new MemoryStream());

		Logger.LogInfo($"\n[BeatmapConcatenation] Adjusting beatmap audio 1/{beatmaps.Count}");
		AdjustBeatmapAudio(beatmaps[0], audioStreams[0], TrimType.End, 0, delays[0]);

		for (int i = 1; i < beatmaps.Count - 1; i++) {
			Logger.LogInfo($"\n[BeatmapConcatenation] Adjusting beatmap audio {i + 1}/{beatmaps.Count}");
			AdjustBeatmapAudio(beatmaps[i], audioStreams[i], TrimType.Both, delays[i - 1], delays[i]);
		}

		Logger.LogInfo($"\n[BeatmapConcatenation] Adjusting beatmap audio {beatmaps.Count}/{beatmaps.Count}");
		AdjustBeatmapAudio(beatmaps.Last(), audioStreams.Last(), TrimType.Start, delays.Last(), 0);

		Logger.LogInfo($"\n[BeatmapConcatenation] Combining audio");
		FFMpegAudioManipulation.Concatenate(audioStreams, outputStream);
		outputStream.Position = 0;
	}

	//? AdjustBeatmapAudio
	private static void AdjustBeatmapAudio(BeatmapWithScores beatmap, Stream outputStream, TrimType trimOption, int startDelay, int endDelay) {
		int startChange = 0;
		int endChange = 0;

		IMediaAnalysis inputFileProbe = FFProbe.Analyse(beatmap.AudioPath);
		int audioLength = (int)inputFileProbe.Duration.TotalMilliseconds;

		if (trimOption == TrimType.Start || trimOption == TrimType.Both) {
			startChange = startDelay / 2 - beatmap.Beatmap.HitObjects.First().Time;
		}
		if (trimOption == TrimType.End || trimOption == TrimType.Both) {
			int endTime = GetBeatmapEndTime(beatmap.Beatmap);
			endChange = endTime + endDelay / 2 - audioLength;
		}

		Logger.LogInfo($"[BeatmapConcatenation] startDelay: {startDelay}, endDelay: {endDelay}");
		Logger.LogInfo($"[BeatmapConcatenation] startChange: {startChange}, endChange: {endChange}, duration: {startChange + audioLength + endChange}");

		var watch = new Stopwatch();
		watch.Start();

		FFMpegAudioManipulation.AdjustAudio(beatmap.AudioPath, outputStream, startChange, endChange);

		watch.Stop();
		// Logger.LogDebug($"Trimmed audio in {watch.ElapsedMilliseconds}ms");
	}
}
