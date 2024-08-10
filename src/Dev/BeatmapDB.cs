using System;
using System.Collections.Generic;
using Rythmify.Core.Shared;

namespace Rythmify.Core.Databases;

public class BeatmapDB {
	public int GameVersion;
	public int FolderCount;
	public bool AccountUnlocked;
	public DateTime DateTime;
	public string PlayerName;
	public int BeatmapCount;
	public Dictionary<string, BeatmapDataFromDatabase> Beatmaps;
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

public class BeatmapDataFromDatabase {
	public int SizeInBytes = 0;
	public string ArtistName;
	public string ArtistNameUnicode;
	public string SongTitle;
	public string SongTitleUnicode;
	public string CreatorName;
	public string Difficulty;
	public string AudioFilename;
	public string BeatmapMD5;
	public string Filename;
	public byte RankedStatus;
	public short HitCirclesCount;
	public short SlidersCount;
	public short SpinnersCount;
	public long LastModified;
	public float ApproachRate;
	public float CircleSize;
	public float HPDrain;
	public float OverallDifficulty;
	public double SliderVelocity;
	public Dictionary<int, double> StandardStarRating;
	public Dictionary<int, double> TaikoStarRating;
	public Dictionary<int, double> CatchTheBeatStarRating;
	public Dictionary<int, double> ManiaStarRating;
	public int DrainTime;
	public int TotalTime;
	public int AudioPreviewTime;
	// timing points list
	public int DifficultyID;
	public int BeatmapID;
	public int ThreadID;
	public byte StandardGrade;
	public byte TaikoGrade;
	public byte CatchTheBeatGrade;
	public byte ManiaGrade;
	public short LocalOffset;
	public float StackLeniency;
	public GameMode Mode;
	public string SongSource;
	public string SongTags;
	public short OnlineOffset;
	public string SongTitleFont;
	public bool IsUnplayed;
	public long LastPlayed;
	public bool IsOsz2;
	public string FolderName;
	public long LastCheckTime;
	public bool IgnoreSound;
	public bool IgnoreSkin;
	public bool DisableStoryboard;
	public bool DisableVideo;
	public bool VisualOverride;
	public byte ManiaScrollSpeed;

	public override string ToString() {
		string str;

		str = $"SizeInBytes: {SizeInBytes}\n";
		str += $"ArtistName: {ArtistName}\n";
		str += $"ArtistNameUnicode: {ArtistNameUnicode}\n";
		str += $"SongTitle: {SongTitle}\n";
		str += $"SongTitleUnicode: {SongTitleUnicode}\n";
		str += $"CreatorName: {CreatorName}\n";
		str += $"Difficulty: {Difficulty}\n";
		str += $"AudioFilename: {AudioFilename}\n";
		str += $"BeatmapMD5: {BeatmapMD5}\n";
		str += $"Filename: {Filename}\n";
		str += $"RankedStatus: {RankedStatus}\n";
		str += $"HitCirclesCount: {HitCirclesCount}\n";
		str += $"SlidersCount: {SlidersCount}\n";
		str += $"SpinnersCount: {SpinnersCount}\n";
		str += $"LastModified: {LastModified}\n";
		str += $"ApproachRate: {ApproachRate}\n";
		str += $"CircleSize: {CircleSize}\n";
		str += $"HPDrain: {HPDrain}\n";
		str += $"OverallDifficulty: {OverallDifficulty}\n";
		str += $"SliderVelocity: {SliderVelocity}\n";
		// star ratings
		str += $"DrainTime: {DrainTime}\n";
		str += $"TotalTime: {TotalTime}\n";
		str += $"AudioPreviewTime: {AudioPreviewTime}\n";
		// timing points
		str += $"DifficultyID: {DifficultyID}\n";
		str += $"BeatmapID: {BeatmapID}\n";
		str += $"ThreadID: {ThreadID}\n";
		str += $"StandardGrade: {StandardGrade}\n";
		str += $"TaikoGrade: {TaikoGrade}\n";
		str += $"CatchTheBeatGrade: {CatchTheBeatGrade}\n";
		str += $"ManiaGrade: {ManiaGrade}\n";
		str += $"LocalOffset: {LocalOffset}\n";
		str += $"StackLeniency: {StackLeniency}\n";
		str += $"Mode: {Mode}\n";
		str += $"SongSource: {SongSource}\n";
		str += $"SongTags: {SongTags}\n";
		str += $"OnlineOffset: {OnlineOffset}\n";
		str += $"SongTitleFont: {SongTitleFont}\n";
		str += $"IsUnplayed: {IsUnplayed}\n";
		str += $"LastPlayed: {LastPlayed}\n";
		str += $"IsOsz2: {IsOsz2}\n";
		str += $"FolderName: {FolderName}\n";
		str += $"LastCheckTime: {LastCheckTime}\n";
		str += $"IgnoreSound: {IgnoreSound}\n";
		str += $"IgnoreSkin: {IgnoreSkin}\n";
		str += $"DisableStoryboard: {DisableStoryboard}\n";
		str += $"DisableVideo: {DisableVideo}\n";
		str += $"VisualOverride: {VisualOverride}\n";
		str += $"ManiaScrollSpeed: {ManiaScrollSpeed}\n";

		return str;
	}
}
