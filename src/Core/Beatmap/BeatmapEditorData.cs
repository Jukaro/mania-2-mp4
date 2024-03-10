namespace Rythmify.Core.Beatmap;

public class BeatmapEditorData {
	public int[] Bookmarks;
	public double DistanceSpacing;
	public int BeatDivisor;
	public int GridSize;
	public double TimelineZoom;

	public override string ToString() => $"Bookmarks: [{(Bookmarks == null ? "null" : string.Join(", ", Bookmarks))}]\nDistanceSpacing: {DistanceSpacing}\nBeatDivisor: {BeatDivisor}\nGridSize: {GridSize}\nTimelineZoom: {TimelineZoom}";
}