using System;
using System.Collections.Generic;

namespace Rythmify.Core.Beatmap;

public partial class BeatmapParser {

	private static BeatmapColor[] ParseColorsSection(string[] lines) {
		List<BeatmapColor> colors = new();

		foreach (string line in lines) {
			var (key, arguments) = GetKeyAndArguments(line);
			if (arguments.Length != 1)
				throw new ArgumentException($"Color is not in the key:arguments format | {line}");

			var colorChannels = arguments[0].Split(',');
			if (colorChannels.Length != 3)
				throw new ArgumentException($"Color must have 3 parameters, but got {colorChannels.Length}");

			BeatmapColor color = new()
			{
				Label = key,
				R = byte.Parse(colorChannels[0]),
				G = byte.Parse(colorChannels[1]),
				B = byte.Parse(colorChannels[2]),
			};

			colors.Add(color);
		}

		return colors.ToArray();
	}
}
