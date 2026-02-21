using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Mania2mp4.Models;
using Rythmify.Core;
using Rythmify.Core.Beatmap;
using Rythmify.Core.Databases;
using Rythmify.Core.Replay;
using Rythmify.Core.Shared;
using Rythmify.UI;

public static class WIPFunctions {
	public static void RewriteReplayBeatmapMD5() {
		var replay = ReplayParser.Parse("G:/Jeux/osssu/Replays/Jukaro - Make Them Suffer - Ghost Of Me [Hopeless (cut)] (2025-10-24) OsuMania.osr", 4, false);
		replay.BeatmapMD5 = "b907a549a454fa3f061b5f2468029dee";
		Logger.LogDebug($"replay md5: {replay.BeatmapMD5}");
		Logger.LogDebug($"replay timestamp: {replay.TimeStamp}");
		// var datetime = new DateTime(replay.ReplayTimeStamp, DateTimeKind.Utc).AddYears(1600).ToLocalTime();
		var datetime = new DateTime(replay.TimeStamp.ToUniversalTime().Ticks, DateTimeKind.Utc).ToLocalTime();
		Logger.LogDebug($"replay timestamp from timestamp: {datetime}");
		ReplayWriter.Write("test.osr", replay);
	}

	public static void MergeDatabases() {
		var beatmapDB1 = BeatmapDBParser.Parse("D:\\Downloads\\osu!_current.db");
		var beatmapDB2 = BeatmapDBParser.Parse("D:\\Downloads\\osu!_old.db");

		var scoreDB1 = ScoreDBParser.Parse("D:\\Downloads\\scores_current.db", beatmapDB1);
		var scoreDB2 = ScoreDBParser.Parse("D:\\Downloads\\scores_old.db", beatmapDB2);

		int totalSkippedScoresCount = 0;
		Dictionary<GameMode, int> skippedScoresCount = new() {
			{ GameMode.Standard, 0 },
			{ GameMode.Mania, 0 },
			{ GameMode.Taiko, 0 },
			{ GameMode.CatchTheBeat, 0 },
		};

		int totalAddedScoresCount = 0;
		Dictionary<GameMode, int> addedScoresCount = new() {
			{ GameMode.Standard, 0 },
			{ GameMode.Mania, 0 },
			{ GameMode.Taiko, 0 },
			{ GameMode.CatchTheBeat, 0 },
		};

		Logger.LogDebug($"current scoreDB score count: {scoreDB1.AllGameModesBeatmaps.Values.Sum(b => b.Replays.Count)}");

		foreach (var entry in scoreDB2.AllGameModesBeatmaps) {
			bool hasScoresInTheReceivingDatabase = scoreDB1.AllGameModesBeatmaps.ContainsKey(entry.Key);
			bool existsInTheReceivingDatabase = beatmapDB1.Beatmaps.ContainsKey(entry.Key);
			var dbInfo = entry.Value.BeatmapDBInfo;
			int scoreCount = entry.Value.Replays.Count;

			if (!hasScoresInTheReceivingDatabase && !existsInTheReceivingDatabase) {
				if (scoreCount > 0)
					Logger.LogWarning($"({dbInfo.Mode}) {dbInfo.SongTitle} [{dbInfo.Difficulty}] was not found in receiving database and {scoreCount} scores were skipped");
				totalSkippedScoresCount += scoreCount;
				skippedScoresCount[dbInfo.Mode] += scoreCount;
				continue;
			}

			totalAddedScoresCount += scoreCount;
			addedScoresCount[dbInfo.Mode] += scoreCount;

			if (!hasScoresInTheReceivingDatabase)
				scoreDB1.AllGameModesBeatmaps.Add(entry.Key, new (entry.Value.BeatmapDBInfo));

			BeatmapWithScores beatmap = scoreDB1.AllGameModesBeatmaps[entry.Key];
			beatmap.Replays = beatmap.Replays.UnionBy(entry.Value.Replays, r => r.ReplayMD5).ToList();
		}

		Logger.LogDebug($"{totalSkippedScoresCount} scores were skipped in total ({string.Join(", ", skippedScoresCount.Select(kv => $"{kv.Key}: {kv.Value}"))})");
		Logger.LogDebug($"{totalAddedScoresCount} scores were added in total ({string.Join(", ", addedScoresCount.Select(kv => $"{kv.Key}: {kv.Value}"))})");
		Logger.LogDebug($"current scoreDB score count: {scoreDB1.AllGameModesBeatmaps.Values.Sum(b => b.Replays.Count)}");

		ScoreDBWriter.Write(scoreDB1, Path.Combine(Paths.OsuDirectoryPath, "scores2.db"));
	}

	public static async Task UpdateScoreDB(DatabasesService databasesModel, string replaysFolder) {
		string[] originalReplayPaths = Directory.GetFiles(replaysFolder, "*.osr");
		string[] replayPaths = Directory.GetFiles(replaysFolder, "*.osr").Select(f => Path.GetFileNameWithoutExtension(f)).ToArray();
		List<string> databaseReplays = new();

		foreach (BeatmapWithScores beatmap in databasesModel.ScoreDB.AllGameModesBeatmaps.Values) {
			databaseReplays.AddRange(beatmap.Replays.Select(r => Path.GetFileNameWithoutExtension(r.FilePath)));
		}

		int isNotInDBCount = 0;
		DateTime limit = new DateTime(2024, 9, 1);

		for (int i = 0; i < replayPaths.Count(); i++) {
			long ticks;
			long.TryParse(replayPaths[i].Split("-")[1], out ticks);
			DateTime utc = DateTime.FromFileTimeUtc(ticks);
			DateTime local = utc.ToLocalTime();

			string beatmapMD5 = replayPaths[i].Split("-")[0];
			string beatmapName = beatmapMD5;
			string beatmapDifficulty = "";

			if (databasesModel.BeatmapDB.Beatmaps.ContainsKey(beatmapMD5)) {
				beatmapName = databasesModel.BeatmapDB.Beatmaps[beatmapMD5].SongTitle;
				beatmapDifficulty = databasesModel.BeatmapDB.Beatmaps[beatmapMD5].Difficulty;
			}

			bool isInDB = databaseReplays.Contains(replayPaths[i]);

			if (!isInDB && beatmapDifficulty != "") {
				Logger.LogDebug($"file[{isNotInDBCount}]: {beatmapName} [{beatmapDifficulty}] ({local}) {databaseReplays.Contains(replayPaths[i])}");
				ReplayData score = ReplayParser.Parse(originalReplayPaths[i], 4, true);
				if (!databasesModel.ScoreDB.AllGameModesBeatmaps.ContainsKey(beatmapMD5))
					databasesModel.ScoreDB.AllGameModesBeatmaps[beatmapMD5] = new BeatmapWithScores(databasesModel.BeatmapDB.Beatmaps[beatmapMD5]);
				databasesModel.ScoreDB.AllGameModesBeatmaps[beatmapMD5].AddReplay(score);
				isNotInDBCount++;
			}
		}
	}
}
