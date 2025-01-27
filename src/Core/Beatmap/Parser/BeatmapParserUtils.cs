using System;
using System.Collections.Generic;
using System.Linq;

namespace Rythmify.Core.Beatmap;

public partial class BeatmapParser {
	private static (string key, string[] arguments) GetKeyAndArguments(string line) {
		var split = Array.ConvertAll(line.Split(':', 2), (string s) => s.Trim());
		if (split.Length < 2)
			throw new ArgumentException($"Line is not in the key:arguments format | {line}");

		var key = split[0];
		var arguments = split[1..];

		return (key, arguments);
	}

	private static T ParseKeyValueSection<T>(string[] lines, Dictionary<string, Action<T, string>> sectionDataProperties) where T : new() {
		T sectionData = new();

		foreach (string line in lines) {
			var (key, arguments) = GetKeyAndArguments(line);

			if (arguments.Length != 1)
				throw new ArgumentException($"Unexpected argument count for {key}, expected 1 but got {arguments.Length}");

			var value = arguments[0];
			if (sectionDataProperties.TryGetValue(key, out Action<T, string> propertySetter))
				propertySetter(sectionData, value);
			else
				Logger.LogWarning($"Unknown section property {key}");
		}

		return sectionData;
	}

	public static double GetLastObjectTime(BeatmapHitObject[] HitObjects) {
		if (HitObjects.Length == 0)
			return 0;

		if (HitObjects[^1] is HoldHitObject holdHitObject)
			return holdHitObject.EndTime;
		return HitObjects[^1].Time;
	}

	// Heavily inspired by osu's GetDominantBpm function
	// https://github.com/ppy/osu/blob/56f4f4033400819ba882155514296747edfe84af/osu.Game/Beatmaps/Beatmap.cs#L81
	public static double GetDominantBpm(BeatmapData beatmap) {
		double lastPlayableTime = GetLastObjectTime(beatmap.HitObjects);

		var (beatLength, duration) =
			beatmap.TimingPoints.Select((timingPoint, i) => {
				bool isTimingPointValid = timingPoint.Uninherited && timingPoint.Time <= lastPlayableTime;
				if (!isTimingPointValid) return (beatLength: timingPoint.BeatLength, duration: 0);

				if (i == beatmap.TimingPoints.Length - 1)
					return (beatLength: timingPoint.BeatLength, duration: lastPlayableTime - timingPoint.Time);

				var nextTimingPointTime = beatmap.TimingPoints.Skip(i + 1).FirstOrDefault(tp => tp.Uninherited, null)?.Time ?? lastPlayableTime;

				return (beatLength: timingPoint.BeatLength, duration: nextTimingPointTime - timingPoint.Time);
			})
			.GroupBy(timingPoint => Math.Round(timingPoint.beatLength * 1000) / 1000)
			.Select(group => (beatLength: group.Key, duration: group.Sum(t => t.duration)))
			.OrderByDescending(bpm => bpm.duration)
			.FirstOrDefault();

		if (beatLength == 0)
			return BeatmapData.DefaultBPM;

		return 60000 / beatLength;
	}
}
