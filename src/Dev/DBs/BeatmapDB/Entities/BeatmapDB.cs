using System;
using System.Collections.Generic;

namespace Rythmify.Core.Databases;

public class BeatmapDB {
	public int GameVersion;
	public int FolderCount;
	public bool AccountUnlocked;
	public DateTime DateTime;
	public string PlayerName;
	public int BeatmapCount;
	public Dictionary<string, BeatmapDataFromDB> Beatmaps;
	public int UserPermissions;

	public BeatmapDB() {
		Beatmaps = new();
	}

	public override string ToString() {
		string str;

		str = $"GameVersion: {GameVersion}\n";
		str += $"FolderCount: {FolderCount}\n";
		str += $"AccountUnlocked: {AccountUnlocked}\n";
		str += $"DateTime: {DateTime}\n";
		str += $"PlayerName: {PlayerName}\n";
		str += $"BeatmapCount: {BeatmapCount}\n";

		return str;
	}
}
