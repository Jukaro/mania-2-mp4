using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Rythmify.Core;


public class IniParser {
	private enum IniLineType {
		Assignment,
		Section,
		None
	}

	private static readonly Regex AssignmentRegex = new(@"^([\dA-Za-z_]+):(.+)$");
	private static readonly Regex SectionRegex = new(@"^\[(.+)\]$");

	private static readonly List<Tuple<Regex, IniLineType>> RegexToLineType = new() {
		new(AssignmentRegex, IniLineType.Assignment),
		new(SectionRegex, IniLineType.Section)
	};

	// We use a List of IniSection because we can have multiple sections with the same name
	private readonly Dictionary<string, List<IniSection>> _sections = new();

	public IniParser(string fileName) {
		Parse(fileName);
	}

	private static IniLineType GetLineType(string line) {
		IniLineType? type = RegexToLineType.FirstOrDefault(tuple => tuple.Item1.IsMatch(line), null)?.Item2;
		return type ?? IniLineType.None;
	}

	private IniSection HandleSection(string line) {
		Match sectionMatch = SectionRegex.Match(line);
		string newSectionName = sectionMatch.Groups[1].Value.Trim();
		Logger.LogInfo($"[INIParser] Parsing Section [{newSectionName}]");
		return AddSection(newSectionName);
	}

	private static void HandleAssignment(IniSection section, string line) {
		Match assignmentMatch = AssignmentRegex.Match(line);
		var (key, value) = (assignmentMatch.Groups[1].Value.Trim(), assignmentMatch.Groups[2].Value.Trim());
		section.SetValue(key, value);
	}

	public void Parse(string iniPath) {
		string content = File.ReadAllText(iniPath);
		IniSection currentSection = null;

		string[] lines = content.Split('\n')
			.Select(line => CleanLine(line))
			.Where(line => !string.IsNullOrWhiteSpace(line))
			.ToArray();

		foreach (string line in lines) {
			IniLineType lineType = GetLineType(line);

			if (lineType == IniLineType.Section)
				currentSection = HandleSection(line);
			else if (lineType == IniLineType.Assignment && currentSection != null)
				HandleAssignment(currentSection, line);
		}
	}

	private string CleanLine(string line) {
		line = line.Trim();
		int commentIndex = line.IndexOf("//");
		if (commentIndex != -1) line = line[..commentIndex];
		return line;
	}

	private IniSection AddSection(string name) {
		if (!_sections.ContainsKey(name))
			_sections[name] = new List<IniSection>();
		_sections[name].Add(new IniSection(name));
		return _sections[name].Last();
	}

	public List<IniSection> GetSections(string name) => _sections[name];

	public IniSection GetSection(string name, int index = 0) {
		if (!_sections.ContainsKey(name) || index >= _sections[name].Count || index < 0)
			return null;
		return _sections[name][index];
	}

	public IniSection FindSection(string name, Predicate<IniSection> predicate) {
		if (!_sections.ContainsKey(name))
			return null;
		return _sections[name].Find(predicate);
	}
}
