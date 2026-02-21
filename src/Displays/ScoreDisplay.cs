using Rythmify.Core.Replay;
using Rythmify.Core.Shared;

namespace Mania2mp4.Displays;

public class ScoreDisplay {
	public ReplayData Replay { get; set; }
	public BeatmapDisplay BeatmapDisplay { get; set; }

	public string DebugToolTip { get; set; }

	// General
	public string PlayerName { get; set; }
	public int Score { get; set; }
	public short MaxCombo { get; set; }
	public float Accuracy { get; set; }
	public float Ratio { get; set; }
	public double PerformancePoints { get; set; }
	public ScoreGrade Grade { get; set; }
	public string ReplayMD5 { get; set; }

	// Local
	public int ScoreDifference { get; set; }

	public ScoreDisplay(ReplayData score) {
		Replay = score;

		DebugToolTip = score.ToString();

		PlayerName = score.PlayerName;
		Score = score.Score;
		MaxCombo = score.MaxCombo;
		Accuracy = score.Accuracy;
		Ratio = score.Ratio;
		PerformancePoints = score.PerformancePoints;
		ScoreDifference = score.ScoreDifference;

		Grade = GetGrade(Accuracy, score.Mods);

		ReplayMD5 = score.ReplayMD5;
	}

	private ScoreGrade GetGrade(float accuracy, int mods) {
		int silverMods = (int)Mods.Hidden | (int)Mods.Flashlight | (int)Mods.FadeIn;

		if (accuracy == 100 && (mods & silverMods) > 0)
			return ScoreGrade.SilverSS;
		else if (accuracy == 100)
			return ScoreGrade.SS;
		else if (accuracy > 95 && (mods & silverMods) > 0)
			return ScoreGrade.SilverS;
		else if (accuracy > 95)
			return ScoreGrade.S;
		else if (accuracy > 90)
			return ScoreGrade.A;
		else if (accuracy > 80)
			return ScoreGrade.B;
		else if (accuracy > 70)
			return ScoreGrade.C;

		return ScoreGrade.D;
	}
}
