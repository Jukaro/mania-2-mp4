using System;
using System.Collections.Generic;
using Rythmify.Core;

public static class Parser {
	public static int ParseIntFromULEB128(byte[] bytes, ref int index) {
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
			index++;
			if (notEnd == 0)
				break;
			shift += 7;
		}
		return result;
	}

	public static string ParseString(byte[] bytes, ref int index) {
		string str = "";

		if (bytes[index] == 0x00)
			index++;
		else if (bytes[index] == 0x0b)
		{
			index++;
			int length = ParseIntFromULEB128(bytes, ref index);
			str = System.Text.Encoding.UTF8.GetString(bytes, index, length);
			index += length;
		}
		else
			throw new ArgumentException($"Unknown indicator for OSR string: {bytes[index]}");
		return str;
	}

	private static KeyValuePair<int, float> ParseIntDecimalPair(byte[] bytes, ref int index, bool inputIsFloat) {
		index++;
		int i = ParseInt(bytes, ref index);
		index++;
		float f = inputIsFloat ? ParseFloat(bytes, ref index) : (float)ParseDouble(bytes, ref index);
		return new KeyValuePair<int, float>(i, f);
	}

	private static Dictionary<int, float> ParseDictionary(byte[] bytes, ref int index, bool skip, bool inputIsFloat) {
		int pairCount = ParseInt(bytes, ref index);
		if (pairCount < 0)
			return null;
		if (skip) {
			index += inputIsFloat ? pairCount * (sizeof(int) + sizeof(float) + 2) : pairCount * (sizeof(int) + sizeof(double) + 2);
			return null;
		}
		Dictionary<int, float> dict = new();
		for (int j = 0; j < pairCount; j++) {
			KeyValuePair<int, float> pair = ParseIntDecimalPair(bytes, ref index, inputIsFloat);
			dict[pair.Key] = pair.Value;
		}
		return dict;
	}

	public static Dictionary<int, float> ParseIntFloatDictionary(byte[] bytes, ref int index, bool skip) => ParseDictionary(bytes, ref index, skip, true);
	public static Dictionary<int, float> ParseIntDoubleDictionary(byte[] bytes, ref int index, bool skip) => ParseDictionary(bytes, ref index, skip, false);

	public static bool ParseBool(byte[] bytes, ref int index) {
		bool result = bytes[index] != 0;
		index += 1;
		return result;
	}

	public static byte ParseByte(byte[] bytes, ref int index) {
		byte result = bytes[index];
		index += 1;
		return result;
	}

	public static short ParseShort(byte[] bytes, ref int index) {
		short result = BitConverter.ToInt16(bytes, index);
		index += 2;
		return result;
	}

	public static int ParseInt(byte[] bytes, ref int index) {
		int result = BitConverter.ToInt32(bytes, index);
		index += 4;
		return result;
	}

	public static long ParseLong(byte[] bytes, ref int index) {
		long result = BitConverter.ToInt64(bytes, index);
		index += 8;
		return result;
	}

	public static float ParseSingle(byte[] bytes, ref int index) {
		float result = BitConverter.ToSingle(bytes, index);
		index += 4;
		return result;
	}

	public static float ParseFloat(byte[] bytes, ref int index) {
		float result = BitConverter.ToSingle(bytes, index);
		index += 4;
		return result;
	}

	public static double ParseDouble(byte[] bytes, ref int index) {
		double result = BitConverter.ToDouble(bytes, index);
		index += 8;
		return result;
	}

	// for debug
	public static void PrintNextHundredBytes(byte[] bytes, ref int index) {
		string str = "";
		for (int j = index; j < index + 100; j++) {
			str += bytes[j] >= 32 && bytes[j] <= 126 ? (char)bytes[j] : " [" + bytes[j] + "] ";
		}
		Logger.LogDebug($"next 100 bytes: {str}");
	}
}
