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

			var isUnherited = int.Parse(parameters[6]) == 1;

			BeatmapTimingPoint timingPoint = new()
			{
				Time = timingPoints.Count == 0 ? 0 : double.Parse(parameters[0], CultureInfo.InvariantCulture),
				BeatLength = double.Parse(parameters[1], CultureInfo.InvariantCulture),
				Meter = int.Parse(parameters[2]),
				SampleSet = int.Parse(parameters[3]),
				SampleIndex = int.Parse(parameters[4]),
				Volume = int.Parse(parameters[5]),
				Uninherited = isUnherited,
				Effects = new(int.Parse(parameters[7])),
				LastBPM = isUnherited ? -1 : timingPoints.Count > 0 ? timingPoints[^1].BPM : 0
			};

			timingPoints.Add(timingPoint);
		}

		return timingPoints.ToArray();
	}
}
