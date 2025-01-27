using System;
using System.Collections.Generic;
using System.Globalization;

namespace Rythmify.Core.Beatmap;

public partial class BeatmapParser {
	private static BeatmapTimingPoint[] ParseTimingPointsSection(string[] lines) {
		List<BeatmapTimingPoint> timingPoints = new();

		double? firstTimingPointTime = null;

		foreach (string line in lines) {
			var parameters = Array.ConvertAll(line.Split(','), (string s) => s.Trim());

			if (parameters.Length != 8)
				throw new ArgumentException($"Timing point must have 8 parameters, but got {parameters.Length}");

			firstTimingPointTime ??= double.Parse(parameters[0], CultureInfo.InvariantCulture);

			bool isUnherited = int.Parse(parameters[6]) == 1;
			double time = double.Parse(parameters[0], CultureInfo.InvariantCulture);

			BeatmapTimingPoint timingPoint = new()
			{
				Time = Math.Abs(time - firstTimingPointTime.Value) < 1e-6 ? 0 : time,
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
