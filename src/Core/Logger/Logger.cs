using System;
using System.Linq;

namespace Rythmify.Core;

public static class Logger {

	public static void LogError(string message) {
		Console.Error.WriteLine(CreateLog(message, "[ERROR]"));
	}

	public static void LogWarning(string message) {
		Console.WriteLine(CreateLog(message, "[WARNING]"));
	}

	public static void LogDebug(string message) {
		Console.WriteLine(CreateLog(message, "[DEBUG]"));
	}

	public static void LogInfo(string message) {
		Console.WriteLine(CreateLog(message, "[INFO]"));
	}

	public static void LogSuccess(string message) {
		Console.WriteLine(CreateLog(message, "[SUCCESS]"));
	}

	public static void LogFatal(string message) {
		Console.Error.WriteLine(CreateLog(message, "[FATAL]"));
	}

	private static string CreateLog(string message, string level) {
		return string.Join("\n", message.Split("\n").Select(s => $"{level} {s}"));
	}
}
