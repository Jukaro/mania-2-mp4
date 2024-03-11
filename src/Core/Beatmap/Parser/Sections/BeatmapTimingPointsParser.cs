using System;
using System.Collections.Generic;
using System.Globalization;

namespace Rythmify.Core.Beatmap;

public partial class BeatmapParser {
	private static BeatmapTimingPoint[] ParseTimingPointsSection(string[] lines) {
		List<BeatmapTimingPoint> timingPoints = new();

		foreach (string line in lines) {
			var parameters = Array.ConvertAll(line.Split(','), (string s) => s.Trim());

			if (parameters.Length != 8)
				throw new ArgumentException($"Timing point must have 8 parameters, but got {parameters.Length}");

			BeatmapTimingPoint timingPoint = new()
			{
				Time = int.Parse(parameters[0]),
				BeatLength = double.Parse(parameters[1], CultureInfo.InvariantCulture),
				Meter = int.Parse(parameters[2]),
				SampleSet = int.Parse(parameters[3]),
				SampleIndex = int.Parse(parameters[4]),
				Volume = int.Parse(parameters[5]),
				Uninherited = int.Parse(parameters[6]) == 1,
				Effects = new(int.Parse(parameters[7]))
			};

			timingPoints.Add(timingPoint);
		}

		return timingPoints.ToArray();
	}
}
