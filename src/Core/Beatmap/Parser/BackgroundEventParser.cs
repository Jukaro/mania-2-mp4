using System;

namespace Rythmify.Core.Beatmap;

public partial class BeatmapParser {
	private static BackgroundEvent ParseBackgroundEvent(string[] parameters) {
		BackgroundEvent backgroundEvent = new();

		if (parameters.Length != 5)
			throw new ArgumentException($"Background event must have 5 parameters, but got {parameters.Length}");

		return backgroundEvent;
	}
}
