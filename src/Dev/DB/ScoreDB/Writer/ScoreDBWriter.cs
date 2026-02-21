using System.Collections.Generic;
using System.IO;
using System.Linq;
using Rythmify.Core.Beatmap;
using Rythmify.Core.Replay;

namespace Rythmify.Core.Databases;

public static partial class ScoreDBWriter {
	public static void Write(ScoreDB scoreDB, string filePath) {
		if (File.Exists(filePath)) {
			FileStream erase = new FileStream(filePath, FileMode.Truncate, FileAccess.Write);
			erase.Close();
		}
		FileStream fs = new FileStream(filePath, FileMode.Append, FileAccess.Write);

		Writer.WriteInt(scoreDB.GameVersion, fs);
		Writer.WriteInt(scoreDB.AllGameModesBeatmaps.Count, fs);

		var watch = new System.Diagnostics.Stopwatch();
		watch.Start();

		int totalScoresCount = 0;
		List<BeatmapWithScores> beatmaps = scoreDB.AllGameModesBeatmaps.Values.ToList();

		for (int i = 0; i < scoreDB.AllGameModesBeatmaps.Count; i++) {
			BeatmapWithScores beatmap = beatmaps[i];
			int scoreCount = beatmap.Replays.Count;

			beatmap.Replays = beatmap.Replays.OrderByDescending(r => r.Score).ToList();
			Writer.WriteString(beatmap.BeatmapDBInfo.BeatmapMD5, fs);
			totalScoresCount += scoreCount;
			Writer.WriteInt(scoreCount, fs);

			for (int j = 0; j < scoreCount; j++) {
				ReplayData score = beatmap.Replays[j];

				Writer.WriteByte((byte)score.GameMode, fs);
				Writer.WriteInt(score.GameVersion, fs);
				Writer.WriteString(score.BeatmapMD5, fs);
				Writer.WriteString(score.PlayerName, fs);
				Writer.WriteString(score.ReplayMD5, fs);
				Writer.WriteShort(score.Nb300s, fs);
				Writer.WriteShort(score.Nb100s, fs);
				Writer.WriteShort(score.Nb50s, fs);
				Writer.WriteShort(score.NbMax300s, fs);
				Writer.WriteShort(score.Nb200s, fs);
				Writer.WriteShort(score.NbMiss, fs);
				Writer.WriteInt(score.Score, fs);
				Writer.WriteShort(score.MaxCombo, fs);
				Writer.WriteBool(score.FullCombo, fs);
				Writer.WriteInt(score.Mods, fs);
				Writer.WriteString("", fs);
				Writer.WriteLong(score.TimeStamp.ToUniversalTime().Ticks, fs);
				Writer.WriteInt(-1, fs);
				Writer.WriteLong(score.ScoreID, fs);
			}
		}

		fs.Close();

		watch.Stop();
		Logger.LogDebug($"ScoreDB: Successfully wrote {totalScoresCount} scores in {watch.ElapsedMilliseconds}ms");
	}
}
