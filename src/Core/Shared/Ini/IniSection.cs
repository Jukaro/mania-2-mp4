using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace Rythmify.Core;

public class IniSection {
	public string Name { get; private set; }
	private readonly Dictionary<string, string> _values;

	public IniSection(string name) {
		Name = name;
		_values = new();
	}

	public void SetValue(string key, string value) => _values[key] = value;
	public string GetValue(string key) => _values[key];

	public T Bind<T>() where T : new() {
		var type = typeof(T);
		var instance = Activator.CreateInstance(type);

		foreach (var (key, value) in _values) {
			FieldInfo field = type.GetField(key);

			if (field == null) {
				Logger.LogWarning($"[INIParser] Field {key} not found in {type.Name}");
				continue;
			}

			if (field.FieldType == typeof(string))
				field.SetValue(instance, value);
			else if (field.FieldType == typeof(int))
				field.SetValue(instance, int.Parse(value));
			else if (field.FieldType == typeof(bool))
				field.SetValue(instance, int.Parse(value) != 0);
			else if (field.FieldType == typeof(float))
				field.SetValue(instance, float.Parse(value, CultureInfo.InvariantCulture));
			else if (field.FieldType == typeof(double))
				field.SetValue(instance, double.Parse(value, CultureInfo.InvariantCulture));
			else
				Logger.LogWarning($"[INIParser] Field {key} has an unsupported type {field.FieldType}");
		}

		return (T)instance;
	}

	public override string ToString() {
		string result = $"[{Name}]\n";
		foreach (var (key, value) in _values)
			result += $"{key}: {value}\n";
		return result;
	}
}
