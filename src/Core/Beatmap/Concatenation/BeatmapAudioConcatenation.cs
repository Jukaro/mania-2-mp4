using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Rythmify.Core.Beatmap;

public partial class BeatmapConcatenation {
	public static void ConcatenateBeatmapAudio(List<BeatmapWithScores> beatmaps, Stream outputStream, int[] delays) {
		List<Stream> audioStreams = new();
		for (int i = 0; i < beatmaps.Count; i++)
			audioStreams.Add(new MemoryStream());

		TrimBeatmapAudio(beatmaps[0], audioStreams[0], TrimType.End, 0, delays[0]);
		for (int i = 1; i < beatmaps.Count - 1; i++)
			TrimBeatmapAudio(beatmaps[i], audioStreams[i], TrimType.Both, delays[i - 1], delays[i]);
		TrimBeatmapAudio(beatmaps.Last(), audioStreams.Last(), TrimType.Start, delays.Last(), 0);

		FFMpegAudioManipulation.Concatenate(audioStreams, outputStream);
		outputStream.Position = 0;
	}

	private static void TrimBeatmapAudio(BeatmapWithScores beatmap, Stream outputStream, TrimType trimOption, int startDelay, int endDelay) {
		int startMs = 0;
		if (trimOption == TrimType.Start || trimOption == TrimType.Both) {
			startMs = beatmap.Beatmap.HitObjects.First().Time;
			startMs -= startDelay / 2;
		}
		int? endMs = null;
		if (trimOption == TrimType.End || trimOption == TrimType.Both) {
			endMs = beatmap.Beatmap.HitObjects.Last().Time;
			if (beatmap.Beatmap.HitObjects.Last() is HoldHitObject holdNote)
				endMs = holdNote.EndTime;
			endMs += endDelay / 2;
		}

		var watch = new Stopwatch();
		watch.Start();

		FFMpegAudioManipulation.Trim(beatmap.AudioPath, outputStream, startMs, endMs);

		watch.Stop();
		Logger.LogDebug($"Trimmed audio in {watch.ElapsedMilliseconds}ms");

		Logger.LogDebug($"start: {startMs}, end: {endMs}, duration: {endMs - startMs}");
		Logger.LogDebug($"startDelay: {startDelay}, endDelay: {endDelay}");
	}
}
