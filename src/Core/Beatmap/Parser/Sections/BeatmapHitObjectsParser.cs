using System;
using System.Collections.Generic;

namespace Rythmify.Core.Beatmap;

public partial class BeatmapParser {
	static readonly Dictionary<HitObjectType, Func<string[], BeatmapHitObject>> objectTypeToParser = new() {
		{HitObjectType.Circle, (parameters) => { return ParseCircle(parameters); } },
		{HitObjectType.Hold, (parameters) => { return ParseHold(parameters); } },
	};

	private static BeatmapHitObject[] ParseHitObjectsSection(string[] lines) {
		List<BeatmapHitObject> hitObjects = new();

		foreach (string line in lines) {
			var parameters = Array.ConvertAll(line.Split(','), (string s) => s.Trim());

			if (parameters.Length < 5)
				throw new ArgumentException($"HitObject must have at least 5 parameters, but got {parameters.Length}");

			HitObjectTypeFlag type = new(int.Parse(parameters[3]));

			if (objectTypeToParser.TryGetValue(type.Type, out Func<string[], BeatmapHitObject> parser))
				hitObjects.Add(parser(parameters));
			else
				Logger.LogWarning($"Could not find parser for hit object type {type.Type}");
		}

		return hitObjects.ToArray();
	}


	private static CircleHitObject ParseCircle(string[] parameters) {
		if (parameters.Length < 5)
			throw new ArgumentException($"Circle must have at least 5 parameters, but got {parameters.Length}");

		CircleHitObject circle = new()
		{
			X = int.Parse(parameters[0]),
			Y = int.Parse(parameters[1]),
			Time = int.Parse(parameters[2]),
			Type = new(int.Parse(parameters[3])),
			HitSound = new(int.Parse(parameters[4])),
			HitSample = ParseHitSample(parameters.Length >= 6 ? parameters[5] : "0:0:0:0:"),
		};

		return circle;
	}

	private static HoldHitObject ParseHold(string[] parameters) {
		if (parameters.Length < 6)
			throw new ArgumentException($"Hold must have at least 6 parameters, but got {parameters.Length}");


		string endTime = parameters[5];
		string hitSample = "0:0:0:0:";

		// This is a trick to separate the hit sample from the end time
		// We have to do this because the split character for every other argument is "," except specifically for the end time and hit sample
		// of the hold object, which is ":" (ex: hitSound,endTime:hitSample)
		var hasHitSample = parameters[5].Contains(':');
		if (hasHitSample) {
			var endTimeAndHitSampleStrings = parameters[5].Split(':', 2);
			endTime = endTimeAndHitSampleStrings[0];
			hitSample = endTimeAndHitSampleStrings[1];
		}

		HoldHitObject hold = new()
		{
			X = int.Parse(parameters[0]),
			Y = int.Parse(parameters[1]),
			Time = int.Parse(parameters[2]),
			Type = new(int.Parse(parameters[3])),
			HitSound = new(int.Parse(parameters[4])),
			EndTime = int.Parse(endTime),
			HitSample = ParseHitSample(hitSample),
		};

		return hold;
	}

	private static HitSample ParseHitSample(string parametersString) {
		var parameters = parametersString.Split(':', StringSplitOptions.RemoveEmptyEntries);
		if (parameters.Length < 4)
			throw new ArgumentException($"HitSample must have at least 4 parameters, but got {parameters.Length} in {parametersString}");

		return new()
		{
			NormalSet = int.Parse(parameters[0]),
			AdditionSet = int.Parse(parameters[1]),
			Index = int.Parse(parameters[2]),
			Volume = int.Parse(parameters[3]),
			Filename = parameters.Length >= 5 ? parameters[4] : null,
		};
	}
}
