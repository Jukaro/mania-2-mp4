using System.Collections.Generic;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Rythmify.UI;

public static class Paths {
	public static string OsuDirectoryPath;
	public static string OsuSongsDirectoryPath;

	public static string SavingDirectory = "Config";
	private static string _pathsFile;

	public static void Init() {
		if (!Directory.Exists(SavingDirectory))
			Directory.CreateDirectory(SavingDirectory);

		_pathsFile = Path.Combine(SavingDirectory, "Paths.txt");
	}

	public static void Save() {
		string str = $"OsuDirectory={Paths.OsuDirectoryPath}\n";
		str += $"OsuSongsDirectory={Paths.OsuSongsDirectoryPath}";

		File.WriteAllText(_pathsFile, str);
	}

	public static void TryLoad() {
		if (!File.Exists(_pathsFile))
			return;

		Dictionary<string, string> paths = ProgramDataParser.GetKeyValueDictFromFile(_pathsFile, '=');
		if (!paths.ContainsKey("OsuDirectory") || !paths.ContainsKey("OsuSongsDirectory"))
			return;

		OsuDirectoryPath = paths["OsuDirectory"];
		OsuSongsDirectoryPath = paths["OsuSongsDirectory"];
	}
}
