using System;
using System.Collections.Generic;

namespace Rythmify.Core.Beatmap;

public static partial class BeatmapParser {
	static readonly Dictionary<string, Action<BeatmapGeneralData, string>> generalDataProperties = new() {
		{"AudioFilename", (general, value) => { general.AudioFilename = value; } },
		{"AudioLeadIn", (general, value) => { general.AudioLeadIn = int.Parse(value); } },
		{"PreviewTime", (general, value) => { general.AudioLeadIn = int.Parse(value); } },
		{"Countdown", (general, value) => { general.Countdown = Enum.Parse<Countdown>(value); }},
		{"SampleSet", (general, value) => { general.SampleSet = value; } },
		{"StackLeniency", (general, value) => { general.StackLeniency = double.Parse(value); } },
		{"GameMode", (general, value) => { general.GameMode = Enum.Parse<GameMode>(value); } },
		{"LetterboxInBreaks", (general, value) => { general.LetterboxInBreaks = bool.Parse(value); } },
		{"UseSkinSprites", (general, value) => { general.UseSkinSprites = bool.Parse(value); } },
		{"OverlayPosition", (general, value) => { general.OverlayPosition = value; } },
		{"SkinPreference", (general, value) => { general.SkinPreference = value; } },
		{"EpilepsyWarning", (general, value) => { general.EpilepsyWarning = bool.Parse(value); }},
		{"CountdownOffset", (general, value) => { general.AudioLeadIn = int.Parse(value); } },
		{"SpecialStyle", (general, value) => { general.SpecialStyle = bool.Parse(value); } },
		{"WidescreenStoryboard", (general, value) => { general.WidescreenStoryboard = bool.Parse(value); } },
		{"SamplesMatchPlaybackRate", (general, value) => { general.SamplesMatchPlaybackRate = bool.Parse(value); } },
	};
}