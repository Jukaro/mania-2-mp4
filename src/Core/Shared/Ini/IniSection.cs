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

	private static bool AssignField<T>(FieldInfo field, T instance, string value) {
		Type type = typeof(T);
		ConstructorInfo stringConstructor = type.GetConstructor(new Type[]{typeof(string)});

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
		else if (stringConstructor != null)
			field.SetValue(instance, stringConstructor.Invoke(new object[]{ value }));
		else
			return false;

		return true;
	}

	public T As<T>() where T : new() {
		Type destinationType = typeof(T);
		T instance = Activator.CreateInstance<T>();

		foreach (var (key, value) in _values) {
			FieldInfo field = destinationType.GetField(key);

			if (field == null) {
				Logger.LogWarning($"[INIParser] Field {key} not found in {destinationType.Name}");
				continue;
			}

			bool success = AssignField(field, instance, value);
			if (!success)
				Logger.LogWarning($"[INIParser] Failed to assign value {value} to field {key} of type {field.FieldType.Name} in {destinationType.Name}");
		}

		return instance;
	}

	public override string ToString() {
		string result = $"[{Name}]\n";
		foreach (var (key, value) in _values)
			result += $"{key}: {value}\n";
		return result;
	}
}
