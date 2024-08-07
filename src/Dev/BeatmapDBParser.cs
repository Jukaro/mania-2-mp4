using System;
using System.Collections.Generic;
using System.IO;
using Rythmify.Core.Shared;

namespace Rythmify.Core.BeatmapDB;

public static partial class BeatmapDBParser {

	private static int ParseIntFromULEB128(byte[] bytes, ref int index) {
		int result = 0;
		int shift = 0;
		int singleByte;
		int notEnd;
		int lowBytes;

		while (true) {
			singleByte = bytes[index];
			lowBytes = singleByte & ((1 << 7) - 1);
			result |= lowBytes << shift;
			notEnd = singleByte & (1 << 7);
			if (notEnd == 0)
				break;
			shift += 7;
			index++;
		}
		return result;
	}

	private static string ParseStringFromOSR(byte[] bytes, ref int index) {
		string str = "";

		if (bytes[index] == 0x00)
			index++;
		else if (bytes[index] == 0x0b)
		{
			index++;
			int length = ParseIntFromULEB128(bytes, ref index);
			index++;
			str = System.Text.Encoding.UTF8.GetString(bytes, index, length);
			index += length;
		}
		else
			throw new ArgumentException($"Unknown indicator for OSR string: {bytes[index]}");
		return str;
	}

	private static KeyValuePair<int, double> ParseIntDoublePairFromOSR(byte[] bytes, ref int index) {
		index++;
		int i = ParseIntFromOSR(bytes, ref index);
		index++;
		double d = ParseDoubleFromOSR(bytes, ref index);
		return new KeyValuePair<int, double>(i, d);
	}

	private static Dictionary<int, double> ParseDictionaryFromOSR(byte[] bytes, ref int index) {
		int pairCount = ParseIntFromOSR(bytes, ref index);
		if (pairCount < 0)
			return null;
		Dictionary<int, double> dict = new();
		for (int j = 0; j < pairCount; j++) {
			KeyValuePair<int, double> pair = ParseIntDoublePairFromOSR(bytes, ref index);
			dict[pair.Key] = pair.Value;
		}
		return dict;
	}

	private static bool ParseBoolFromOSR(byte[] bytes, ref int index) {
		bool result = bytes[index] != 0;
		index += 1;
		return result;
	}

	private static byte ParseByteFromOSR(byte[] bytes, ref int index) {
		byte result = bytes[index];
		index += 1;
		return result;
	}

	private static short ParseShortFromOSR(byte[] bytes, ref int index) {
		short result = BitConverter.ToInt16(bytes, index);
		index += 2;
		return result;
	}

	private static int ParseIntFromOSR(byte[] bytes, ref int index) {
		int result = BitConverter.ToInt32(bytes, index);
		index += 4;
		return result;
	}

	private static long ParseLongFromOSR(byte[] bytes, ref int index) {
		long result = BitConverter.ToInt64(bytes, index);
		index += 8;
		return result;
	}

	private static float ParseSingleFromOSR(byte[] bytes, ref int index) {
		float result = BitConverter.ToSingle(bytes, index);
		index += 4;
		return result;
	}

	private static double ParseDoubleFromOSR(byte[] bytes, ref int index) {
		double result = BitConverter.ToDouble(bytes, index);
		index += 8;
		return result;
	}

