using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Rythmify.Core;

public enum SpecialStyleMode {
	None = 0,
	Back = 1,
	Right = 2,
}

public enum ComboBurstStyleMode {
	Left = 0,
	Right = 1,
	Both = 2,
}

public enum NodeBodyStyleMode {
	Stretch = 0,
	CascadeFromTop = 1,
	CascadeFromBottom = 2,
}

public class SkinManiaSection {
	public int Keys;

	public double ColumnStart = 136;
	public double ColumnRight = 19;

	[IniHandler("CommaSeparatedIntListParser")]
	public int[] ColumnSpacing = new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};

	[IniHandler("CommaSeparatedIntListParser")]
	public int[] ColumnWidth = new int[] {30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30};

	[IniHandler("CommaSeparatedIntListParser")]
	public int[] ColumnLineWidth = new int[] {2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2};

	public double BarlineHeight = 1.2;

	[IniHandler("CommaSeparatedIntListParser")]
	public int[] LightingNWidth = Array.Empty<int>();

	[IniHandler("CommaSeparatedIntListParser")]
	public int[] LightingLWidth = Array.Empty<int>();

	public double WidthForNoteHeightScale;
	public int HitPosition = 402;
	public int LightPosition = 413;
	public int ScorePosition;
	public int ComboPosition;
	public bool JudgementLine;

	// Default value is unknown in the doc
	public int LightFramePerSecond = 30;

	public SpecialStyleMode SpecialStyle = SpecialStyleMode.None;

	[IniHandler("ComboBurstStyleParser")]
	public ComboBurstStyleMode ComboBurstStyle = ComboBurstStyleMode.Right;

	public bool SplitStages;
	public double StageSeparation = 40;
	public bool SeparateScore = true;
	public bool KeysUnderNotes = false;
	public bool UpsideDown = false;
	public bool KeyFlipWhenUpsideDown = true;

	[IniHandler("IntBoolDictionaryParser")]
	[IniPattern(@"KeyFlipWhenUpsideDown(\d{1,2})")]
	public Dictionary<int, bool> KeyFlipWhenUpsideDownLanes = new();

	public bool NoteFlipWhenUpsideDown = true;

	[IniHandler("IntBoolDictionaryParser")]
	[IniPattern(@"KeyFlipWhenUpsideDown(\d{1,2})D")]
	public Dictionary<int, bool> KeyFlipWhenUpsideDownLanesD = new();

	[IniHandler("IntBoolDictionaryParser")]
	[IniPattern(@"NoteFlipWhenUpsideDown(\d{1,2})")]
	public Dictionary<int, bool> NoteFlipWhenUpsideDownLanes = new();

	[IniHandler("IntBoolDictionaryParser")]
	[IniPattern(@"NoteFlipWhenUpsideDown(\d{1,2})H")]
	public Dictionary<int, bool> NoteFlipWhenUpsideDownLanesH = new();

	[IniHandler("IntBoolDictionaryParser")]
	[IniPattern(@"NoteFlipWhenUpsideDown(\d{1,2})L")]
	public Dictionary<int, bool> NoteFlipWhenUpsideDownLanesL = new();

	[IniHandler("IntBoolDictionaryParser")]
	[IniPattern(@"NoteFlipWhenUpsideDown(\d{1,2})T")]
	public Dictionary<int, bool> NoteFlipWhenUpsideDownLanesT = new();

	public NodeBodyStyleMode NoteBodyStyle = NodeBodyStyleMode.CascadeFromTop;

	[IniHandler("IntNoteBodyStyleModeDictionaryParser")]
	[IniPattern(@"NoteBodyStyle(\d{1,2})")]
	public Dictionary<int, NodeBodyStyleMode> NoteBodyStyleLanes = new();

	[IniHandler("IntColorDictionaryParser")]
	[IniPattern(@"Colour(\d{1,2})")]
	public Dictionary<int, RGB> ColorLanes = new(); // 0 0 0 255

	[IniHandler("IntColorDictionaryParser")]
	[IniPattern(@"ColourLight(\d{1,2})")]
	public Dictionary<int, RGB> ColorLightLanes = new(); // 55 255 255

	[IniPattern("ColourColumnLine")]
	public RGB ColorColumnLine = new(255, 255, 255);

	[IniPattern("ColourBarline")]
	public RGB ColorBarline = new(255, 255, 255);

	[IniPattern("ColourJudgementLine")]
	public RGB ColorJudgementLine = new(255, 255, 255);

	[IniPattern("ColourKeyWarning")]
	public RGB ColorKeyWarning = new(255, 255, 255);

	[IniPattern("ColourHold")]
	public RGB ColorHold = new(255, 191, 51);

	[IniPattern("ColourBreak")]
	public RGB ColorBreak = new(255, 0, 0);

	[IniPattern(@"KeyImage(\d{1,2})")]
	[IniHandler("IntStringDictionaryParser")]
	public Dictionary<int, string> KeyImageLanes = new();

	[IniPattern(@"KeyImage(\d{1,2})D")]
	[IniHandler("IntStringDictionaryParser")]
	public Dictionary<int, string> KeyImageLanesD = new();

	[IniPattern(@"NoteImage(\d{1,2})")]
	[IniHandler("IntStringDictionaryParser")]
	public Dictionary<int, string> NoteImageLanes = new();

	[IniPattern(@"NoteImage(\d{1,2})H")]
	[IniHandler("IntStringDictionaryParser")]
	public Dictionary<int, string> NoteImageLanesH = new();

	[IniPattern(@"NoteImage(\d{1,2})L")]
	[IniHandler("IntStringDictionaryParser")]
	public Dictionary<int, string> NoteImageLanesL = new();

	[IniPattern(@"NoteImage(\d{1,2})T")]
	[IniHandler("IntStringDictionaryParser")]
	public Dictionary<int, string> NoteImageLanesT = new();

	public string StageLeft;
	public string StageRight;
	public string StageBottom;
	public string StageHint;
	public string StageLight;
	public string LightingN;
	public string LightingL;
	public string WarningArrow;
	public string Hit0;
	public string Hit50;
	public string Hit100;
	public string Hit200;
	public string Hit300;
	public string Hit300g;

	public void ComboBurstStyleParser(FieldInfo destination, string key, string value, Regex pattern) {
		if (int.TryParse(value, out int result))
			destination.SetValue(this, (ComboBurstStyleMode)result);
		else if (Enum.TryParse(value, out ComboBurstStyleMode result2))
			destination.SetValue(this, result2);
		else
			Logger.LogWarning($"[SkinManiaSection] Invalid ComboBurstStyle value: {value}");
	}

	public void IntStringDictionaryParser(FieldInfo destination, string key, string value, Regex pattern) {
		var matches = pattern.Match(key).Groups;
		var keyIndex = int.Parse(matches[1].Value);

		var dict = (Dictionary<int, string>)destination.GetValue(this);
		dict.Add(keyIndex, value);
	}

	public void IntBoolDictionaryParser(FieldInfo destination, string key, string value, Regex pattern) {
		var matches = pattern.Match(key).Groups;
		var keyIndex = int.Parse(matches[1].Value);

		var dict = (Dictionary<int, bool>)destination.GetValue(this);
		var boolValue = int.Parse(value) != 0;
		dict.Add(keyIndex, boolValue);
	}

	public void IntColorDictionaryParser(FieldInfo destination, string key, string value, Regex pattern) {
		var matches = pattern.Match(key).Groups;
		var keyIndex = int.Parse(matches[1].Value);

		var dict = (Dictionary<int, RGB>)destination.GetValue(this);
		var color = new RGB(value);
		dict.Add(keyIndex, color);
	}

	public void IntEnumDictionaryParser<T>(FieldInfo destination, string key, string value, Regex pattern) {
		var matches = pattern.Match(key).Groups;
		var keyIndex = int.Parse(matches[1].Value);

		var dict = (Dictionary<int, T>)destination.GetValue(this);
		var enumValue = (T)Enum.Parse(typeof(T), value);
		dict.Add(keyIndex, enumValue);
	}

	public void IntNoteBodyStyleModeDictionaryParser(FieldInfo destination, string key, string value, Regex pattern) =>
		IntEnumDictionaryParser<NodeBodyStyleMode>(destination, key, value, pattern);

	public void CommaSeparatedIntListParser(FieldInfo destination, string key, string value, Regex pattern) {
		var values = value.Split(',').Select(int.Parse).ToArray();
		destination.SetValue(this, values);
	}

	public override string ToString() {
		string columnSpacing = string.Join(", ", ColumnSpacing);
		string columnWidth = string.Join(", ", ColumnWidth);
		string columnLineWidth = string.Join(", ", ColumnLineWidth);
		string lightingNWidth = string.Join(", ", LightingNWidth);
		string lightingLWidth = string.Join(", ", LightingLWidth);

		string keyFlipWhenUpsideDownLanes = string.Join(", ", KeyFlipWhenUpsideDownLanes.Select(kv => $"{kv.Key}: {kv.Value}"));
		string keyFlipWhenUpsideDownLanesD = string.Join(", ", KeyFlipWhenUpsideDownLanesD.Select(kv => $"{kv.Key}: {kv.Value}"));
		string noteFlipWhenUpsideDownLanes = string.Join(", ", NoteFlipWhenUpsideDownLanes.Select(kv => $"{kv.Key}: {kv.Value}"));
		string noteFlipWhenUpsideDownLanesH = string.Join(", ", NoteFlipWhenUpsideDownLanesH.Select(kv => $"{kv.Key}: {kv.Value}"));
		string noteFlipWhenUpsideDownLanesL = string.Join(", ", NoteFlipWhenUpsideDownLanesL.Select(kv => $"{kv.Key}: {kv.Value}"));
		string noteFlipWhenUpsideDownLanesT = string.Join(", ", NoteFlipWhenUpsideDownLanesT.Select(kv => $"{kv.Key}: {kv.Value}"));
		string noteBodyStyleLanes = string.Join(", ", NoteBodyStyleLanes.Select(kv => $"{kv.Key}: {kv.Value}"));
		string colorLanes = string.Join(", ", ColorLanes.Select(kv => $"{kv.Key}: {kv.Value}"));
		string colorLightLanes = string.Join(", ", ColorLightLanes.Select(kv => $"{kv.Key}: {kv.Value}"));
		string keyImageLanes = string.Join(", ", KeyImageLanes.Select(kv => $"{kv.Key}: {kv.Value}"));
		string keyImageLanesD = string.Join(", ", KeyImageLanesD.Select(kv => $"{kv.Key}: {kv.Value}"));
		string noteImageLanes = string.Join(", ", NoteImageLanes.Select(kv => $"{kv.Key}: {kv.Value}"));
		string noteImageLanesH = string.Join(", ", NoteImageLanesH.Select(kv => $"{kv.Key}: {kv.Value}"));
		string noteImageLanesL = string.Join(", ", NoteImageLanesL.Select(kv => $"{kv.Key}: {kv.Value}"));
		string noteImageLanesT = string.Join(", ", NoteImageLanesT.Select(kv => $"{kv.Key}: {kv.Value}"));

		return $"Keys: {Keys}, ColumnStart: {ColumnStart}, ColumnRight: {ColumnRight}, ColumnSpacing: {columnSpacing}, ColumnWidth: {columnWidth}, ColumnLineWidth: {columnLineWidth}, BarlineHeight: {BarlineHeight}, LightingNWidth: {lightingNWidth}, LightingLWidth: {lightingLWidth}, WidthForNoteHeightScale: {WidthForNoteHeightScale}, HitPosition: {HitPosition}, LightPosition: {LightPosition}, ScorePosition: {ScorePosition}, ComboPosition: {ComboPosition}, JudgementLine: {JudgementLine}, LightFramePerSecond: {LightFramePerSecond}, SpecialStyle: {SpecialStyle}, ComboBurstStyle: {ComboBurstStyle}, SplitStages: {SplitStages}, StageSeparation: {StageSeparation}, SeparateScore: {SeparateScore}, KeysUnderNotes: {KeysUnderNotes}, UpsideDown: {UpsideDown}, KeyFlipWhenUpsideDown: {keyFlipWhenUpsideDownLanes}, NoteFlipWhenUpsideDown: {noteFlipWhenUpsideDownLanes}, NoteBodyStyle: {NoteBodyStyle}, NoteBodyStyleLanes: {noteBodyStyleLanes}, ColorLanes: {colorLanes}, ColorLightLanes: {colorLightLanes}, ColorColumnLine: {ColorColumnLine}, ColorBarline: {ColorBarline}, ColorJudgementLine: {ColorJudgementLine}, ColorKeyWarning: {ColorKeyWarning}, ColorHold: {ColorHold}, ColorBreak: {ColorBreak}, KeyImageLanes: {keyImageLanes}, KeyImageLanesD: {keyImageLanesD}, NoteImageLanes: {noteImageLanes}, NoteImageLanesH: {noteImageLanesH}, NoteImageLanesL: {noteImageLanesL}, NoteImageLanesT: {noteImageLanesT}, StageLeft: {StageLeft}, StageRight: {StageRight}, StageBottom: {StageBottom}, StageHint: {StageHint}, StageLight: {StageLight}, LightingN: {LightingN}, LightingL: {LightingL}, WarningArrow: {WarningArrow}, Hit0: {Hit0}, Hit50: {Hit50}, Hit100: {Hit100}, Hit200: {Hit200}, Hit300: {Hit300}, Hit300g: {Hit300g}";
	}
}

