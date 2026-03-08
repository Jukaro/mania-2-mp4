using System;
using System.IO;
using System.Linq;

namespace Rythmify.Core;

public static class Logger {

	public static void LogError(string message) {
		WriteLog(message, "[ERROR]", Console.Out, ConsoleColor.Red);
	}

	public static void LogWarning(string message) {
		WriteLog(message, "[WARNING]", Console.Out, ConsoleColor.DarkYellow);
	}

	public static void LogDebug(string message) {
		WriteLog(message, "[DEBUG]", Console.Out, ConsoleColor.Blue);
	}

	public static void LogInfo(string message) {
		WriteLog(message, "[INFO]", Console.Out, ConsoleColor.White);
	}

	public static void LogSuccess(string message) {
		WriteLog(message, "[SUCCESS]", Console.Out, ConsoleColor.Green);
	}

	public static void LogFatal(string message) {
		WriteLog(message, "[FATAL]", Console.Error, ConsoleColor.DarkRed);
	}

	private static void WriteLog(string message, string level, TextWriter textWriter, ConsoleColor consoleColor) {
		if (consoleColor != ConsoleColor.White) Console.ForegroundColor = consoleColor;
		textWriter.WriteLine(CreateLog(message, level));
		Console.ResetColor();
	}

	private static string CreateLog(string message, string level) {
		return string.Join("\n", message.Split("\n").Select(s => $"{level} {s}"));
	}
}
