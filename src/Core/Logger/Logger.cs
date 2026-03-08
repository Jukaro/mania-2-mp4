using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Rythmify.Core;

class ErrorInfo {
	private string _error;
	private string? _stacktrace;

	public ErrorInfo(string error, string? stacktrace) {
		_error = error;
		_stacktrace = stacktrace;
	}

	public override string ToString() {
		string str = "";

		str += $"Error: {_error}\n\n";
		if (_stacktrace != null)
			str += $"Stacktrace:\n{_stacktrace}";

		return str;
	}
}

public static class Logger {
	static readonly string currentDate;
	static readonly string logfileName;
	static readonly string errorLogsFileName;

	static readonly StringBuilder logs = new();
	static readonly List<ErrorInfo> errors = new();

	static Logger() {
		Directory.CreateDirectory("logs");
		currentDate = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");
		logfileName = $"{currentDate}-logs.txt";
		errorLogsFileName = $"{currentDate}-error-logs.txt";
	}

	public static void LogFatal(string message, string? stacktrace = null) {
		Log(message, "[FATAL]", Console.Error, ConsoleColor.DarkRed);
		errors.Add(new ErrorInfo(message, stacktrace));
	}

	public static void LogError(string message, string? stacktrace = null) {
		Log(message, "[ERROR]", Console.Out, ConsoleColor.Red);
		errors.Add(new ErrorInfo(message, stacktrace));
	}

	public static void LogWarning(string message) {
		Log(message, "[WARNING]", Console.Out, ConsoleColor.DarkYellow);
	}

	public static void LogDebug(string message) {
		Log(message, "[DEBUG]", Console.Out, ConsoleColor.Blue);
	}

	public static void LogInfo(string message) {
		Log(message, "[INFO]", Console.Out, ConsoleColor.White);
	}

	public static void LogSuccess(string message) {
		Log(message, "[SUCCESS]", Console.Out, ConsoleColor.Green);
	}

	private static void Log(string message, string level, TextWriter textWriter, ConsoleColor consoleColor) {
		string toLog = CreateLog(message, level);

		if (consoleColor != ConsoleColor.White)
			Console.ForegroundColor = consoleColor;

		textWriter.WriteLine(toLog);
		Console.ResetColor();
		logs.AppendLine(toLog);
	}

	private static string CreateLog(string message, string level) {
		return string.Join("\n", message.Split("\n").Select(s => $"{level} {s}"));
	}

	public static void FlushLogs() {
		File.AppendAllText(Path.Combine("logs", logfileName), logs.ToString());
		logs.Clear();

		if (errors.Count < 1) return;

		File.AppendAllText(Path.Combine("logs", errorLogsFileName), GetErrorsString());
		errors.Clear();
	}

	private static string GetErrorsString() {
		var stringBuilder = new StringBuilder();

		for (int i = 0; i < errors.Count; i++) {
			stringBuilder.Append($"\n\n\n\n\n\n------ Error #{i} ------\n\n");
			stringBuilder.AppendLine(errors[i].ToString());
		}

		return stringBuilder.ToString();
	}
}
