using System;
using System.Collections.Generic;
using System.Globalization;

namespace Rythmify.Core.Beatmap;

public static partial class BeatmapParser {
	static readonly Dictionary<string, Action<BeatmapGeneralData, string>> generalDataProperties = new() {
		{"AudioFilename", (general, value) => { general.AudioFilename = value; } },
		{"AudioLeadIn", (general, value) => { general.AudioLeadIn = int.Parse(value); } },
		{"PreviewTime", (general, value) => { general.AudioLeadIn = int.Parse(value); } },
		{"Countdown", (general, value) => { general.Countdown = Enum.Parse<Countdown>(value); }},
		{"SampleSet", (general, value) => { general.SampleSet = value; } },
		{"StackLeniency", (general, value) => { general.StackLeniency = double.Parse(value, CultureInfo.InvariantCulture); } },
		{"Mode", (general, value) => { general.GameMode = Enum.Parse<GameMode>(value); } },
		{"LetterboxInBreaks", (general, value) => { general.LetterboxInBreaks = int.Parse(value) == 1; } },
		{"UseSkinSprites", (general, value) => { general.UseSkinSprites = int.Parse(value) == 1; } },
		{"OverlayPosition", (general, value) => { general.OverlayPosition = value; } },
		{"SkinPreference", (general, value) => { general.SkinPreference = value; } },
		{"EpilepsyWarning", (general, value) => { general.EpilepsyWarning = int.Parse(value) == 1; } },
		{"CountdownOffset", (general, value) => { general.AudioLeadIn = int.Parse(value); } },
		{"SpecialStyle", (general, value) => { general.SpecialStyle = int.Parse(value) == 1; } },
		{"WidescreenStoryboard", (general, value) => { general.WidescreenStoryboard = int.Parse(value) == 1; } },
		{"SamplesMatchPlaybackRate", (general, value) => { general.SamplesMatchPlaybackRate = int.Parse(value) == 1; } },
	};
}
