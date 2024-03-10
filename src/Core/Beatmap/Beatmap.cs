using System;

namespace Rythmify.Core.Beatmap;

public class Beatmap {
	public BeatmapGeneralData GeneralData;
	public BeatmapEditorData EditorData;
	public BeatmapMetadata Metadata;
	public BeatmapDifficultyData DifficultyData;
	public BeatmapEvent[] Events;
	public BeatmapTimingPoint[] TimingPoints;
	public BeatmapColor[] Colors;
	public BeatmapHitObject[] HitObjects;

	public Beatmap() {
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
