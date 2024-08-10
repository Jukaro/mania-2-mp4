namespace Rythmify.Core;

public class SkinFontsSection {
	public string HitCirclePrefix = "default";
	public int HitCircleOverlap = -2;
	public string ScorePrefix = "score";
	public int ScoreOverlap = 0;
	public string ComboPrefix = "combo";
	public int ComboOverlap = 0;

	public override string ToString() => $"HitCirclePrefix: {HitCirclePrefix}, HitCircleOverlap: {HitCircleOverlap}, ScorePrefix: {ScorePrefix}, ScoreOverlap: {ScoreOverlap}, ComboPrefix: {ComboPrefix}, ComboOverlap: {ComboOverlap}";
}
