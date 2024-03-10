using System;

public static class Logger {

	public static void LogError(string message) {
		Console.Error.WriteLine($"[ERROR] {message}");
	}

	public static void LogWarning(string message) {
		Console.WriteLine($"[WARNING] {message}");
	}

	public static void LogDebug(string message) {
		Console.WriteLine($"[DEBUG] {message}");
	}

	public static void LogInfo(string message) {
		Console.WriteLine($"[INFO] {message}");
	}

	public static void LogSuccess(string message) {
		Console.WriteLine($"[SUCCESS] {message}");
	}

	public static void LogFatal(string message) {
		Console.Error.WriteLine($"[FATAL] {message}");
	}
}