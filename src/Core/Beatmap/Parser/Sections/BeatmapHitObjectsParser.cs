using System;
using System.Collections.Generic;

namespace Rythmify.Core.Beatmap;

public partial class BeatmapParser {

	private static BeatmapColor[] ParseHitObjectsSection(string[] lines) {
		List<BeatmapColor> colors = new();

		foreach (string line in lines) {
			var (key, arguments) = GetKeyAndArguments(line);

			if (arguments.Length != 3)
				throw new ArgumentException($"Color must have 3 parameters, but got {arguments.Length}");

			BeatmapColor color = new()
			{
				Label = key,
				R = byte.Parse(arguments[0]),
				G = byte.Parse(arguments[1]),
				B = byte.Parse(arguments[2]),
			};

			colors.Add(color);
		}

		return colors.ToArray();
	}
}
