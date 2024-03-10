using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Rythmify.Core.Beatmap;

public static partial class BeatmapParser {
	static readonly Dictionary<string, Action<Beatmap, string[]>> sectionToParser = new(){
		{"General", (beatmap, lines) => { beatmap.GeneralData = ParseKeyValueSection(lines, generalDataProperties); } },
		{"Editor", (beatmap, lines) => { beatmap.EditorData = ParseKeyValueSection(lines, editorDataProperties); } },
		{"Metadata", (beatmap, lines) => { beatmap.Metadata = ParseKeyValueSection(lines, metadataDataProperties); } },
		{"Difficulty", (beatmap, lines) => { beatmap.DifficultyData = ParseKeyValueSection(lines, difficultyDataProperties); } },
		{"Events", (beatmap, lines) => { beatmap.Events = ParseEventsSection(lines); } },
		{"TimingPoints", (beatmap, lines) => { beatmap.TimingPoints = ParseTimingPointsSection(lines); } },
		{"Colours", (beatmap, lines) => { beatmap.Colors = ParseColorsSection(lines); } },
		// {"HitObjects", (beatmap, lines) => { beatmap.HitObjects = ParseHitObjectsSection(lines); } },
	};

	private static bool IsSectionString(string line) => line.StartsWith("[") && line.EndsWith("]");
	private static bool IsCommentString(string line) => line.StartsWith("//");

	public static Beatmap Parse(string filePath) {
		var lines = File.ReadAllLines(filePath);
		var filteredLines = (from line in lines where !IsCommentString(line) && !string.IsNullOrWhiteSpace(line) select line).ToArray();

		Beatmap beatmap = new();

		for (int currentLineIndex = 0; currentLineIndex < filteredLines.Length; currentLineIndex++) {
			var line = filteredLines[currentLineIndex];

			if (!IsSectionString(line))
				continue;

			Logger.LogDebug($"Parsing section {line}");

			var sectionName = line[1..^1];
			if (!sectionToParser.ContainsKey(sectionName)) {
				Logger.LogWarning($"Unknown section {sectionName}");
				continue;
			}

			var sectionEnd = Array.FindIndex(filteredLines, currentLineIndex + 1, (string l) => IsSectionString(l));
			var sectionLines = filteredLines[(currentLineIndex + 1)..sectionEnd];
			sectionToParser[sectionName](beatmap, sectionLines);
		}

		return beatmap;
	}
}
