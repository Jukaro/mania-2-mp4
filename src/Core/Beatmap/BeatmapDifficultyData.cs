namespace Rythmify.Core.Beatmap;

public class BeatmapDifficultyData {
	public double HPDrainRate;
	public double CircleSize;
	public double OverallDifficulty;
	public double ApproachRate;
	public double SliderMultiplier;
	public double SliderTickRate;

	public int LaneCount => (int)CircleSize;

	public override string ToString() => $"HPDrainRate: {HPDrainRate}\nCircleSize: {CircleSize}\nOverallDifficulty: {OverallDifficulty}\nApproachRate: {ApproachRate}\nSliderMultiplier: {SliderMultiplier}\nSliderTickRate: {SliderTickRate}";
}
