namespace Rythmify.Core.Beatmap;

public class BeatmapColor {
	public string Label;
	public byte R;
	public byte G;
	public byte B;

	public override string ToString() => $"Label: {Label} R: {R}, G: {G}, B: {B}";
}
