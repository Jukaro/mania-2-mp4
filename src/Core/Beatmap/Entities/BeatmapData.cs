using System;
using System.Data;
using System.Linq;

namespace Rythmify.Core.Beatmap;

public class BeatmapData {
	public BeatmapGeneralData GeneralData;
	public BeatmapEditorData EditorData;
	public BeatmapMetadata Metadata;
	public BeatmapDifficultyData DifficultyData;
	public BeatmapEvent[] Events;
	public BeatmapTimingPoint[] TimingPoints;
	public BeatmapColor[] Colors;
	public BeatmapHitObject[] HitObjects;
	public double DominantBpm;

	public double GetLastObjectTime() {
		if (HitObjects.Length == 0)
			return 0;

		var lastHitObject = HitObjects[^1];
		if (lastHitObject is HoldHitObject holdHitObject)
			return holdHitObject.EndTime;

		return lastHitObject.Time;
	}

	public double GetDominantBpm() {
		double lastTime;

		lastTime = GetLastObjectTime();

		var (beatLength, duration) =
			TimingPoints.Select((t, i) => {
				if (!t.Uninherited)
					return (beatLength: t.BeatLength, 0);

				if (t.Time > lastTime)
					return (beatLength: t.BeatLength, 0);

				var nextTimingPoint = TimingPoints.Skip(i + 1).FirstOrDefault(tp => tp.Uninherited, null);

				double nextTime = i == TimingPoints.Length - 1 ? lastTime : (nextTimingPoint?.Time ?? lastTime);
				double currentTime = t.Time;

				return (beatLength: t.BeatLength, duration: nextTime - currentTime);
			})
			.GroupBy(t => Math.Round(t.beatLength * 1000) / 1000)
			.Select(g => (beatLength: g.Key, duration: g.Sum(t => t.duration)))
			.OrderByDescending(i => i.duration).FirstOrDefault();

		if (beatLength == 0)
			return 60000 / 1000;

		return 60000 / beatLength;
	}

	public BeatmapData() {
		GeneralData = new BeatmapGeneralData();
		EditorData = new BeatmapEditorData();
		Metadata = new BeatmapMetadata();
		DifficultyData = new BeatmapDifficultyData();
		Events = Array.Empty<BeatmapEvent>();
		TimingPoints = Array.Empty<BeatmapTimingPoint>();
		Colors = Array.Empty<BeatmapColor>();
		HitObjects = Array.Empty<BeatmapHitObject>();
	}

	public override string ToString() => $"GeneralData:\n{GeneralData}\nEditorData:\n{EditorData}\nMetadata:\n{Metadata}\nDifficultyData:\n{DifficultyData}\nEvents:\n{string.Join("\n", Array.ConvertAll(Events, (e) => e.ToString()))}\nTimingPoints:\n{string.Join("\n", Array.ConvertAll(TimingPoints, (tp) => tp.ToString()))}\nColors:\n{string.Join("\n", Array.ConvertAll(Colors, (c) => c.ToString()))}\nHitObjects:\n{string.Join("\n", Array.ConvertAll(HitObjects, (ho) => ho.ToString()))}";
}
