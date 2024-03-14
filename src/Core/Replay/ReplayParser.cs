using System;
using System.IO;
using Rythmify.Core.Shared;

namespace Rythmify.Core.Replay;

public static partial class ReplayParser {

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

	private static bool ParseBoolFromOSR(byte[] bytes, ref int index) {
		bool result = bytes[index] != 0;
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

	public static Replay Parse(string filePath, int laneCount) {
		var bytes = File.ReadAllBytes(filePath);
		int currentByteIndex = 0;

		if (!Enum.IsDefined(typeof(GameMode), (int)bytes[currentByteIndex]))
			throw new ArgumentException($"Unexpected argument type for enum GameMode: {bytes[currentByteIndex]}");

		Replay replay = new(laneCount);

		replay.GameMode = (GameMode)bytes[currentByteIndex];
		currentByteIndex++;

		replay.GameVersion = ParseIntFromOSR(bytes, ref currentByteIndex);
		replay.BeatmapMD5 = ParseStringFromOSR(bytes, ref currentByteIndex);
		replay.PlayerName = ParseStringFromOSR(bytes, ref currentByteIndex);
		replay.ReplayMD5 = ParseStringFromOSR(bytes, ref currentByteIndex);
		replay.Nb300s = ParseShortFromOSR(bytes, ref currentByteIndex);
		replay.Nb100s = ParseShortFromOSR(bytes, ref currentByteIndex);
		replay.Nb50s = ParseShortFromOSR(bytes, ref currentByteIndex);
		replay.NbMax300s = ParseShortFromOSR(bytes, ref currentByteIndex);
		replay.Nb200s = ParseShortFromOSR(bytes, ref currentByteIndex);
		replay.NbMiss = ParseShortFromOSR(bytes, ref currentByteIndex);
		replay.Score = ParseIntFromOSR(bytes, ref currentByteIndex);
		replay.MaxCombo = ParseShortFromOSR(bytes, ref currentByteIndex);
		replay.FullCombo = ParseBoolFromOSR(bytes, ref currentByteIndex);
		replay.Mods = ParseIntFromOSR(bytes, ref currentByteIndex);
		replay.LifeBar = ParseStringFromOSR(bytes, ref currentByteIndex);
		replay.TimeStamp = ParseLongFromOSR(bytes, ref currentByteIndex);
		replay.CompressedReplayLength = ParseIntFromOSR(bytes, ref currentByteIndex);
		parseInputs(bytes, replay.CompressedReplayLength, ref currentByteIndex, ref replay);
		replay.ScoreID = ParseLongFromOSR(bytes, ref currentByteIndex);

		return replay;
	}
}
