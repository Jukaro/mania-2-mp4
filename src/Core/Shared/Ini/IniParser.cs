using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Rythmify.Core;

public class IniParser {
	private static readonly Regex AssignmentRegex = new(@"^([\dA-Za-z_]+):(.+)$");
	private static readonly Regex SectionRegex = new(@"^\[(.+)\]$");

	// We use a List of IniSection because we can have multiple sections with the same name
	private readonly Dictionary<string, List<IniSection>> _sections = new();

	public IniParser(string fileName) {
		Parse(fileName);
	}

	public void Parse(string iniPath) {
		string content = File.ReadAllText(iniPath);
		string currentSectionName = null;

		string[] lines = content.Split('\n')
			.Select(line => line.Trim())
			.Where(line => !string.IsNullOrWhiteSpace(line)).ToArray();

		foreach (string line in lines) {
			Match sectionMatch = SectionRegex.Match(line);
			bool isSection = sectionMatch.Success;
			if (isSection) {
				currentSectionName = sectionMatch.Groups[1].Value.Trim();
				AddSection(currentSectionName);
				Logger.LogInfo($"[INIParser] Parsing Section [{currentSectionName}]");
				continue;
			}

			Match assignmentMatch = AssignmentRegex.Match(line);
			bool isAssignment = assignmentMatch.Success;
			if (isAssignment && currentSectionName != null) {
				var (key, value) = (assignmentMatch.Groups[1].Value.Trim(), assignmentMatch.Groups[2].Value.Trim());
				_sections[currentSectionName].Last().SetValue(key, value);
			}
		}
	}

	public void AddSection(string name) {
		if (!_sections.ContainsKey(name))
			_sections[name] = new List<IniSection>();
		_sections[name].Add(new IniSection(name));
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