	public static BeatmapDB Parse(string filePath) {
		var bytes = File.ReadAllBytes(filePath);
		int currentByteIndex = 0;

		BeatmapDB beatmapDB = new();

		beatmapDB.GameVersion = ParseIntFromOSR(bytes, ref currentByteIndex);
		beatmapDB.FolderCount = ParseIntFromOSR(bytes, ref currentByteIndex);
		beatmapDB.AccountUnlocked = ParseBoolFromOSR(bytes, ref currentByteIndex);
		beatmapDB.DateTime = new (ParseLongFromOSR(bytes, ref currentByteIndex));
		beatmapDB.PlayerName = ParseStringFromOSR(bytes, ref currentByteIndex);
		beatmapDB.BeatmapCount = ParseIntFromOSR(bytes, ref currentByteIndex);

		for (int i = 0; i < beatmapDB.BeatmapCount; i++) {
			BeatmapDataFromDatabase beatmap = new();

			if (beatmapDB.GameVersion < 20191106)
				beatmap.SizeInBytes = ParseIntFromOSR(bytes, ref currentByteIndex);

			beatmap.ArtistName = ParseStringFromOSR(bytes, ref currentByteIndex);
			beatmap.ArtistNameUnicode = ParseStringFromOSR(bytes, ref currentByteIndex);
			beatmap.SongTitle = ParseStringFromOSR(bytes, ref currentByteIndex);
			beatmap.SongTitleUnicode = ParseStringFromOSR(bytes, ref currentByteIndex);
			beatmap.CreatorName = ParseStringFromOSR(bytes, ref currentByteIndex);
			beatmap.Difficulty = ParseStringFromOSR(bytes, ref currentByteIndex);
			beatmap.AudioFilename = ParseStringFromOSR(bytes, ref currentByteIndex);
			beatmap.BeatmapMD5 = ParseStringFromOSR(bytes, ref currentByteIndex);
			beatmap.Filename = ParseStringFromOSR(bytes, ref currentByteIndex);
			beatmap.RankedStatus = ParseByteFromOSR(bytes, ref currentByteIndex);
			beatmap.HitCirclesCount = ParseShortFromOSR(bytes, ref currentByteIndex);
			beatmap.SlidersCount = ParseShortFromOSR(bytes, ref currentByteIndex);
			beatmap.SpinnersCount = ParseShortFromOSR(bytes, ref currentByteIndex);
			beatmap.LastModified = ParseLongFromOSR(bytes, ref currentByteIndex);
			beatmap.ApproachRate = beatmapDB.GameVersion < 20140609 ? ParseByteFromOSR(bytes, ref currentByteIndex) : ParseSingleFromOSR(bytes, ref currentByteIndex);
			beatmap.CircleSize = beatmapDB.GameVersion < 20140609 ? ParseByteFromOSR(bytes, ref currentByteIndex) : ParseSingleFromOSR(bytes, ref currentByteIndex);
			beatmap.HPDrain = beatmapDB.GameVersion < 20140609 ? ParseByteFromOSR(bytes, ref currentByteIndex) : ParseSingleFromOSR(bytes, ref currentByteIndex);
			beatmap.OverallDifficulty = beatmapDB.GameVersion < 20140609 ? ParseByteFromOSR(bytes, ref currentByteIndex) : ParseSingleFromOSR(bytes, ref currentByteIndex);
			beatmap.SliderVelocity = ParseDoubleFromOSR(bytes, ref currentByteIndex);

			if (beatmapDB.GameVersion >= 20140609) {
				beatmap.StandardStarRating = ParseDictionaryFromOSR(bytes, ref currentByteIndex);
				beatmap.TaikoStarRating = ParseDictionaryFromOSR(bytes, ref currentByteIndex);
				beatmap.CatchTheBeatStarRating = ParseDictionaryFromOSR(bytes, ref currentByteIndex);
				beatmap.ManiaStarRating = ParseDictionaryFromOSR(bytes, ref currentByteIndex);
			}

			beatmap.DrainTime = ParseIntFromOSR(bytes, ref currentByteIndex);
			beatmap.TotalTime = ParseIntFromOSR(bytes, ref currentByteIndex);
			beatmap.AudioPreviewTime = ParseIntFromOSR(bytes, ref currentByteIndex);

			int timingPointsCount = ParseIntFromOSR(bytes, ref currentByteIndex);
			currentByteIndex += timingPointsCount * 17;

			beatmap.DifficultyID = ParseIntFromOSR(bytes, ref currentByteIndex);
			beatmap.BeatmapID = ParseIntFromOSR(bytes, ref currentByteIndex);
			beatmap.ThreadID = ParseIntFromOSR(bytes, ref currentByteIndex);
			beatmap.StandardGrade = ParseByteFromOSR(bytes, ref currentByteIndex);
			beatmap.TaikoGrade = ParseByteFromOSR(bytes, ref currentByteIndex);
			beatmap.CatchTheBeatGrade = ParseByteFromOSR(bytes, ref currentByteIndex);
			beatmap.ManiaGrade = ParseByteFromOSR(bytes, ref currentByteIndex);
			beatmap.LocalOffset = ParseShortFromOSR(bytes, ref currentByteIndex);
			beatmap.StackLeniency = ParseSingleFromOSR(bytes, ref currentByteIndex);
			beatmap.Mode = (GameMode)ParseByteFromOSR(bytes, ref currentByteIndex);
			beatmap.SongSource = ParseStringFromOSR(bytes, ref currentByteIndex);
			beatmap.SongTags = ParseStringFromOSR(bytes, ref currentByteIndex);
			beatmap.OnlineOffset = ParseShortFromOSR(bytes, ref currentByteIndex);
			beatmap.SongTitleFont = ParseStringFromOSR(bytes, ref currentByteIndex);
			beatmap.IsUnplayed = ParseBoolFromOSR(bytes, ref currentByteIndex);
			beatmap.LastPlayed = ParseLongFromOSR(bytes, ref currentByteIndex);
			beatmap.IsOsz2 = ParseBoolFromOSR(bytes, ref currentByteIndex);
			beatmap.FolderName = ParseStringFromOSR(bytes, ref currentByteIndex);
			beatmap.LastCheckTime = ParseLongFromOSR(bytes, ref currentByteIndex);
			beatmap.IgnoreSound = ParseBoolFromOSR(bytes, ref currentByteIndex);
			beatmap.IgnoreSkin = ParseBoolFromOSR(bytes, ref currentByteIndex);
			beatmap.DisableStoryboard = ParseBoolFromOSR(bytes, ref currentByteIndex);
			beatmap.DisableVideo = ParseBoolFromOSR(bytes, ref currentByteIndex);
			beatmap.VisualOverride = ParseBoolFromOSR(bytes, ref currentByteIndex);

			if (beatmapDB.GameVersion < 20140609)
				currentByteIndex += 2; // skip unknown short
			currentByteIndex += 4; // skip unknown int

			beatmap.ManiaScrollSpeed = ParseByteFromOSR(bytes, ref currentByteIndex);

			// Logger.LogDebug($"--- Beatmap: ---\n{beatmap}");
			// Logger.LogDebug($"Beatmap processed ({i}): {beatmap.FolderName} [{beatmap.Difficulty}]");

			try {
				beatmapDB.Beatmaps.Add(beatmap.BeatmapMD5, beatmap);
			} catch(Exception e) {
				// Logger.LogDebug($"Trying to add: {beatmap.FolderName} [{beatmap.Difficulty}], but {beatmapDB.Beatmaps[beatmap.BeatmapMD5].FolderName} [{beatmapDB.Beatmaps[beatmap.BeatmapMD5].Difficulty}] is already added.");
			}
		}

		return beatmapDB;
	}
}
