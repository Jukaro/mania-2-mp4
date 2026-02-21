using System.Linq;
using Rythmify.Core.Beatmap;
using Rythmify.Core.Replay;

public class SessionScore {
	public ReplayData Score;

	public int ScoreDiffVsTopScore;
	public float PPDiffVsTopScore;
	public int LocalRank;
	public int LocalRankWhenSet;
	public string TimestampInHours;

	// Extended Stats
	public int ScoreCountWhenSet;
	public float AccDiffVsTopScore;
	public float RatioDiffVsTopScore;

	public SessionScore(ReplayData score, BeatmapWithScores beatmap) {
		Score = score;
		TimestampInHours = score.TimeStamp.TimeOfDay.ToString(@"%h'h'mm");

		var beatmapScores = beatmap.Replays.Where(r => r.TimeStamp <= Score.TimeStamp
			&& r.Mods == Score.Mods)
			.OrderByDescending(r => r.Score)
			.ToList();

		int localRankWhenSet = beatmapScores.IndexOf(Score) + 1;
		LocalRankWhenSet = localRankWhenSet;

		ReplayData bestScore = Score;
		if (localRankWhenSet == 1 && beatmapScores.Count > 1) bestScore = beatmapScores[1];
		else if (beatmapScores.Count > 1) bestScore = beatmapScores[0];
		bestScore.Accuracy = ScoreMetrics.ComputeV1Accuracy(bestScore);

		ScoreCountWhenSet = beatmapScores.Count;
		SetDiffs(bestScore);
	}

	private void SetDiffs(ReplayData bestScore) {
		ScoreDiffVsTopScore = Score.Score - bestScore.Score;
		PPDiffVsTopScore = (float)(Score.PerformancePoints - bestScore.PerformancePoints);
		AccDiffVsTopScore = Score.Accuracy - bestScore.Accuracy;
		RatioDiffVsTopScore = Score.Ratio - bestScore.Ratio;
	}
}
