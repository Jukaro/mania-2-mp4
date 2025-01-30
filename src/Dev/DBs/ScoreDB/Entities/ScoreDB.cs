using System;
using System.Collections.Generic;
using Rythmify.Core.Replay;
using Rythmify.Core.Shared;

namespace Rythmify.Core.Databases;

public class ScoreDB {
	public int GameVersion;
	public int BeatmapCount;
	public Dictionary<string, BeatmapWithScores> Beatmaps = new();

	public ScoreDB() {
		Beatmaps = new();
	}

	public override string ToString() {
		string str;

		str = $"GameVersion: {GameVersion}\n";
		str += $"BeatmapCount: {BeatmapCount}\n";

		return str;
	}
}
