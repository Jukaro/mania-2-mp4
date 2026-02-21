using Avalonia.Media;

namespace Mania2mp4.Displays;

public class SessionScoreDisplay : ScoreDisplay {
	public int ScoreDiffVsTopScore { get; set; }
	public float PPDiffVsTopScore { get; set; }
	public int LocalRank { get; set; }
	public int LocalRankWhenSet { get; set; }
	public string TimestampInHours { get; set; }

	// Extended Stats
	public int ScoreCountWhenSet { get; set; }
	public float AccDiffVsTopScore { get; set; }
	public float RatioDiffVsTopScore { get; set; }

	// Diff Colors
	public SolidColorBrush ScoreDiffBrush { get; set; }
	public SolidColorBrush PPDiffBrush { get; set; }
	public SolidColorBrush AccDiffBrush { get; set; }
	public SolidColorBrush RatioDiffBrush { get; set; }

	public SessionScoreDisplay(SessionScore sessionScore) : base(sessionScore.Score) {
		ScoreDiffVsTopScore = sessionScore.ScoreDiffVsTopScore;
		PPDiffVsTopScore = sessionScore.PPDiffVsTopScore;
		LocalRank = sessionScore.LocalRank;
		LocalRankWhenSet = sessionScore.LocalRankWhenSet;
		TimestampInHours = sessionScore.TimestampInHours;

		ScoreCountWhenSet = sessionScore.ScoreCountWhenSet;
		AccDiffVsTopScore = sessionScore.AccDiffVsTopScore;
		RatioDiffVsTopScore = sessionScore.RatioDiffVsTopScore;
		SetDiffBrushes();
	}

	private void SetDiffBrushes() {
		ScoreDiffBrush = GetDiffBrush(ScoreDiffVsTopScore);
		PPDiffBrush = GetDiffBrush(PPDiffVsTopScore);
		AccDiffBrush = GetDiffBrush(AccDiffVsTopScore);
		RatioDiffBrush = GetDiffBrush(RatioDiffVsTopScore);
	}

	private SolidColorBrush GetDiffBrush(float diff) {
		if (diff > 0) return BrushColors.PositiveDiffBrush;
		else if (diff < 0) return BrushColors.NegativeDiffBrush;
		return BrushColors.NoDiffBrush;
	}
}
