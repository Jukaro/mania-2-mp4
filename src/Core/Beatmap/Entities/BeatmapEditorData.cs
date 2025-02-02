using System;

namespace Rythmify.Core.Beatmap;

public class BeatmapEditorData {
	public int[] Bookmarks;
	public double DistanceSpacing;
	public int BeatDivisor;
	public int GridSize;
	public double TimelineZoom;

	public BeatmapEditorData DeepClone() {
		BeatmapEditorData res = (BeatmapEditorData)MemberwiseClone();
		if (Bookmarks != null)
			Array.Copy(Bookmarks, res.Bookmarks, Bookmarks.Length);
		return res;
	}

	public override string ToString() => $"Bookmarks: [{(Bookmarks == null ? "null" : string.Join(", ", Bookmarks))}]\nDistanceSpacing: {DistanceSpacing}\nBeatDivisor: {BeatDivisor}\nGridSize: {GridSize}\nTimelineZoom: {TimelineZoom}";
}
