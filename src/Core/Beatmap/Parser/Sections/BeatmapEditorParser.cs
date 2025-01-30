using System;
using System.Collections.Generic;
using System.Globalization;

namespace Rythmify.Core.Beatmap;

public static partial class BeatmapParser {
	static readonly Dictionary<string, Action<BeatmapEditorData, string>> editorDataProperties = new() {
		{"Bookmarks", (editor, value) => { editor.Bookmarks = Array.ConvertAll(value.Split(','), int.Parse); } },
		{"DistanceSpacing", (editor, value) => { editor.DistanceSpacing = double.Parse(value, CultureInfo.InvariantCulture); } },
		{"BeatDivisor", (editor, value) => { editor.BeatDivisor = int.Parse(value); } },
		{"GridSize", (editor, value) => { editor.GridSize = int.Parse(value); }},
		{"TimelineZoom", (editor, value) => { editor.TimelineZoom = double.Parse(value, CultureInfo.InvariantCulture); } },
	};
}
