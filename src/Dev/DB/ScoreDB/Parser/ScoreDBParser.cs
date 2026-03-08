using System;
using System.IO;
using System.Linq;
using Rythmify.Core.Beatmap;
using Rythmify.Core.Replay;
using Rythmify.Core.Shared;

namespace Rythmify.Core.Databases;

public static partial class ScoreDBParser {
	public static ScoreDB Parse(string filePath, BeatmapDB beatmapDB) {
		var bytes = File.ReadAllBytes(filePath);
		int currentByteIndex = 0;

		ScoreDB scoreDB = new();

		scoreDB.GameVersion = Parser.ParseInt(bytes, ref currentByteIndex);
		scoreDB.BeatmapCount = Parser.ParseInt(bytes, ref currentByteIndex);

		foreach (BeatmapDataFromDB beatmap in beatmapDB.Beatmaps.Values) {
			if (beatmap.Mode == GameMode.Mania)
				scoreDB.Beatmaps.Add(beatmap.BeatmapMD5, new (beatmap));
		}

		var watch = new System.Diagnostics.Stopwatch();
		watch.Start();

		int totalScoresCount = 0;
		int maniaBeatmapCount = 0;

		for (int i = 0; i < scoreDB.BeatmapCount; i++) {
			string beatmapMD5 = Parser.ParseString(bytes, ref currentByteIndex);
			bool skip = false;

			BeatmapWithScores beatmap;
			if (beatmapDB.Beatmaps.ContainsKey(beatmapMD5)) {
				beatmap = new(beatmapDB.Beatmaps[beatmapMD5]);
			} else {
				Logger.LogWarning($"[ScoreDBParser] Beatmap {beatmapMD5} has scores but hasn't been found in the beatmap database");
				beatmap = new(beatmapDB.Beatmaps.ElementAt(0).Value);
				skip = true;
			}


			int scoresCount = Parser.ParseInt(bytes, ref currentByteIndex);
			totalScoresCount += scoresCount;

			// Logger.LogDebug($"beatmap: {beatmapDB.Beatmaps[beatmap.BeatmapDBInfo.BeatmapMD5].SongTitle} [{beatmapDB.Beatmaps[beatmap.BeatmapDBInfo.BeatmapMD5].Difficulty}], score count: {scoresCount}");

			for (int j = 0; j < scoresCount; j++) {
				// Parser.PrintNextHundredBytes(bytes, ref currentByteIndex);
				ReplayData replay = ReplayParser.Parse(bytes, currentByteIndex, 4, true);
				if (beatmap.BeatmapDBInfo.Mode == GameMode.Mania && !skip) {
					replay.PerformancePoints = ScoreMetrics.ComputePerformancePoints(replay, beatmap.BeatmapDBInfo);
					replay.Ratio = replay.Nb300s > 0 ? (float)replay.NbMax300s / replay.Nb300s : float.PositiveInfinity;
					replay.LaneCount = (int)beatmap.BeatmapDBInfo.CircleSize;
				}
				currentByteIndex += replay.SizeInBytes; // help
				beatmap.Replays.Add(replay);
			}
			if (beatmap.BeatmapDBInfo.Mode == GameMode.Mania && !skip) {
				scoreDB.Beatmaps[beatmap.BeatmapDBInfo.BeatmapMD5] = beatmap;
				maniaBeatmapCount++;
			}

			scoreDB.AllGameModesBeatmaps.Add(beatmapMD5, beatmap);
		}

		watch.Stop();
		Logger.LogInfo($"[ScoreDBParser] Successfully parsed {totalScoresCount} scores from {scoreDB.BeatmapCount} beatmaps in {watch.ElapsedMilliseconds}ms");

		return scoreDB;
	}
}
