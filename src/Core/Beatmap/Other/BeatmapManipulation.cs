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
	public static BeatmapData Concatenate(List<BeatmapData> beatmaps, int addedGap) {
		BeatmapData beatmap = beatmaps[0].DeepClone();

		TrimExcessTimingPoints(ref beatmap.TimingPoints, beatmap, addedGap, TrimType.End);

		for (int i = 1; i < beatmaps.Count; i++) {
			int lastBeatmapEnd = GetHitObjectEndTime(beatmap.HitObjects.Last());
			int beatmapStart = beatmaps[i].HitObjects.First().Time;
			int offset = lastBeatmapEnd - beatmapStart + addedGap;

			UpdateTimingPoints(beatmap, beatmaps[i], offset, addedGap, i < beatmaps.Count - 1 ? TrimType.Full : TrimType.Start);
			UpdateHitObjects(beatmap, beatmaps[i], offset);
		}

		return beatmap;
	}

	public static void TrimBeatmapAudio(BeatmapWithScores beatmap, Stream outputStream, TrimType trimOption) {
		int startMs = 0;
		if (trimOption == TrimType.Start || trimOption == TrimType.Full)
			startMs = beatmap.Beatmap.HitObjects.First().Time;
		int? endMs = null;
		if (trimOption == TrimType.End || trimOption == TrimType.Full) {
			endMs = beatmap.Beatmap.HitObjects.Last().Time;
			if (beatmap.Beatmap.HitObjects.Last() is HoldHitObject holdNote)
				endMs = holdNote.EndTime;
		}

		var watch = new Stopwatch();
		watch.Start();

		FFMpegAudioManipulation.FFmpegTrim(beatmap.AudioPath, outputStream, startMs, endMs);

		watch.Stop();
		Logger.LogDebug($"Trimmed audio in {watch.ElapsedMilliseconds}ms");

		Logger.LogDebug($"start: {startMs}, end: {endMs}, duration: {endMs - startMs}");
	}
}
