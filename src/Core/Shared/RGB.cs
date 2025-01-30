using System;
using System.Linq;

namespace Rythmify.Core;

public class RGB {
	public byte R;
	public byte G;
	public byte B;
	public byte A;

	public RGB(byte r, byte g, byte b, byte a = 255) {
		R = r;
		G = g;
		B = b;
		A = a;
	}

	public RGB(string rgba) {
		string[] split = rgba.Trim().Split(',').Select(s => s.Trim()).ToArray();

		if (split.Length != 3 && split.Length != 4)
			throw new ArgumentException($"Invalid RGB format, expected 3 channels but found {split.Length}");

		R = byte.Parse(split[0]);
		G = byte.Parse(split[1]);
		B = byte.Parse(split[2]);
		A = split.Length == 4 ? byte.Parse(split[3]) : (byte)255;
	}

	public string ToHex() => $"#{R:X2}{G:X2}{B:X2}{A:X2}";

	public override string ToString() => $"{R},{G},{B},{A}";
}
