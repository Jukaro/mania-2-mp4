using System;
using Rythmify.Core.Databases;
using Rythmify.Core.Replay;
using Rythmify.Core.Shared;

public static class ScoreMetrics {
	public static float ComputeV1Accuracy(ReplayData r) {
		int totalHits = r.NbMax300s + r.Nb300s + r.Nb200s + r.Nb100s + r.Nb50s + r.NbMiss;
		float accuracy = (r.NbMax300s + r.Nb300s) * 300 + r.Nb200s * 200 + r.Nb100s * 100 + r.Nb50s * 50;
		float maxAccuracy = totalHits * 300;
		return accuracy / maxAccuracy * 100.0f;
	}

	public static float ComputeV2Accuracy(ReplayData r) {
		int totalHits = r.NbMax300s + r.Nb300s + r.Nb200s + r.Nb100s + r.Nb50s + r.NbMiss;
		float accuracy = r.NbMax300s * 320 + r.Nb300s * 300 + r.Nb200s * 200 + r.Nb100s * 100 + r.Nb50s * 50;
		float maxAccuracy = totalHits * 320;
		return accuracy / maxAccuracy * 100.0f;
	}

	public static double ComputePerformancePoints(ReplayData replay, BeatmapDataFromDB beatmap) {
		double performancePoints = 0.0f;

		double starRating = beatmap.ManiaStarRating == null ? 0.0f : beatmap.ManiaStarRating[(int)Mods.None];

		if (starRating == 0.0f)
			return performancePoints;

		if ((replay.Mods & (int)Mods.DoubleTime) != 0)
			starRating = beatmap.ManiaStarRating[(int)Mods.DoubleTime];
		else if ((replay.Mods & (int)Mods.HalfTime) != 0)
			starRating = beatmap.ManiaStarRating[(int)Mods.HalfTime];

		int totalHits = replay.NbMax300s + replay.Nb300s + replay.Nb200s + replay.Nb100s + replay.Nb50s + replay.NbMiss;

		performancePoints = 8.0 * Math.Pow(Math.Max(starRating - 0.15, 0.05), 2.2) // Star rating to pp curve
								* Math.Max(0, 5 * (ComputeV2Accuracy(replay) / 100) - 4) // From 80% accuracy, 1/20th of total pp is awarded per additional 1% accuracy
								* (1 + 0.1 * Math.Min(1, (double)totalHits / 1500)); // Length bonus, capped at 1500 notes

		return performancePoints;
	}
}
