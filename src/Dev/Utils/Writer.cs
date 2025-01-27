using System;
using System.IO;
using System.Text;

public static class Writer {
	public static void WriteULEB128FromInt(int toWrite, FileStream fs) {
		byte singleByte;

		while (toWrite != 0) {
			singleByte = (byte)(toWrite & ((1 << 7) - 1));
			toWrite >>= 7;
			if (toWrite != 0)
				singleByte |= 1 << 7;
			else
				singleByte &= (1 << 7) - 1;
			fs.WriteByte(singleByte);
		}
	}

	public static void WriteString(string toWrite, FileStream fs) {
		if (toWrite == "")
			fs.WriteByte(0x00);
		else
			fs.WriteByte(0x0b);


		UTF8Encoding utf8 = new UTF8Encoding();
		byte[] encodedString = utf8.GetBytes(toWrite);

		WriteULEB128FromInt(encodedString.Length, fs);
		fs.Write(encodedString);
	}

	// public static KeyValuePair<int, double> ParseIntDoublePair(byte[] bytes, ref int index) {
	// 	index++;
	// 	int i = ParseInt(bytes, ref index);
	// 	index++;
	// 	double d = ParseDouble(bytes, ref index);
	// 	return new KeyValuePair<int, double>(i, d);
	// }

	// public static Dictionary<int, double> ParseIntDoubleDictionary(byte[] bytes, ref int index, bool skip) {
	// 	int pairCount = ParseInt(bytes, ref index);
	// 	if (pairCount < 0)
	// 		return null;
	// 	if (skip) {
	// 		index += pairCount * 14;
	// 		return null;
	// 	}
	// 	Dictionary<int, double> dict = new();
	// 	for (int j = 0; j < pairCount; j++) {
	// 		KeyValuePair<int, double> pair = ParseIntDoublePair(bytes, ref index);
	// 		dict[pair.Key] = pair.Value;
	// 	}
	// 	return dict;
	// }

	public static void WriteBool(bool toWrite, FileStream fs) {
		fs.WriteByte(toWrite ? (byte) 1 : (byte) 0);
	}

	public static void WriteByte(byte toWrite, FileStream fs) {
		fs.WriteByte(toWrite);
	}

	public static void WriteShort(short toWrite, FileStream fs) {
		fs.Write(BitConverter.GetBytes(toWrite));
	}

	public static void WriteInt(int toWrite, FileStream fs) {
		fs.Write(BitConverter.GetBytes(toWrite));
	}

	public static void WriteLong(long toWrite, FileStream fs) {
		fs.Write(BitConverter.GetBytes(toWrite));
	}

	public static void WriteSingle(float toWrite, FileStream fs) {
		fs.Write(BitConverter.GetBytes(toWrite));
	}

	public static void WriteDouble(double toWrite, FileStream fs) {
		fs.Write(BitConverter.GetBytes(toWrite));
	}
}
