using System;
using System.Collections.Generic;
using System.IO;

public static class ProgramDataParser {
	public static Dictionary<string, string> GetKeyValueDictFromFile(string filepath, char separator) {
		Dictionary<string, string> dict = new();

		var lines = File.ReadAllLines(filepath);

		foreach (string line in lines) {
			var (key, value) = GetKeyAndValue(line, separator);
			dict[key] = value;
		}

		return dict;
	}

	private static (string key, string value) GetKeyAndValue(string line, char separator) {
		var split = Array.ConvertAll(line.Split(separator, 2), (string s) => s.Trim());
		if (split.Length != 2)
			throw new ArgumentException($"Line is not in the key{separator}value format | {line}");

		var key = split[0];
		var value = split[1];

		return (key, value);
	}
}