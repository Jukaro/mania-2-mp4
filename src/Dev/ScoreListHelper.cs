using System;
using System.Collections.Generic;
using System.Linq;
using Mania2mp4.Models;
using Rythmify.Core.Beatmap;
using Rythmify.Core.Replay;
using Rythmify.Core.Shared;
using Tmds.DBus.Protocol;

namespace Rythmify.Dev;

public class WeightedPP {
	public double Weight { get; set; }
	public double PerformancePoints { get; set; }

	public WeightedPP(double weight, double performancePoints) {
		Weight = weight;
		PerformancePoints = performancePoints;
	}
}

public static class ScoreListHelper {
	public static List<ReplayData> GetFilteredBestScoresPerBeatmap(DatabasesService databases, ScoreFilter filters, int numberOfScores) {
		List<ReplayData> scores = new();
		List<ReplayData> filteredScores = new();
		List<ReplayData> allTopScores = new();

		foreach (BeatmapWithScores beatmap in databases.ScoreDB.Beatmaps.Values) {
			filteredScores = beatmap.Replays
				.OrderByDescending(r => r.PerformancePoints)
				.Where(r => FilterFunction(databases, r, filters))
				.ToList();
			if (filteredScores.Count > 0) {
				allTopScores.Add(filteredScores[0]);
			}
		}

		scores = allTopScores
			.OrderByDescending(r => r.PerformancePoints)
			.Take(numberOfScores)
			.ToList();

		return scores;
	}

	private static bool FilterFunction(DatabasesService databases, ReplayData r, ScoreFilter filters) {
		string playerName = r.PlayerName;
		int keyMode = (int)databases.BeatmapDB.Beatmaps[r.BeatmapMD5].CircleSize;
		RankedStatus rankedStatus = (RankedStatus)databases.BeatmapDB.Beatmaps[r.BeatmapMD5].RankedStatus;

		if (r.TimeStamp < filters.UpperLimitDate
			&& (filters.Players.Items.Count == 0 || filters.Players.Items.Contains(playerName))
			&& (filters.KeyModes.Items.Count == 0 || filters.KeyModes.Items.Contains(keyMode))
			&& (filters.RankedStatuses.Items.Count == 0 || filters.RankedStatuses.Items.Contains(rankedStatus)))
			return true;

		return false;
	}

	public static double GetWeightedPerfomancePoints(List<ReplayData> scores) {
		if (scores == null)
			return 0;

		double totalPP = 0;

		for (int i = 0; i < scores.Count; i++) {
			double weightedPP = scores[i].PerformancePoints * Math.Pow(0.95, i);
			totalPP += weightedPP;
		}

		return totalPP;
	}

	public static List<WeightedPP> GetWeightPercentageAndPP(List<ReplayData> scores) {
		if (scores == null)
			return null;

		List<WeightedPP> weightedPPs = new();

		for (int i = 0; i < scores.Count; i++) {
			double weight = Math.Pow(0.95, i);
			double weightedPP = scores[i].PerformancePoints * weight;
			weightedPPs.Add(new (weight * 100, weightedPP));
		}

		return weightedPPs;
	}
}
