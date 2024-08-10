using System;
using System.Text.RegularExpressions;

[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
sealed class IniPatternAttribute : Attribute {
	public Regex Pattern { get; }

	public IniPatternAttribute(string pattern) {
		Pattern = new("^" + pattern + "$");
	}

	public bool IsMatch(string input) => Pattern.IsMatch(input);
}
