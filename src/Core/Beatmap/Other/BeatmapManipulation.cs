using System;
using System.Collections.Generic;
using System.Linq;

namespace Rythmify.Core.Beatmap;

public enum TrimType {
	Start,
	End,
	Full
}

public static class BeatmapManipulation {
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

	private static void UpdateTimingPoints(BeatmapData beatmap, BeatmapData beatmapToAdd, int offset, int addedGap, TrimType trimOption) {
		BeatmapTimingPoint[] adjustedTimingPoints = beatmapToAdd.TimingPoints.Select(t => t.DeepClone()).ToArray();

		TrimExcessTimingPoints(ref adjustedTimingPoints, beatmapToAdd, addedGap, trimOption);

		for (int j = 0; j < adjustedTimingPoints.Length; j++)
				adjustedTimingPoints[j].Time += offset;

		beatmap.TimingPoints = beatmap.TimingPoints.Concat(adjustedTimingPoints).ToArray();
	}

	private static void UpdateHitObjects(BeatmapData beatmap, BeatmapData beatmapToAdd, int offset) {
		BeatmapHitObject[] adjustedHitObjects = beatmapToAdd.HitObjects.Select(t => t.DeepClone()).ToArray();

		for (int j = 0; j < adjustedHitObjects.Length; j++) {
			if (adjustedHitObjects[j] is HoldHitObject holdHitObject)
				holdHitObject.EndTime += offset;
			adjustedHitObjects[j].Time += offset;
		}

		beatmap.HitObjects = beatmap.HitObjects.Concat(adjustedHitObjects).ToArray();
	}

	private static void TrimExcessTimingPoints(ref BeatmapTimingPoint[] timingPoints, BeatmapData beatmapData, int addedGap, TrimType trimOption) {
		int beatmapStart = beatmapData.HitObjects.First().Time;
		int beatmapEnd = GetHitObjectEndTime(beatmapData.HitObjects.Last());

		List<BeatmapTimingPoint> temp = timingPoints.ToList();
		if (trimOption == TrimType.Start || trimOption == TrimType.Full)
			TrimStartTimingPoints(temp, beatmapStart, addedGap);
		if (trimOption == TrimType.End || trimOption == TrimType.Full)
			temp.RemoveAll(t => t.Time > beatmapEnd);
		timingPoints = temp.ToArray();
	}

	private static void TrimStartTimingPoints(List<BeatmapTimingPoint> trimmedTimingPoints, int beatmapStart, int addedGap) {
		BeatmapTimingPoint lastUninheritedTimingPoint = null;
		int i = 0;

		while (i < trimmedTimingPoints.Count - 1 && trimmedTimingPoints[i + 1].Time < beatmapStart) {
			if (trimmedTimingPoints[i].Uninherited)
				lastUninheritedTimingPoint = trimmedTimingPoints[i];
			i++;
		}

		trimmedTimingPoints.RemoveRange(0, i);
		trimmedTimingPoints.First().Time = beatmapStart - addedGap / 2;

		if (!trimmedTimingPoints.First().Uninherited) {
			lastUninheritedTimingPoint.Time = beatmapStart - addedGap / 2;
			trimmedTimingPoints.Prepend(lastUninheritedTimingPoint);
		}
	}

	private static int GetHitObjectEndTime(BeatmapHitObject beatmapHitObject) {
		if (beatmapHitObject is HoldHitObject holdHitObject)
			return holdHitObject.EndTime;
		return beatmapHitObject.Time;
	}
}
