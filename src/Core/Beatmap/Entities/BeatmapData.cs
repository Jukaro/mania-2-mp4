using System;
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

	public BeatmapData DeepClone() {
		return new() {
			GeneralData = GeneralData.DeepClone(),
			EditorData = EditorData.DeepClone(),
			Metadata = Metadata.DeepClone(),
			DifficultyData = DifficultyData.DeepClone(),
			Events = Events.Select(e => e.DeepClone()).ToArray(),
			TimingPoints = TimingPoints.Select(t => t.DeepClone()).ToArray(),
			Colors = Colors.Select(c => c.DeepClone()).ToArray(),
			HitObjects = HitObjects.Select(h => h.DeepClone()).ToArray()
		};
	}

	public override string ToString() => $"GeneralData:\n{GeneralData}\nEditorData:\n{EditorData}\nMetadata:\n{Metadata}\nDifficultyData:\n{DifficultyData}\nEvents:\n{string.Join("\n", Array.ConvertAll(Events, (e) => e.ToString()))}\nTimingPoints:\n{string.Join("\n", Array.ConvertAll(TimingPoints, (tp) => tp.ToString()))}\nColors:\n{string.Join("\n", Array.ConvertAll(Colors, (c) => c.ToString()))}\nHitObjects:\n{string.Join("\n", Array.ConvertAll(HitObjects, (ho) => ho.ToString()))}";

	public const double DefaultBPM = 60000 / 1000.0;
}
