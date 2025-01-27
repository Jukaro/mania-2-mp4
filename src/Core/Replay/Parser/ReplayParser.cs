using System;
using System.IO;
using Rythmify.Core.Shared;
using Rythmify.UI;

namespace Rythmify.Core.Replay;

public static partial class ReplayParser {
	public static ReplayData Parse(byte[] bytes, int currentByteIndex, int laneCount, bool skipInputsParsing) {
		ReplayData replay = ParseBytes(bytes, currentByteIndex, laneCount, skipInputsParsing);
		replay.FilePath = Path.Join(Path.Join(Paths.OsuDirectoryPath, "Data/r"), replay.BeatmapMD5 + "-" + replay.ReplayTimeStamp + ".osr");

		return replay;
	}

	public static ReplayData Parse(string filePath, int laneCount, bool skipInputsParsing) {
		var bytes = File.ReadAllBytes(filePath);

		ReplayData replay = ParseBytes(bytes, 0, laneCount, skipInputsParsing);
		replay.FilePath = filePath;

		return replay;
	}

	private static ReplayData ParseBytes(byte[] bytes, int start, int laneCount, bool skipInputsParsing) {
		int currentByteIndex = start;

		if (!Enum.IsDefined(typeof(GameMode), (int)bytes[currentByteIndex]))
			throw new ArgumentException($"Unexpected argument type for enum GameMode: {bytes[currentByteIndex]}");

		ReplayData replay = new(laneCount)
		{
			GameMode = (GameMode)Parser.ParseByte(bytes, ref currentByteIndex),
			GameVersion = Parser.ParseInt(bytes, ref currentByteIndex),
			BeatmapMD5 = Parser.ParseString(bytes, ref currentByteIndex),
			PlayerName = Parser.ParseString(bytes, ref currentByteIndex),
			ReplayMD5 = Parser.ParseString(bytes, ref currentByteIndex),
			Nb300s = Parser.ParseShort(bytes, ref currentByteIndex),
			Nb100s = Parser.ParseShort(bytes, ref currentByteIndex),
			Nb50s = Parser.ParseShort(bytes, ref currentByteIndex),
			NbMax300s = Parser.ParseShort(bytes, ref currentByteIndex),
			Nb200s = Parser.ParseShort(bytes, ref currentByteIndex),
			NbMiss = Parser.ParseShort(bytes, ref currentByteIndex),
			Score = Parser.ParseInt(bytes, ref currentByteIndex),
			MaxCombo = Parser.ParseShort(bytes, ref currentByteIndex),
			FullCombo = Parser.ParseBool(bytes, ref currentByteIndex),
			Mods = Parser.ParseInt(bytes, ref currentByteIndex),
			LifeBar = Parser.ParseString(bytes, ref currentByteIndex),
			TimeStamp = new DateTime(Parser.ParseLong(bytes, ref currentByteIndex)),
		};

		DateTime newTimeStamp = replay.TimeStamp.AddYears(-1600); // je ????
		replay.ReplayTimeStamp = newTimeStamp.ToBinary();

		replay.CompressedReplayLength = Parser.ParseInt(bytes, ref currentByteIndex);
		if (skipInputsParsing) {
			if (replay.CompressedReplayLength == -1)
				currentByteIndex += 4;
			else
				currentByteIndex += replay.CompressedReplayLength;
			replay.Inputs = null;
		}
		else
			parseInputs(bytes, replay.CompressedReplayLength, ref currentByteIndex, ref replay);
		replay.ScoreID = Parser.ParseInt(bytes, ref currentByteIndex);

		replay.SizeInBytes = currentByteIndex - start;

		return replay;
	}
}
