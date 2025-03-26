using System.IO;
using dotenv.net;
using dotenv.net.Utilities;
using Rythmify.Core;

public static class Env {
	public static string OsuDirectoryPath;
	public static string OsuSongsDirectoryPath;

	public static void Init() {
		Logger.LogDebug(Directory.GetCurrentDirectory());

		DotEnv.Load(options: new DotEnvOptions(envFilePaths: new[] {"../../../.env"}));

		OsuDirectoryPath = EnvReader.GetStringValue("OSU_DIRECTORY_PATH");
		OsuSongsDirectoryPath = EnvReader.GetStringValue("OSU_SONGS_DIRECTORY_PATH");
	}
}
