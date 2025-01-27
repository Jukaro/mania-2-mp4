using System;
using System.Collections.Generic;

namespace Rythmify.Core.Beatmap;

public static partial class BeatmapParser {
	static readonly Dictionary<string, Action<BeatmapMetadata, string>> metadataDataProperties = new() {
		{"Title", (metadata, value) => { metadata.Title = value; } },
		{"TitleUnicode", (metadata, value) => { metadata.TitleUnicode = value; } },
		{"Artist", (metadata, value) => { metadata.Artist = value; } },
		{"ArtistUnicode", (metadata, value) => { metadata.ArtistUnicode = value; } },
		{"Creator", (metadata, value) => { metadata.Creator = value; } },
		{"Version", (metadata, value) => { metadata.Version = value; } },
		{"Source", (metadata, value) => { metadata.Source = value; } },
		{"Tags", (metadata, value) => { metadata.Tags = value.Split(' '); } },
		{"BeatmapID", (metadata, value) => { metadata.BeatmapID = int.Parse(value); } },
		{"BeatmapSetID", (metadata, value) => { metadata.BeatmapSetID = int.Parse(value); } },
	};
}
