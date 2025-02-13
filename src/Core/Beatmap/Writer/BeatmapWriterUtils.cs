using System;
using System.Collections;
using System.Globalization;

namespace Rythmify.Core.Beatmap;

public partial class BeatmapWriter {
	private static string GetFieldString(string name, object value) => $"{name}: {GetTypeString(value)}\n";

	private static string GetTypeString(object value) {
		if (value == null)
			return "";

		Type type = value.GetType();

		if (type.IsEnum)
			return ((int)value).ToString();
		else if (type == typeof(bool))
			return (bool)value ? "1" : "0";
		else if (value is float f)
			return f.ToString(CultureInfo.InvariantCulture);
		else if (value is double d)
			return d.ToString(CultureInfo.InvariantCulture);
		return value.ToString();
	}

	private static string GetArrayString(string name, object value, string separator) {
		if (value == null)
			return "";

		string str = name + ": ";

		if (value is IEnumerable array) {
			foreach (object obj in array)
				str += obj + separator;
		}
		return str.Remove(str.Length - separator.Length) + "\n";
	}

	private static string GetObjectString(object[] values, string separator) {
		if (values == null)
			return "";

		string str = "";

		foreach (object value in values)
			str += GetTypeString(value) + separator;
		return str.Remove(str.Length - separator.Length);
	}
}
