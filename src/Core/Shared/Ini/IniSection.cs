using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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

	private static FieldInfo GetMatchingField(Type type, string key) {
		FieldInfo[] fields = type.GetFields();

		FieldInfo[] nonPatternFields = fields.Where(field => field.GetCustomAttribute<IniPatternAttribute>() == null).ToArray();
		FieldInfo exactMatch = nonPatternFields.FirstOrDefault(field => field.Name == key, null);
		if (exactMatch != null)
			return exactMatch;

		FieldInfo[] patternFields = fields.Where(field => field.GetCustomAttribute<IniPatternAttribute>() != null).ToArray();
		FieldInfo patternMatch = patternFields.FirstOrDefault(field => {
			IniPatternAttribute patternAttribute = field.GetCustomAttribute<IniPatternAttribute>();
			return patternAttribute.IsMatch(key);
		}, null);
		if (patternMatch != null)
			return patternMatch;

		return null;
	}

	private static bool AssignField<T>(FieldInfo field, T instance, string key, string value) {
		Type type = typeof(T);
		ConstructorInfo stringConstructor = type.GetConstructor(new Type[]{typeof(string)});

		var customHandler = field.GetCustomAttribute<IniHandlerAttribute>();
		if (customHandler != null) {
			MethodInfo handlerMethod = type.GetMethod(customHandler.Handler);
			var patternAttribute = field.GetCustomAttribute<IniPatternAttribute>();
			var pattern = patternAttribute?.Pattern;
			handlerMethod.Invoke(instance, new object[]{ field, key, value, pattern });
			return true;
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
			FieldInfo field = GetMatchingField(destinationType, key);
			if (field == null) {
				Logger.LogWarning($"[INIParser] No matching field found for {key} in {destinationType.Name}");
				continue;
			}

			bool success = AssignField(field, instance, key, value);
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
