using System.IO;
using Rythmify.Core.Shared;

namespace Rythmify.Core.Databases;

public static partial class BeatmapDBParser {
	public static BeatmapDB Parse(string filePath) {
		var bytes = File.ReadAllBytes(filePath);
		int currentByteIndex = 0;

		BeatmapDB beatmapDB = new();

		beatmapDB.GameVersion = Parser.ParseInt(bytes, ref currentByteIndex);
		beatmapDB.FolderCount = Parser.ParseInt(bytes, ref currentByteIndex);
		beatmapDB.AccountUnlocked = Parser.ParseBool(bytes, ref currentByteIndex);
		beatmapDB.DateTime = new (Parser.ParseLong(bytes, ref currentByteIndex));
		beatmapDB.PlayerName = Parser.ParseString(bytes, ref currentByteIndex);
		beatmapDB.BeatmapCount = Parser.ParseInt(bytes, ref currentByteIndex);

		var watch = new System.Diagnostics.Stopwatch();
		watch.Start();

		for (int i = 0; i < beatmapDB.BeatmapCount; i++) {
			BeatmapDataFromDB beatmap = new();

			if (beatmapDB.GameVersion < 20191106)
				beatmap.SizeInBytes = Parser.ParseInt(bytes, ref currentByteIndex);

			beatmap.ArtistName = Parser.ParseString(bytes, ref currentByteIndex);
			beatmap.ArtistNameUnicode = Parser.ParseString(bytes, ref currentByteIndex);
			beatmap.SongTitle = Parser.ParseString(bytes, ref currentByteIndex);
			beatmap.SongTitleUnicode = Parser.ParseString(bytes, ref currentByteIndex);
			beatmap.CreatorName = Parser.ParseString(bytes, ref currentByteIndex);
			beatmap.Difficulty = Parser.ParseString(bytes, ref currentByteIndex);
			beatmap.AudioFilename = Parser.ParseString(bytes, ref currentByteIndex);
			beatmap.BeatmapMD5 = Parser.ParseString(bytes, ref currentByteIndex);
			beatmap.Filename = Parser.ParseString(bytes, ref currentByteIndex);
			beatmap.RankedStatus = Parser.ParseByte(bytes, ref currentByteIndex);
			beatmap.HitCirclesCount = Parser.ParseShort(bytes, ref currentByteIndex);
			beatmap.SlidersCount = Parser.ParseShort(bytes, ref currentByteIndex);
			beatmap.SpinnersCount = Parser.ParseShort(bytes, ref currentByteIndex);
			beatmap.LastModified = Parser.ParseLong(bytes, ref currentByteIndex);
			beatmap.ApproachRate = beatmapDB.GameVersion < 20140609 ? Parser.ParseByte(bytes, ref currentByteIndex) : Parser.ParseSingle(bytes, ref currentByteIndex);
			beatmap.CircleSize = beatmapDB.GameVersion < 20140609 ? Parser.ParseByte(bytes, ref currentByteIndex) : Parser.ParseSingle(bytes, ref currentByteIndex);
			beatmap.HPDrain = beatmapDB.GameVersion < 20140609 ? Parser.ParseByte(bytes, ref currentByteIndex) : Parser.ParseSingle(bytes, ref currentByteIndex);
			beatmap.OverallDifficulty = beatmapDB.GameVersion < 20140609 ? Parser.ParseByte(bytes, ref currentByteIndex) : Parser.ParseSingle(bytes, ref currentByteIndex);
			beatmap.SliderVelocity = Parser.ParseDouble(bytes, ref currentByteIndex);

			bool shouldSkipStarRatingParsing = false;

			if (beatmapDB.GameVersion >= 20140609) {
				if (beatmapDB.GameVersion <= 20250107) {
					beatmap.StandardStarRating = Parser.ParseIntDoubleDictionary(bytes, ref currentByteIndex, shouldSkipStarRatingParsing);
					beatmap.TaikoStarRating = Parser.ParseIntDoubleDictionary(bytes, ref currentByteIndex, shouldSkipStarRatingParsing);
					beatmap.CatchTheBeatStarRating = Parser.ParseIntDoubleDictionary(bytes, ref currentByteIndex, shouldSkipStarRatingParsing);
					beatmap.ManiaStarRating = Parser.ParseIntDoubleDictionary(bytes, ref currentByteIndex, shouldSkipStarRatingParsing);
				} else {
					beatmap.StandardStarRating = Parser.ParseIntFloatDictionary(bytes, ref currentByteIndex, shouldSkipStarRatingParsing);
					beatmap.TaikoStarRating = Parser.ParseIntFloatDictionary(bytes, ref currentByteIndex, shouldSkipStarRatingParsing);
					beatmap.CatchTheBeatStarRating = Parser.ParseIntFloatDictionary(bytes, ref currentByteIndex, shouldSkipStarRatingParsing);
					beatmap.ManiaStarRating = Parser.ParseIntFloatDictionary(bytes, ref currentByteIndex, shouldSkipStarRatingParsing);
				}
			}

			beatmap.DrainTime = Parser.ParseInt(bytes, ref currentByteIndex);
			beatmap.TotalTime = Parser.ParseInt(bytes, ref currentByteIndex);
			beatmap.AudioPreviewTime = Parser.ParseInt(bytes, ref currentByteIndex);

			// Console.WriteLine($"{beatmap}");

			int timingPointsCount = Parser.ParseInt(bytes, ref currentByteIndex);
			currentByteIndex += timingPointsCount * 17;

			beatmap.DifficultyID = Parser.ParseInt(bytes, ref currentByteIndex);
			beatmap.BeatmapID = Parser.ParseInt(bytes, ref currentByteIndex);
			beatmap.ThreadID = Parser.ParseInt(bytes, ref currentByteIndex);
			beatmap.StandardGrade = Parser.ParseByte(bytes, ref currentByteIndex);
			beatmap.TaikoGrade = Parser.ParseByte(bytes, ref currentByteIndex);
			beatmap.CatchTheBeatGrade = Parser.ParseByte(bytes, ref currentByteIndex);
			beatmap.ManiaGrade = Parser.ParseByte(bytes, ref currentByteIndex);
			beatmap.LocalOffset = Parser.ParseShort(bytes, ref currentByteIndex);
			beatmap.StackLeniency = Parser.ParseSingle(bytes, ref currentByteIndex);
			beatmap.Mode = (GameMode)Parser.ParseByte(bytes, ref currentByteIndex);
			beatmap.SongSource = Parser.ParseString(bytes, ref currentByteIndex);
			beatmap.SongTags = Parser.ParseString(bytes, ref currentByteIndex);
			beatmap.OnlineOffset = Parser.ParseShort(bytes, ref currentByteIndex);
			beatmap.SongTitleFont = Parser.ParseString(bytes, ref currentByteIndex);
			beatmap.IsUnplayed = Parser.ParseBool(bytes, ref currentByteIndex);
			beatmap.LastPlayed = Parser.ParseLong(bytes, ref currentByteIndex);
			beatmap.IsOsz2 = Parser.ParseBool(bytes, ref currentByteIndex);
			beatmap.FolderName = Parser.ParseString(bytes, ref currentByteIndex);
			beatmap.LastCheckTime = Parser.ParseLong(bytes, ref currentByteIndex);
			beatmap.IgnoreSound = Parser.ParseBool(bytes, ref currentByteIndex);
			beatmap.IgnoreSkin = Parser.ParseBool(bytes, ref currentByteIndex);
			beatmap.DisableStoryboard = Parser.ParseBool(bytes, ref currentByteIndex);
			beatmap.DisableVideo = Parser.ParseBool(bytes, ref currentByteIndex);
			beatmap.VisualOverride = Parser.ParseBool(bytes, ref currentByteIndex);

			if (beatmapDB.GameVersion < 20140609)
				currentByteIndex += 2; // skip unknown short
			currentByteIndex += 4; // skip unknown int

			beatmap.ManiaScrollSpeed = Parser.ParseByte(bytes, ref currentByteIndex);

			if (beatmapDB.Beatmaps.ContainsKey(beatmap.BeatmapMD5))
				Logger.LogWarning($"[BeatmapDBParser] Beatmap hash already present in the database: [{beatmap.CircleSize}] {beatmap.SongTitle} [{beatmap.Difficulty}]");
			if (beatmap.TotalTime > 0 && !beatmapDB.Beatmaps.ContainsKey(beatmap.BeatmapMD5))
				beatmapDB.Beatmaps.Add(beatmap.BeatmapMD5, beatmap);
		}

		watch.Stop();
		Logger.LogInfo($"[BeatmapDBParser] Successfully parsed {beatmapDB.BeatmapCount} beatmaps in {watch.ElapsedMilliseconds}ms");

		return beatmapDB;
	}
}
