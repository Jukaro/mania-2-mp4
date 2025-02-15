using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Rythmify.Core.Beatmap;

public enum TrimType {
	Start,
	End,
	Full
}

public static partial class BeatmapManipulation {
	public static BeatmapData ConcatenateBeatmap(List<BeatmapData> beatmaps, int delay) {
		BeatmapData beatmap = beatmaps[0].DeepClone();

		TrimExcessTimingPoints(ref beatmap.TimingPoints, beatmap, delay, TrimType.End);

		for (int i = 1; i < beatmaps.Count; i++) {
			int lastBeatmapEnd = GetHitObjectEndTime(beatmap.HitObjects.Last());
			int beatmapStart = beatmaps[i].HitObjects.First().Time;
			int offset = lastBeatmapEnd - beatmapStart + delay;

			UpdateTimingPoints(beatmap, beatmaps[i], offset, delay, i < beatmaps.Count - 1 ? TrimType.Full : TrimType.Start);
			UpdateHitObjects(beatmap, beatmaps[i], offset);
		}

		UpdateInheritedTimingPointsWithDominantBpm(beatmap);

		return beatmap;
	}

	public static void TrimBeatmapAudio(BeatmapWithScores beatmap, Stream outputStream, TrimType trimOption, int delay) {
		int startMs = 0;
		if (trimOption == TrimType.Start || trimOption == TrimType.Full) {
			startMs = beatmap.Beatmap.HitObjects.First().Time;
			startMs -= delay / 2;
		}
		int? endMs = null;
		if (trimOption == TrimType.End || trimOption == TrimType.Full) {
			endMs = beatmap.Beatmap.HitObjects.Last().Time;
			if (beatmap.Beatmap.HitObjects.Last() is HoldHitObject holdNote)
				endMs = holdNote.EndTime;
			endMs += delay / 2;
		}

		var watch = new Stopwatch();
		watch.Start();

		FFMpegAudioManipulation.Trim(beatmap.AudioPath, outputStream, startMs, endMs);

		watch.Stop();
		Logger.LogDebug($"Trimmed audio in {watch.ElapsedMilliseconds}ms");

		Logger.LogDebug($"start: {startMs}, end: {endMs}, duration: {endMs - startMs}");
	}

	public static void ConcatenateBeatmapAudio(List<BeatmapWithScores> beatmaps, Stream outputStream, int delay) {
		List<Stream> audioStreams = new();
		for (int i = 0; i < beatmaps.Count; i++)
			audioStreams.Add(new MemoryStream());

		TrimBeatmapAudio(beatmaps[0], audioStreams[0], TrimType.End, delay);
		for (int i = 1; i < beatmaps.Count - 1; i++)
			TrimBeatmapAudio(beatmaps[i], audioStreams[i], TrimType.Full, delay);
		TrimBeatmapAudio(beatmaps.Last(), audioStreams.Last(), TrimType.Start, delay);

		FFMpegAudioManipulation.Concatenate(audioStreams, outputStream);
		outputStream.Position = 0;
	}
}
