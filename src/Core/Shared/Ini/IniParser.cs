using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Rythmify.Core;

public class IniParser {
	private static readonly Regex AssignmentRegex = new(@"^([\dA-Za-z_]+):(.+)$");
	private static readonly Regex SectionRegex = new(@"^\[(.+)\]$");

	// We use an array of sections because we can have multiple sections with the same name
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
			Match assignmentMatch = AssignmentRegex.Match(line);
			Match sectionMatch = SectionRegex.Match(line);
			if (!assignmentMatch.Success && !sectionMatch.Success) continue;

			if (sectionMatch.Success) {
				string newSectionName = sectionMatch.Groups[1].Value.Trim();
				Logger.LogInfo($"[INIParser] Parsing Section [{newSectionName}]");
				AddSection(newSectionName);
				currentSectionName = newSectionName;
				continue;
			}

			if (assignmentMatch.Success && currentSectionName != null) {
				string key = assignmentMatch.Groups[1].Value.Trim();
				string value = assignmentMatch.Groups[2].Value.Trim();
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
	public IniSection GetSection(string name, int index = 0) => _sections[name][index];
	public IniSection FindSection(string name, Predicate<IniSection> predicate) => _sections[name].Find(predicate);
}
