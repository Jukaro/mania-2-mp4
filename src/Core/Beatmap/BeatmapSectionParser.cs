using System;
using System.Collections.Generic;

namespace Rythmify.Core.Beatmap;

public partial class BeatmapParser {
	private static (string key, string[] arguments) GetKeyAndArguments(string line) {
		var split = line.Split(':');
		if (split.Length < 2)
			throw new ArgumentException($"Line is not in the key:arguments format | {line}");

		var key = split[0].Trim();
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
				Logger.LogWarning($"Unknown editor property {key}");

		}

		return sectionData;
	}
}