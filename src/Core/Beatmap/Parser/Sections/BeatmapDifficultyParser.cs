using System;
using System.Collections.Generic;
using System.Globalization;

namespace Rythmify.Core.Beatmap;

public static partial class BeatmapParser {
	static readonly Dictionary<string, Action<BeatmapDifficultyData, string>> difficultyDataProperties = new() {
		{"HPDrainRate", (difficulty, value) => { difficulty.HPDrainRate = double.Parse(value, CultureInfo.InvariantCulture); } },
		{"CircleSize", (difficulty, value) => { difficulty.CircleSize = double.Parse(value, CultureInfo.InvariantCulture); } },
		{"OverallDifficulty", (difficulty, value) => { difficulty.OverallDifficulty = double.Parse(value, CultureInfo.InvariantCulture); } },
		{"ApproachRate", (difficulty, value) => { difficulty.ApproachRate = double.Parse(value, CultureInfo.InvariantCulture); } },
		{"SliderMultiplier", (difficulty, value) => { difficulty.SliderMultiplier = double.Parse(value, CultureInfo.InvariantCulture); } },
		{"SliderTickRate", (difficulty, value) => { difficulty.SliderTickRate = double.Parse(value, CultureInfo.InvariantCulture); } },
	};
}
