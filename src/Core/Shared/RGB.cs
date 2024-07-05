using System;
using System.Linq;

namespace Rythmify.Core;

public class RGB {
	public byte R;
	public byte G;
	public byte B;

	public RGB(byte r, byte g, byte b) {
		R = r;
		G = g;
		B = b;
	}

	public RGB(string rgb) {
		string[] split = rgb.Trim().Split(',').Select(s => s.Trim()).ToArray();

		if (split.Length != 3)
			throw new ArgumentException($"Invalid RGB format, expected 3 channels but found {split.Length}");

		R = byte.Parse(split[0]);
		G = byte.Parse(split[1]);
		B = byte.Parse(split[2]);
	}

	public string ToHex() => $"#{R:X2}{G:X2}{B:X2}";

	public override string ToString() => $"{R},{G},{B}";
}
