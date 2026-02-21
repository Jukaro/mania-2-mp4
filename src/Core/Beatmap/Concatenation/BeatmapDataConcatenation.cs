using System;
using System.Collections.Generic;
using System.Linq;

namespace Rythmify.Core.Beatmap;

public partial class BeatmapConcatenation {
	public static BeatmapData ConcatenateBeatmapData(List<BeatmapData> beatmaps, int[] delays) {
		BeatmapData beatmap = beatmaps[0].DeepClone();

		Logger.LogDebug($"delays: {string.Join(",", delays.Select(d => $"{d}ms"))}");

		TrimExcessTimingPoints(ref beatmap.TimingPoints, beatmap, delays[0], TrimType.End);

		for (int i = 1; i < beatmaps.Count; i++) {
			int lastBeatmapEnd = GetBeatmapEndTime(beatmap);
			int beatmapStart = beatmaps[i].HitObjects.First().Time;
			int offset = lastBeatmapEnd - beatmapStart + delays[i - 1];

			UpdateTimingPoints(beatmap, beatmaps[i], offset, delays[i - 1], i < beatmaps.Count - 1 ? TrimType.Both : TrimType.Start);
			UpdateHitObjects(beatmap, beatmaps[i], offset);
		}

		UpdateInheritedTimingPointsWithDominantBpm(beatmap);

		return beatmap;
	}

	private static void UpdateTimingPoints(BeatmapData beatmap, BeatmapData beatmapToAdd, int offset, int delay, TrimType trimOption) {
		BeatmapTimingPoint[] adjustedTimingPoints = beatmapToAdd.TimingPoints.Select(t => t.DeepClone()).ToArray();

		TrimExcessTimingPoints(ref adjustedTimingPoints, beatmapToAdd, delay, trimOption);

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

	private static void TrimExcessTimingPoints(ref BeatmapTimingPoint[] timingPoints, BeatmapData beatmapData, int delay, TrimType trimOption) {
		int beatmapStart = beatmapData.HitObjects.First().Time;
		int beatmapEnd = GetBeatmapEndTime(beatmapData);

		List<BeatmapTimingPoint> temp = timingPoints.ToList();
		if (trimOption == TrimType.Start || trimOption == TrimType.Both)
			TrimStartTimingPoints(temp, beatmapStart, delay);
		if (trimOption == TrimType.End || trimOption == TrimType.Both)
			temp.RemoveAll(t => t.Time > beatmapEnd);
		timingPoints = temp.ToArray();
	}

	private static void TrimStartTimingPoints(List<BeatmapTimingPoint> trimmedTimingPoints, int beatmapStart, int delay) {
		BeatmapTimingPoint lastUninheritedTimingPoint = null;
		int i = 0;

		while (i < trimmedTimingPoints.Count - 1 && trimmedTimingPoints[i + 1].Time < beatmapStart) {
			if (trimmedTimingPoints[i].Uninherited)
				lastUninheritedTimingPoint = trimmedTimingPoints[i];
			i++;
		}

		trimmedTimingPoints.RemoveRange(0, i);
		trimmedTimingPoints.First().Time = beatmapStart;

		if (!trimmedTimingPoints.First().Uninherited) {
			lastUninheritedTimingPoint.Time = beatmapStart;
			trimmedTimingPoints.Prepend(lastUninheritedTimingPoint);
		}
	}

	private static void UpdateInheritedTimingPointsWithDominantBpm(BeatmapData beatmap) {
		double dominantBpm = BeatmapParser.GetDominantBpm(beatmap);

		int index = 0;
		while (index < beatmap.TimingPoints.Length && beatmap.TimingPoints[index].Time < beatmap.HitObjects[0].Time) index++;

		BeatmapTimingPoint firstTimingPoint = beatmap.TimingPoints[0].DeepClone();
		firstTimingPoint.Time = beatmap.HitObjects[0].Time;
		List<BeatmapTimingPoint> test = beatmap.TimingPoints.ToList();
		test.Insert(index, firstTimingPoint);
		beatmap.TimingPoints = test.ToArray();

		List<BeatmapTimingPoint> timingPointsList = beatmap.TimingPoints.ToList();
		int addedCount = 0;

		for (int i = 0; i < beatmap.TimingPoints.Length; i++) {
			if (beatmap.TimingPoints[i].Uninherited) {
				BeatmapTimingPoint newTimingPoint = beatmap.TimingPoints[i].DeepClone();
				newTimingPoint.LastBPM = newTimingPoint.BPM;
				newTimingPoint.Uninherited = false;
				newTimingPoint.BeatLength = -100;
				timingPointsList.Insert(i + 1 + addedCount, newTimingPoint);
				addedCount++;
			}
		}

		beatmap.TimingPoints = timingPointsList.ToArray();

		foreach (BeatmapTimingPoint timingPoint in beatmap.TimingPoints) {
			if (!timingPoint.Uninherited)
				timingPoint.BeatLength = timingPoint.LastBPM * timingPoint.BeatLength / dominantBpm;
		}
	}

	private static int GetHitObjectEndTime(BeatmapHitObject beatmapHitObject) => beatmapHitObject is HoldHitObject holdHitObject ? holdHitObject.EndTime : beatmapHitObject.Time;

	private static int GetBeatmapEndTime(BeatmapData beatmap) {
		Dictionary<int, BeatmapHitObject> lastHitObjects = new ();

		for (int i = beatmap.HitObjects.Length - 1; i >= 0; i--) {
			BeatmapHitObject hitObject = beatmap.HitObjects[i];
			if (!lastHitObjects.ContainsKey(hitObject.X))
				lastHitObjects.Add(hitObject.X, hitObject);
			if (lastHitObjects.Count == (int)beatmap.DifficultyData.CircleSize)
				break;
		}

		return lastHitObjects.Max(h => GetHitObjectEndTime(h.Value));
	}
}
