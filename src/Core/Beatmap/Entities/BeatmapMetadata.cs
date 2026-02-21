using System;

namespace Rythmify.Core.Beatmap;

public class BeatmapMetadata {
	public string Title;
	public string TitleUnicode;
	public string Artist;
	public string ArtistUnicode;
	public string Creator;
	public string Version;
	public string Source;
	public string[] Tags;
	public int BeatmapID;
	public int BeatmapSetID;

	public BeatmapMetadata DeepClone() {
		BeatmapMetadata res = (BeatmapMetadata)MemberwiseClone();
		if (Tags != null)
			Array.Copy(Tags, res.Tags, Tags.Length);
		return res;
	}

	public override string ToString() => $"Title: {Title}\nTitleUnicode: {TitleUnicode}\nArtist: {Artist}\nArtistUnicode: {ArtistUnicode}\nCreator: {Creator}\nVersion: {Version}\nSource: {Source}\nTags: [{string.Join(", ", Tags)}]\nBeatmapID: {BeatmapID}\nBeatmapSetID: {BeatmapSetID}";
}
