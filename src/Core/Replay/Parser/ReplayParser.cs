using System;
using System.IO;
using Rythmify.Core.Shared;

namespace Rythmify.Core.Replay;

public static partial class ReplayParser {
	public static ReplayData Parse(byte[] bytes, int currentByteIndex, int laneCount, bool skipInputsParsing) {
		ReplayData replay = ParseBytes(bytes, currentByteIndex, laneCount, skipInputsParsing);
		replay.FilePath = "D:/osssu/Data/r/" + replay.BeatmapMD5 + "-" + replay.ReplayTimeStamp + ".osr";

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

		ReplayData replay = new(laneCount);

		replay.GameMode = (GameMode)Parser.ParseByte(bytes, ref currentByteIndex);
		replay.GameVersion = Parser.ParseInt(bytes, ref currentByteIndex);
		replay.BeatmapMD5 = Parser.ParseString(bytes, ref currentByteIndex);
		replay.PlayerName = Parser.ParseString(bytes, ref currentByteIndex);
		replay.ReplayMD5 = Parser.ParseString(bytes, ref currentByteIndex);
		replay.Nb300s = Parser.ParseShort(bytes, ref currentByteIndex);
		replay.Nb100s = Parser.ParseShort(bytes, ref currentByteIndex);
		replay.Nb50s = Parser.ParseShort(bytes, ref currentByteIndex);
		replay.NbMax300s = Parser.ParseShort(bytes, ref currentByteIndex);
		replay.Nb200s = Parser.ParseShort(bytes, ref currentByteIndex);
		replay.NbMiss = Parser.ParseShort(bytes, ref currentByteIndex);
		replay.Score = Parser.ParseInt(bytes, ref currentByteIndex);
		replay.MaxCombo = Parser.ParseShort(bytes, ref currentByteIndex);
		replay.FullCombo = Parser.ParseBool(bytes, ref currentByteIndex);
		replay.Mods = Parser.ParseInt(bytes, ref currentByteIndex);
		replay.LifeBar = Parser.ParseString(bytes, ref currentByteIndex);
		replay.TimeStamp = Parser.ParseLong(bytes, ref currentByteIndex);

		DateTime newTimeStamp = new DateTime(replay.TimeStamp).AddYears(-1600); // je ????
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
